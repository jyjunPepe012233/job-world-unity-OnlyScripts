using UnityEngine;
using System;

namespace Jobworld
{
    /// <summary>
    /// 화재 현장 목적지 도착 시스템
    /// 목적지 추적 및 도착 판정만 담당 (UI는 MinimapSystem에서 처리)
    /// </summary>
    public class DestinationSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform player;
        [SerializeField] private FireSceneSpawner fireSceneSpawner;
        [SerializeField] private MinimapSystem minimapSystem;
        
        [Header("Destination Settings")]
        [SerializeField] private float arrivalDistance = 5f;
        [SerializeField] private bool showGizmos = true;
        
        [Header("Visual Effects")]
        [SerializeField] private GameObject destinationParticlePrefab;
        [SerializeField] private float particleYOffset = 2f;
        
        // 상태
        private Transform _currentDestination;
        private bool _hasArrived = false;
        private float _arrivalTime = 0f;
        private float _departureTime = 0f;
        private GameObject _currentParticle;
        
        // 이벤트
        public event Action OnDestinationArrived;
        public event Action<float> OnDistanceUpdate;
        
        // 프로퍼티
        public bool HasArrived => _hasArrived;
        public float ArrivalTime => _arrivalTime;
        public Transform CurrentDestination => _currentDestination;
        
        private void Start()
        {
            // 자동으로 참조 찾기
            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
            
            if (fireSceneSpawner == null)
                fireSceneSpawner = FindObjectOfType<FireSceneSpawner>();
            
            if (minimapSystem == null)
                minimapSystem = FindObjectOfType<MinimapSystem>();
            
            // 초기 목적지 설정
            UpdateDestination();
        }
        
        private void Update()
        {
            if (_currentDestination == null || player == null) return;
            
            if (!_hasArrived)
            {
                CheckArrival();
            }
        }
        
        /// <summary>
        /// FireSceneSpawner에서 현재 활성화된 목적지 가져오기
        /// </summary>
        public void UpdateDestination()
        {
            if (fireSceneSpawner == null) return;
            
            // 기존 파티클 제거
            ClearDestinationParticle();
            
            var sceneInfo = fireSceneSpawner.GetCurrentSceneInfo();
            if (sceneInfo != null && sceneInfo.destinationPoint != null)
            {
                _currentDestination = sceneInfo.destinationPoint;
                _hasArrived = false;
                _departureTime = Time.time;
                
                // 목적지에 파티클 생성
                SpawnDestinationParticle();
                
                // MinimapSystem에 목적지 전달
                if (minimapSystem != null)
                {
                    minimapSystem.SetDestination(_currentDestination);
                }
                
                Debug.Log($"목적지 설정: {sceneInfo.sceneName} - {sceneInfo.locationName}");
            }
            else
            {
                _currentDestination = null;
                
                // MinimapSystem에 목적지 제거 알림
                if (minimapSystem != null)
                {
                    minimapSystem.SetDestination(null);
                }
                
                Debug.LogWarning("설정된 목적지가 없습니다.");
            }
        }
        
        /// <summary>
        /// 플레이어가 목적지에 도착했는지 확인
        /// </summary>
        private void CheckArrival()
        {
            float distance = Vector3.Distance(player.position, _currentDestination.position);
            OnDistanceUpdate?.Invoke(distance);
            
            if (distance <= arrivalDistance)
            {
                _hasArrived = true;
                _arrivalTime = Time.time - _departureTime;
                
                Debug.Log($"목적지 도착! 소요 시간: {_arrivalTime:F1}초");
                
                // 도착시 파티클 제거
                ClearDestinationParticle();
                
                OnDestinationArrived?.Invoke();
                
                // 목적지 도착 시에는 미니맵 아이콘을 숨기지 않음
                // 화재현장 해결 시에만 MinimapSystem.OnMissionCompleted()가 호출됨
            }
        }
        
        /// <summary>
        /// 목적지까지의 거리 반환
        /// </summary>
        public float GetDistanceToDestination()
        {
            if (_currentDestination == null || player == null)
                return float.MaxValue;
            
            return Vector3.Distance(player.position, _currentDestination.position);
        }
        
        /// <summary>
        /// 목적지 초기화
        /// </summary>
        public void ResetDestination()
        {
            _currentDestination = null;
            _hasArrived = false;
            _arrivalTime = 0f;
            _departureTime = 0f;
            
            // 파티클 제거
            ClearDestinationParticle();
            
            if (minimapSystem != null)
            {
                minimapSystem.SetDestination(null);
            }
        }
        
        /// <summary>
        /// 목적지 파티클 생성
        /// </summary>
        private void SpawnDestinationParticle()
        {
            if (destinationParticlePrefab == null || _currentDestination == null) return;
    
            // Y축 오프셋을 적용한 위치 계산
            Vector3 particlePosition = _currentDestination.position + Vector3.up * particleYOffset;
    
            _currentParticle = Instantiate(destinationParticlePrefab, particlePosition, Quaternion.identity);
    
            // 목적지의 자식으로 설정 (선택사항)
            // _currentParticle.transform.SetParent(_currentDestination);
    
            Debug.Log("목적지 파티클 생성");
        }
        
        /// <summary>
        /// 목적지 파티클 제거
        /// </summary>
        private void ClearDestinationParticle()
        {
            if (_currentParticle != null)
            {
                Destroy(_currentParticle);
                _currentParticle = null;
                Debug.Log("목적지 파티클 제거");
            }
        }
        
        private void OnDestroy()
        {
            // 정리
            ClearDestinationParticle();
        }
        
        private void OnDrawGizmos()
        {
            if (!showGizmos || _currentDestination == null) return;
            
            // 목적지 표시
            Gizmos.color = _hasArrived ? Color.green : Color.red;
            Gizmos.DrawWireSphere(_currentDestination.position, arrivalDistance);
            Gizmos.DrawLine(_currentDestination.position, _currentDestination.position + Vector3.up * 10f);
            
            // 플레이어에서 목적지까지 선
            if (player != null && !_hasArrived)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(player.position, _currentDestination.position);
            }
            
            #if UNITY_EDITOR
            if (_currentDestination != null)
            {
                var style = new GUIStyle();
                style.normal.textColor = _hasArrived ? Color.green : Color.red;
                style.fontSize = 12;
                
                string label = _hasArrived 
                    ? $"도착 완료 ({_arrivalTime:F1}초)" 
                    : $"목적지 ({GetDistanceToDestination():F1}m)";
                
                UnityEditor.Handles.Label(_currentDestination.position + Vector3.up * 12f, label, style);
            }
            #endif
        }
    }
}