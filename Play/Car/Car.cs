using UnityEngine;
using Oculus.Interaction;
using Photon.Pun;
using Jobworld.CarSystems;

namespace Jobworld
{
    /// <summary>
    /// 자동차 시스템의 메인 컨트롤러 - 각 시스템들을 조율하는 역할
    /// </summary>
    public class Car : MonoBehaviour
    {
        [Header("시스템 참조")]
        [SerializeField] private protected CarMovementSystem movementSystem;
        [SerializeField] private protected CarWheelSystem wheelSystem;
        [SerializeField] private protected CarSeatingSystem seatingSystem;
        [SerializeField] private protected CarInputSystem inputSystem;

        [Header("문손잡이 설정")]
        [SerializeField] private InteractorFinder doorHandleFinder;

        protected PhotonView photonView;

        #region Unity Lifecycle

        private void Start()
        {
            photonView = GetComponent<PhotonView>();
            InitializeSystems();
            ValidateReferences();
        }

        private void Update()
        {
            ProcessCarLogic();
        }

        private void FixedUpdate()
        {
            ProcessCarPhysics();
        }

        #endregion

        #region Initialization

        private void InitializeSystems()
        {
            // 각 시스템들이 서로 독립적으로 초기화됨
            movementSystem?.Initialize();
            wheelSystem?.Initialize();
            seatingSystem?.Initialize();
            inputSystem?.Initialize();
        }

        private void ValidateReferences()
        {
            if (movementSystem == null) Debug.LogError("CarMovementSystem이 할당되지 않았습니다!", this);
            if (wheelSystem == null) Debug.LogError("CarWheelSystem이 할당되지 않았습니다!", this);
            if (seatingSystem == null) Debug.LogError("CarSeatingSystem이 할당되지 않았습니다!", this);
            if (inputSystem == null) Debug.LogError("CarInputSystem이 할당되지 않았습니다!", this);
            if (doorHandleFinder == null) Debug.LogError("Door Handle Finder가 할당되지 않았습니다!", this);
        }

        #endregion

        #region Public API

        /// <summary>
        /// 문손잡이가 잡혔을 때 호출되는 콜백
        /// </summary>
        public void OnDoorHandleGrabbed()
        {
            if (seatingSystem.IsPlayerSeated)
            {
                ExitCar();
            }
            else
            {
                var playerRoot = FindPlayerFromDoorHandle();
                if (playerRoot != null)
                {
                    EnterCar(playerRoot);
                }
            }
        }

        // 상태 조회용 프로퍼티들
        public bool IsPlayerInCar => seatingSystem?.IsPlayerSeated ?? false;
        public float CurrentSpeed => movementSystem?.CurrentSpeed ?? 0f;
        public float SteeringValue => inputSystem?.CurrentSteeringValue ?? 0f;
        public float CurrentSteeringAngle => movementSystem?.CurrentSteeringAngle ?? 0f;
        public Transform CurrentPlayer => seatingSystem?.CurrentPlayer;

        #endregion

        #region Protected Methods for Network Override

        protected virtual void ProcessCarLogic()
        {
            if (seatingSystem.IsPlayerSeated)
            {
                var input = inputSystem.GetInput();
                movementSystem.ProcessInput(input);
                wheelSystem.UpdateWheels(movementSystem.CurrentSpeed, movementSystem.CurrentSteeringAngle);
            }
        }

        protected virtual void ProcessCarPhysics()
        {
            if (seatingSystem.IsPlayerSeated)
            {
                movementSystem.ApplyPhysics();
            }
        }

        #endregion

        #region Private Methods

        private Transform FindPlayerFromDoorHandle()
        {
            if (doorHandleFinder == null) return null;

            var leftHandInteractor = doorHandleFinder.LeftHandInteractorGO;
            var rightHandInteractor = doorHandleFinder.RightHandInteractorGO;

            if (leftHandInteractor != null)
            {
                Debug.Log($"왼손으로 문손잡이를 잡음: {leftHandInteractor.transform.root.name}");
                return leftHandInteractor.transform.root;
            }

            if (rightHandInteractor != null)
            {
                Debug.Log($"오른손으로 문손잡이를 잡음: {rightHandInteractor.transform.root.name}");
                return rightHandInteractor.transform.root;
            }

            return null;
        }

        private void EnterCar(Transform playerTransform)
        {
            RequestOwnershipIfNeeded(playerTransform);
            
            if (seatingSystem.SeatPlayer(playerTransform))
            {
                ResetAllSystems();
                Debug.Log($"{playerTransform.name} 가 자동차에 탑승했습니다!");
            }
        }

        private void ExitCar()
        {
            var playerName = seatingSystem.CurrentPlayer?.name ?? "Unknown";
            
            if (seatingSystem.UnseatPlayer())
            {
                ResetAllSystems();
                Debug.Log($"{playerName} 가 자동차에서 하차했습니다!");
            }
        }

        private void RequestOwnershipIfNeeded(Transform playerTransform)
        {
            if (photonView != null && !photonView.IsMine)
            {
                photonView.RequestOwnership();
                Debug.Log($"[{playerTransform.name}] 자동차 소유권 요청!");
            }
        }

        private void ResetAllSystems()
        {
            movementSystem?.ResetState();
            wheelSystem?.ResetState();
            inputSystem?.ResetState();
        }

        #endregion
    }
}