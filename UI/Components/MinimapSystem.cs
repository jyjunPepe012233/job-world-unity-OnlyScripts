using UnityEngine;

namespace Jobworld
{
    /// <summary>
    /// 미니맵 시스템
    /// 플레이어 추적 및 목적지 표시 (모든 미니맵 UI 관리)
    /// </summary>
    public class MinimapSystem : MonoBehaviour
    {
        [Header("Camera References")]
        public Camera minimapCamera;
        public Transform player;
        
        [Header("Camera Settings")]
        public float cameraHeight = 400f;
        public bool followPlayer = true;
        
        [Header("Player Icon")]
        public RectTransform playerIcon;
        
        [Header("Destination Icon")]
        [SerializeField] private RectTransform destinationIcon;
        [SerializeField] private RectTransform minimapBounds; // 미니맵 경계 (RawImage 등)
        [SerializeField] private float edgeMargin = 20f;
        [SerializeField] private bool rotateEdgeIcon = true;
        
        // 목적지 상태
        private Transform _currentDestination;
        private bool _destinationReached = false;
        
        private void Start()
        {
            // Player 자동 찾기
            if (player == null)
            {
                player = GameObject.FindGameObjectWithTag("Player")?.transform;
            }
            
            // MinimapCamera 자동 찾기
            if (minimapCamera == null)
            {
                minimapCamera = GameObject.Find("MinimapCamera")?.GetComponent<Camera>();
            }
            
            // 초기에는 목적지 아이콘 숨김
            if (destinationIcon != null)
            {
                destinationIcon.gameObject.SetActive(false);
            }
        }
        
        void LateUpdate()
        {
            UpdateCameraPosition();
            UpdatePlayerIcon();
            UpdateDestinationIcon();
        }
        
        /// <summary>
        /// 카메라 위치 업데이트
        /// </summary>
        private void UpdateCameraPosition()
        {
            if (player == null || minimapCamera == null || !followPlayer) return;
            
            Vector3 camPos = minimapCamera.transform.position;
            camPos.x = player.position.x;
            camPos.z = player.position.z;
            camPos.y = cameraHeight;
            minimapCamera.transform.position = camPos;
            
            // 카메라는 항상 아래를 바라봄
            minimapCamera.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
        }
        
        /// <summary>
        /// 플레이어 아이콘 회전
        /// </summary>
        private void UpdatePlayerIcon()
        {
            if (player == null || playerIcon == null) return;
            
            float angle = player.eulerAngles.y;
            playerIcon.localRotation = Quaternion.Euler(0f, 0f, -angle);
        }
        
        /// <summary>
        /// 목적지 아이콘 위치 업데이트
        /// </summary>
        private void UpdateDestinationIcon()
        {
            if (_currentDestination == null || destinationIcon == null || 
                minimapCamera == null || minimapBounds == null || _destinationReached)
                return;
            
            // 월드 좌표를 뷰포트 좌표로 변환
            Vector3 viewportPos = minimapCamera.WorldToViewportPoint(_currentDestination.position);
            
            // 미니맵 안에 있는지 확인
            bool isInBounds = viewportPos.x >= 0 && viewportPos.x <= 1 &&
                             viewportPos.y >= 0 && viewportPos.y <= 1 &&
                             viewportPos.z > 0;
            
            if (isInBounds)
            {
                // 미니맵 내부: 정확한 위치에 표시
                UpdateDestinationIconInBounds(viewportPos);
            }
            else
            {
                // 미니맵 외부: 모서리에 방향 표시
                UpdateDestinationIconAtEdge(viewportPos);
            }
        }
        
        /// <summary>
        /// 미니맵 내부에 목적지 아이콘 표시
        /// </summary>
        private void UpdateDestinationIconInBounds(Vector3 viewportPos)
        {
            Vector2 minimapPos = new Vector2(
                (viewportPos.x - 0.5f) * minimapBounds.rect.width,
                (viewportPos.y - 0.5f) * minimapBounds.rect.height
            );
            
            destinationIcon.anchoredPosition = minimapPos;
            destinationIcon.localRotation = Quaternion.identity;
        }
        
        /// <summary>
        /// 미니맵 모서리에 목적지 방향 표시
        /// </summary>
        private void UpdateDestinationIconAtEdge(Vector3 viewportPos)
        {
            // 뷰포트를 중심 기준으로 변환
            Vector2 centered = new Vector2(viewportPos.x - 0.5f, viewportPos.y - 0.5f);
            
            // 미니맵 크기의 절반
            float halfWidth = minimapBounds.rect.width * 0.5f - edgeMargin;
            float halfHeight = minimapBounds.rect.height * 0.5f - edgeMargin;
            
            // 경계선까지의 스케일 계산
            float scaleX = Mathf.Abs(centered.x) > 0.001f ? halfWidth / Mathf.Abs(centered.x) : float.MaxValue;
            float scaleY = Mathf.Abs(centered.y) > 0.001f ? halfHeight / Mathf.Abs(centered.y) : float.MaxValue;
            float scale = Mathf.Min(scaleX, scaleY);
            
            // 모서리 위치 계산
            Vector2 edgePos = centered * scale;
            destinationIcon.anchoredPosition = edgePos;
            
            // 방향 회전
            if (rotateEdgeIcon)
            {
                float angle = Mathf.Atan2(centered.y, centered.x) * Mathf.Rad2Deg;
                destinationIcon.localRotation = Quaternion.Euler(0, 0, angle - 90f);
            }
        }
        
        /// <summary>
        /// 목적지 설정 (DestinationSystem에서 호출)
        /// </summary>
        public void SetDestination(Transform destination)
        {
            _currentDestination = destination;
            _destinationReached = false; // 새로운 목적지 설정 시 초기화
            
            if (destinationIcon != null)
            {
                destinationIcon.gameObject.SetActive(destination != null);
            }
        }
        
        /// <summary>
        /// 화재현장 해결 완료 시 처리 (FiremanMissionManager에서 호출)
        /// </summary>
        public void OnMissionCompleted()
        {
            _destinationReached = true;
            
            // 화재현장 해결 후 아이콘 숨김
            if (destinationIcon != null)
            {
                destinationIcon.gameObject.SetActive(false);
            }
        }
        
        /// <summary>
        /// UI 요소 동적 등록 (패널이 생성될 때 호출)
        /// </summary>
        public void RegisterUI(RectTransform playerIconRef, RectTransform destinationIconRef, RectTransform minimapBoundsRef)
        {
            playerIcon = playerIconRef;
            destinationIcon = destinationIconRef;
            minimapBounds = minimapBoundsRef;

            // 현재 목적지 상태에 따라 아이콘 표시
            if (destinationIcon != null)
            {
                // 목적지가 설정되어 있고 아직 도착하지 않았으면 활성화
                bool shouldShow = _currentDestination != null && !_destinationReached;
                destinationIcon.gameObject.SetActive(shouldShow);
            }
        }

        /// <summary>
        /// UI 등록 해제 (패널이 파괴될 때 호출)
        /// </summary>
        public void UnregisterUI()
        {
            playerIcon = null;
            destinationIcon = null;
            minimapBounds = null;
        }
        
        /// <summary>
        /// 미니맵 줌 조정
        /// </summary>
        public void SetZoom(float orthographicSize)
        {
            if (minimapCamera != null)
            {
                minimapCamera.orthographicSize = orthographicSize;
            }
        }
        
        /// <summary>
        /// 미니맵 높이 조정
        /// </summary>
        public void SetHeight(float height)
        {
            cameraHeight = height;
        }
        
        /// <summary>
        /// 플레이어 추적 토글
        /// </summary>
        public void ToggleFollow(bool follow)
        {
            followPlayer = follow;
        }
    }
}