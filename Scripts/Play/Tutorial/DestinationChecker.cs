using UnityEngine;

namespace Jobworld
{
    // 목표 지점 도달 조건을 체크하는 클래스
    public class DestinationChecker : MonoBehaviour
    {
        [Header("도달 체크 설정")]
        public Transform playerTransform; // VR 플레이어 트랜스폼(직접 할당)
        public float arrivalDistance = 5.0f; // 도달 거리 임계값
        public Transform destinationTransform; // 목표 지점 Transform
        public TutorialItem tutorialItem; // 조건 충족 시 알릴 대상

        [Header("가이드 큐브 설정")]
        public GameObject guideCubePrefab; // 가이드 큐브 프리팹
        private GameObject m_guideCubeInstance; // 가이드 큐브 인스턴스
        private float m_guideCubeDistance = 1.5f; // 플레이어로부터의 거리(직선)

        private bool m_isArrived = false;

        private void Awake()
        {
            // VR 환경에서는 playerTransform을 인스펙터에서 직접 할당하거나, 외부에서 할당해야 함
            if (playerTransform == null && transform.parent != null)
            {
                playerTransform = transform.parent;
            }
            // 가이드 큐브 인스턴스 생성
            if (guideCubePrefab != null && playerTransform != null)
            {
                m_guideCubeInstance = Instantiate(guideCubePrefab);
            }
        }

        // 튜토리얼 아이템을 외부에서 할당
        public void SetTutorialItem(TutorialItem item)
        {
            tutorialItem = item;
        }

        // 도달 시 가이드 큐브 비활성화
        private void ArriveAtDestination()
        {
            m_isArrived = true;
            if (tutorialItem != null)
                tutorialItem.SetConditionMet(true);
            if (m_guideCubeInstance != null)
            {
                m_guideCubeInstance.SetActive(false);
            }
        }

        private void Update()
        {
            if (!m_isArrived && playerTransform != null && destinationTransform != null)
            {
                float dist = Vector3.Distance(playerTransform.position, destinationTransform.position);
                Debug.Log($"[DestinationChecker] Player: {playerTransform.position}, Dest: {destinationTransform.position}, Dist: {dist}");
                if (dist < arrivalDistance)
                {
                    ArriveAtDestination();
                }
            }
            // 가이드 큐브 위치를 플레이어-목표 직선 위, 플레이어에서 guideCubeDistance만큼 떨어진 곳으로 이동
            if (m_guideCubeInstance != null)
            {
                bool showCube = (playerTransform != null && destinationTransform != null);
                if (m_guideCubeInstance.activeSelf != showCube)
                    m_guideCubeInstance.SetActive(showCube);
                if (showCube)
                {
                    Vector3 playerPos = playerTransform.position;
                    Vector3 targetPos = destinationTransform.position;
                    Vector3 toTarget = targetPos - playerPos;
                    toTarget.y = 0f; // 수평만 고려
                    if (toTarget.sqrMagnitude > 0.01f)
                    {
                        Vector3 cubePos = playerPos + toTarget.normalized * m_guideCubeDistance;
                        cubePos.y = playerPos.y + 1.0f;
                        m_guideCubeInstance.transform.position = cubePos;
                    }
                }
            }
        }

        public void ResetArrival()
        {
            m_isArrived = false;
            // 도달 시 가이드 큐브 숨김
            if (m_guideCubeInstance != null)
            {
                m_guideCubeInstance.SetActive(true);
            }
        }
    }
}
