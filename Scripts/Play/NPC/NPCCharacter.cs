using UnityEngine;
using System.Collections.Generic;

namespace Jobworld
{
    /// <summary>
    /// 웨이포인트 기반으로 자유롭게 이동하는 NPC 캐릭터
    /// - 초기 한 번만 가장 가까운 웨이포인트로 스냅 (옵션)
    /// - 이후엔 자연스럽게 경로를 따라 이동
    /// </summary>
    public class NPCCharacter : MonoBehaviour
    {
        [Header("NPC 설정")]
        [SerializeField] private WaypointManager m_waypointManager;
        [SerializeField] private float m_moveSpeed = 2.0f;
        [SerializeField] private float m_rotationSpeed = 5f;
        [SerializeField] private Animator m_animator;

        [Header("경로 설정")]
        [SerializeField] private float m_pathOffset = 1f;       // 경로에 무작위 오프셋
        [SerializeField] private float m_arrivalDistance = 0.5f;  // 목표 도착 판정 거리
        [SerializeField] private float m_timeoutDuration = 8f;    // 타임아웃

        [Header("시작 위치 보정 (옵션)")]
        [SerializeField] private bool snapToNearestOnStart = true; // 시작 시만 스냅할지
        [SerializeField] private float snapMinDistance = 0.1f;    // 이 거리보다 멀면 스냅
        [SerializeField] private bool useRandomStartOffset = true;// 웨이포인트에 정확히 겹치지 않게 오프셋 적용

        [Header("디버그")]
        [SerializeField] private bool showDebugLogs = true;

        // 경로 관련
        private Transform m_finalTarget;
        private Queue<Transform> m_pathQueue = new Queue<Transform>();
        private Transform m_currentWaypoint;
        private Vector3 m_currentGoalPosition;

        // 이동 관련
        private Vector3 m_velocity;
        private float m_timeAtCurrentGoal;
        private string m_currentAnim;

        // 처음 여부 체크 (초기 스냅은 Awake에서 처리)
        private bool m_hasSpawned = false;

        public bool isStopped { get; set; } = false;

        private void Awake()
        {
            // 초기 스냅은 Awake에서 한 번만 수행 (StartNewJourney와 분리)
            if (snapToNearestOnStart && m_waypointManager != null && m_waypointManager.waypoints != null && m_waypointManager.waypoints.Count > 0)
            {
                Transform nearest = FindNearestWaypointToPosition(transform.position);
                if (nearest != null && Vector3.Distance(transform.position, nearest.position) > snapMinDistance)
                {
                    Vector3 offset = useRandomStartOffset
                        ? new Vector3(Random.Range(-m_pathOffset, m_pathOffset), 0f, Random.Range(-m_pathOffset, m_pathOffset))
                        : Vector3.zero;

                    transform.position = nearest.position + offset;
                    if (showDebugLogs) Debug.Log($"{gameObject.name}: 초기 스냅 -> {nearest.name} (offset {offset})");
                }
                m_hasSpawned = true;
            }
        }

        private void Start()
        {
            if (m_waypointManager?.waypoints == null || m_waypointManager.waypoints.Count == 0)
            {
                Debug.LogError($"{gameObject.name}: WaypointManager가 없거나 웨이포인트가 비어있습니다.");
                enabled = false;
                return;
            }

            StartNewJourney();
        }

        private void Update()
        {
            if (isStopped)
            {
                PlayIdle();
                return;
            }

            if (m_currentWaypoint != null)
            {
                MoveToGoal();
                CheckArrival();
            }
            else
            {
                GetNextWaypoint();
            }

            UpdateAnimation();
        }

        private void StartNewJourney()
        {
            // 단지 목표만 바꾸고 경로 계산 (절대 transform.position 변경하지 않음)
            m_finalTarget = m_waypointManager.waypoints[Random.Range(0, m_waypointManager.waypoints.Count)];
            if (showDebugLogs) Debug.Log($"{gameObject.name}: 새로운 여행 시작 - 목표: {m_finalTarget.name}");
            CalculatePath();
        }

        private void CalculatePath()
        {
            m_pathQueue.Clear();

            Transform start = FindNearestWaypoint();
            if (start == null || start == m_finalTarget)
            {
                m_pathQueue.Enqueue(m_finalTarget);
                GetNextWaypoint();
                return;
            }

            HashSet<Transform> visited = new HashSet<Transform> { start };
            Transform current = start;

            while (current != m_finalTarget && visited.Count < m_waypointManager.waypoints.Count)
            {
                Transform next = null;
                float bestDistance = float.MaxValue;

                foreach (Transform waypoint in m_waypointManager.waypoints)
                {
                    if (visited.Contains(waypoint)) continue;

                    float distToCurrent = Vector3.Distance(current.position, waypoint.position);
                    if (distToCurrent > m_waypointManager.maxConnectionDistance) continue;

                    float distToTarget = Vector3.Distance(waypoint.position, m_finalTarget.position);
                    if (distToTarget < bestDistance)
                    {
                        bestDistance = distToTarget;
                        next = waypoint;
                    }
                }

                if (next == null) break;

                m_pathQueue.Enqueue(next);
                visited.Add(next);
                current = next;
            }

            GetNextWaypoint();
        }

        private Transform FindNearestWaypoint()
        {
            return FindNearestWaypointToPosition(transform.position);
        }

        private Transform FindNearestWaypointToPosition(Vector3 position)
        {
            Transform nearest = null;
            float minDist = float.MaxValue;

            foreach (Transform waypoint in m_waypointManager.waypoints)
            {
                float dist = Vector3.Distance(position, waypoint.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    nearest = waypoint;
                }
            }

            return nearest;
        }

        private void GetNextWaypoint()
        {
            if (m_pathQueue.Count > 0)
            {
                m_currentWaypoint = m_pathQueue.Dequeue();
            }
            else
            {
                StartNewJourney(); // 경로 완료 시 새 여행 시작
                return;
            }

            Vector3 offset = new Vector3(
                Random.Range(-m_pathOffset, m_pathOffset),
                0,
                Random.Range(-m_pathOffset, m_pathOffset)
            );

            m_currentGoalPosition = m_currentWaypoint.position + offset;
            m_timeAtCurrentGoal = 0f;

            if (showDebugLogs) Debug.Log($"{gameObject.name}: 다음 웨이포인트 -> {m_currentWaypoint.name}, 목표위치 {m_currentGoalPosition}");
        }

        private void MoveToGoal()
        {
            m_timeAtCurrentGoal += Time.deltaTime;

            Vector3 direction = (m_currentGoalPosition - transform.position);
            direction.y = 0;

            if (direction.magnitude > 0.1f)
            {
                m_velocity = direction.normalized * m_moveSpeed;
                transform.position += m_velocity * Time.deltaTime;

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * m_rotationSpeed);
            }
            else
            {
                m_velocity = Vector3.zero;
            }
        }

        private void CheckArrival()
        {
            float distance = Vector3.Distance(transform.position, m_currentGoalPosition);

            if (distance < m_arrivalDistance || m_timeAtCurrentGoal > m_timeoutDuration)
            {
                bool isTimeout = m_timeAtCurrentGoal > m_timeoutDuration;
                if (isTimeout && showDebugLogs)
                {
                    Debug.LogWarning($"{gameObject.name}: 타임아웃으로 다음 지점으로 이동");
                }

                m_currentWaypoint = null;
            }
        }

        private void UpdateAnimation()
        {
            string targetAnim = (m_velocity.magnitude > 0.1f) ? "Walk" : "Idle";

            if (m_animator != null && m_currentAnim != targetAnim)
            {
                m_animator.CrossFade(targetAnim, 0.1f);
                m_currentAnim = targetAnim;
            }
        }

        private void PlayIdle()
        {
            m_velocity = Vector3.zero;
            if (m_animator != null && m_currentAnim != "Idle")
            {
                m_animator.CrossFade("Idle", 0.1f);
                m_currentAnim = "Idle";
            }
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            if (m_finalTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(m_finalTarget.position, 0.3f);
                Gizmos.DrawLine(transform.position, m_finalTarget.position);
            }

            if (m_currentWaypoint != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(m_currentWaypoint.position, 0.2f);

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(m_currentGoalPosition, 0.1f);
                Gizmos.DrawLine(transform.position, m_currentGoalPosition);
            }

            if (m_pathQueue?.Count > 0)
            {
                Gizmos.color = Color.cyan;
                Vector3 lastPos = m_currentWaypoint?.position ?? transform.position;

                foreach (Transform waypoint in m_pathQueue)
                {
                    Gizmos.DrawLine(lastPos, waypoint.position);
                    Gizmos.DrawWireSphere(waypoint.position, 0.15f);
                    lastPos = waypoint.position;
                }
            }
        }
    }
}
