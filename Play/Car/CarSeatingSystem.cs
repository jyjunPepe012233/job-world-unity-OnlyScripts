using UnityEngine;
using Oculus.Interaction.Locomotion;
using CharacterController = UnityEngine.CharacterController;

namespace Jobworld.CarSystems
{
    /// <summary>
    /// 플레이어 탑승/하차를 독립적으로 처리하는 시스템
    /// </summary>
    public class CarSeatingSystem : MonoBehaviour
    {
        [Header("좌석 설정")]
        [SerializeField] private Transform seatTransform;
        [SerializeField] private Transform exitPosition;

        private bool isPlayerSeated = false;
        private Transform currentPlayerTransform;
        private Transform originalPlayerParent;
        private Transform playerRoot; // PlayerRoot 참조 저장
        private CharacterController playerCharacterController;
        private CapsuleCollider playerCapsuleCollider;
        private FirstPersonLocomotor playerLocomotor;
        private Vector3 originalCameraOffset; // 원래 카메라 오프셋 저장

        public bool IsPlayerSeated => isPlayerSeated;
        public Transform CurrentPlayer => playerRoot; // PlayerAvatar (루트) 반환

        public void Initialize()
        {
            if (seatTransform == null)
                Debug.LogError("Seat Transform이 할당되지 않았습니다!", this);
        }

        public bool SeatPlayer(Transform playerTransform)
        {
            playerRoot = playerTransform.root;
            var cameraRig = playerRoot.GetComponentInChildren<OVRCameraRig>();
            
            if (cameraRig == null)
            {
                Debug.LogError("플레이어에서 OVRCameraRig을 찾을 수 없습니다!");
                return false;
            }

            // PlayerCameraRig만 자동차에 태우기
            originalPlayerParent = cameraRig.transform.parent;
            currentPlayerTransform = cameraRig.transform;

            DisablePlayerMovement(playerRoot);
            PositionPlayerInSeat(cameraRig.transform, cameraRig);

            isPlayerSeated = true;
            return true;
        }

        public bool UnseatPlayer()
        {
            if (currentPlayerTransform == null) return false;

            var cameraRig = currentPlayerTransform.GetComponent<OVRCameraRig>();
            if (cameraRig != null)
            {
                PositionPlayerAtExit(currentPlayerTransform, cameraRig);
            }

            currentPlayerTransform.SetParent(originalPlayerParent);
            
            // 저장된 PlayerRoot 사용
            if (playerRoot != null)
            {
                EnablePlayerMovement(playerRoot);
            }

            isPlayerSeated = false;
            currentPlayerTransform = null;
            originalPlayerParent = null;
            playerRoot = null;

            return true;
        }

        public void SetRemotePlayerSeated(bool isSeated)
        {
            // 원격 플레이어 탑승 상태 표시용 (시각적 효과만)
            // 실제 로컬 플레이어 탑승 상태와는 별개
            Debug.Log($"원격 플레이어 탑승 상태: {isSeated}");
        }

        private void PositionPlayerInSeat(Transform cameraRigTransform, OVRCameraRig cameraRig)
        {
            var centerEye = cameraRig.centerEyeAnchor;
            // 탑승 전 원래 오프셋 저장
            originalCameraOffset = cameraRigTransform.position - centerEye.position;
            
            cameraRigTransform.position = seatTransform.position + originalCameraOffset;
            cameraRigTransform.SetParent(transform);
        }

        private void PositionPlayerAtExit(Transform cameraRigTransform, OVRCameraRig cameraRig)
        {
            // 저장된 원래 오프셋 사용
            if (exitPosition != null)
            {
                cameraRigTransform.position = exitPosition.position + originalCameraOffset;
            }
            else
            {
                cameraRigTransform.position = transform.position + transform.right * 2f + originalCameraOffset;
            }
        }

        private void DisablePlayerMovement(Transform playerRoot)
        {
            Debug.Log($"DisablePlayerMovement 호출됨. playerRoot: {playerRoot?.name}");
            
            playerCharacterController = playerRoot.GetComponentInChildren<CharacterController>();
            playerCapsuleCollider = playerRoot.GetComponentInChildren<CapsuleCollider>();
            playerLocomotor = playerRoot.GetComponentInChildren<FirstPersonLocomotor>();

            Debug.Log($"찾은 컴포넌트들 - CharacterController: {playerCharacterController?.name}, CapsuleCollider: {playerCapsuleCollider?.name}, FirstPersonLocomotor: {playerLocomotor?.name}");

            if (playerCharacterController != null) 
            {
                playerCharacterController.enabled = false;
                Debug.Log("CharacterController 비활성화됨");
            }

            if (playerCapsuleCollider != null)
            {
                playerCapsuleCollider.enabled = false;
                Debug.Log("CapsuleCollider 비활성화됨");
            }

            if (playerLocomotor != null)
            {
                playerLocomotor.GravityFactor = 0f;
                playerLocomotor.DisableMovement();
                Debug.Log("FirstPersonLocomotor 비활성화됨");
            }
        }

        private void EnablePlayerMovement(Transform playerRoot)
        {
            if (playerCharacterController != null) 
                playerCharacterController.enabled = true;

            if (playerCapsuleCollider != null)
                playerCapsuleCollider.enabled = true;

            if (playerLocomotor != null)
            {
                playerLocomotor.GravityFactor = 1f;
                playerLocomotor.EnableMovement();
            }

            playerCharacterController = null;
            playerCapsuleCollider = null;
            playerLocomotor = null;
        }
    }
}