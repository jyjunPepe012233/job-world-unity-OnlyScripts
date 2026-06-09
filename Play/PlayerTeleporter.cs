using UnityEngine;
using System.Collections;

namespace Jobworld
{
    public class PlayerTeleporter : MonoBehaviour
    {
        [Header("Teleport Settings")]
        [SerializeField] private float teleportDelay = 2f;
        [SerializeField] private bool showTeleportMessage = true;
        
        [Header("Player Reference")]
        [SerializeField] private Transform m_playerTransform;
        
        private FireSceneSpawner m_fireSceneSpawner;
        
        private void Start()
        {
            m_fireSceneSpawner = FindObjectOfType<FireSceneSpawner>();
            
            // FiremanMissionManager 이벤트 구독
            FiremanMissionManager.OnMissionCompleteGlobal += OnMissionComplete;
            FiremanMissionManager.OnMissionFailedGlobal += OnMissionFailed;
        }
        
        private void OnDestroy()
        {
            // 이벤트 구독 해제
            FiremanMissionManager.OnMissionCompleteGlobal -= OnMissionComplete;
            FiremanMissionManager.OnMissionFailedGlobal -= OnMissionFailed;
        }
        
        private void OnMissionComplete()
        {
            TeleportToDestination();
        }
        
        private void OnMissionFailed()
        {
            // 실패 시에도 대피 지점으로 이동
            TeleportToDestination();
        }
        
        
        public void TeleportToDestination()
        {
            if (m_playerTransform == null)
            {
                Debug.LogError("PlayerTeleporter: 플레이어 Transform을 Inspector에서 설정해주세요!");
                return;
            }
            
            if (m_fireSceneSpawner == null)
            {
                Debug.LogError("PlayerTeleporter: FireSceneSpawner를 찾을 수 없습니다!");
                return;
            }
            
            // 현재 활성화된 화재 현장의 DestinationPoint 가져오기
            var currentSceneInfo = m_fireSceneSpawner.GetCurrentSceneInfo();
            if (currentSceneInfo == null || currentSceneInfo.destinationPoint == null)
            {
                Debug.LogWarning("PlayerTeleporter: 활성화된 DestinationPoint를 찾을 수 없습니다!");
                return;
            }
            
            StartCoroutine(TeleportAfterDelay(currentSceneInfo.destinationPoint));
        }
        
        public void TeleportToDestinationImmediate()
        {
            if (m_playerTransform == null || m_fireSceneSpawner == null)
            {
                Debug.LogError("PlayerTeleporter: 필요한 컴포넌트가 설정되지 않았습니다!");
                return;
            }
            
            var currentSceneInfo = m_fireSceneSpawner.GetCurrentSceneInfo();
            if (currentSceneInfo?.destinationPoint != null)
            {
                PerformTeleport(currentSceneInfo.destinationPoint);
            }
        }
        
        public void TeleportToPosition(Vector3 position, Quaternion rotation = default)
        {
            if (m_playerTransform == null)
            {
                Debug.LogError("PlayerTeleporter: 플레이어 Transform을 Inspector에서 설정해주세요!");
                return;
            }
            
            if (rotation == default)
            {
                rotation = m_playerTransform.rotation;
            }
            
            StartCoroutine(TeleportToPositionAfterDelay(position, rotation));
        }
        
        private IEnumerator TeleportAfterDelay(Transform destinationPoint)
        {
            if (showTeleportMessage)
            {
                Debug.Log($"화재 진압 완료! {teleportDelay}초 후 대피 지점으로 이동합니다...");
            }
            
            yield return new WaitForSeconds(teleportDelay);
            
            PerformTeleport(destinationPoint);
        }
        
        private IEnumerator TeleportToPositionAfterDelay(Vector3 position, Quaternion rotation)
        {
            if (showTeleportMessage)
            {
                Debug.Log($"{teleportDelay}초 후 지정된 위치로 이동합니다...");
            }
            
            yield return new WaitForSeconds(teleportDelay);
            
            PerformTeleport(position, rotation);
        }
        
        private void PerformTeleport(Transform destinationPoint)
        {
            PerformTeleport(destinationPoint.position, destinationPoint.rotation);
        }
        
        private void PerformTeleport(Vector3 position, Quaternion rotation)
        {
            // CharacterController가 있으면 비활성화 후 이동
            CharacterController characterController = m_playerTransform.GetComponent<CharacterController>();
            if (characterController != null)
            {
                characterController.enabled = false;
                m_playerTransform.SetPositionAndRotation(position, rotation);
                characterController.enabled = true;
            }
            else
            {
                m_playerTransform.SetPositionAndRotation(position, rotation);
            }
            
            Debug.Log($"플레이어가 대피 지점으로 텔레포트되었습니다: {position}");
            
            // 텔레포트 완료 이벤트 (필요시 확장 가능)
            OnTeleportCompleted?.Invoke();
        }
        
        // 텔레포트 완료 이벤트
        public System.Action OnTeleportCompleted;
        
        // 수동으로 플레이어 Transform 설정
        public void SetPlayerTransform(Transform playerTransform)
        {
            m_playerTransform = playerTransform;
        }
    }
}