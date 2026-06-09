// 웨이포인트 매니저 클래스

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaypointManager : MonoBehaviour
{
    [Header("웨이포인트 설정")]
    [SerializeField] public List<Transform> waypoints = new List<Transform>();
    [SerializeField] public float maxConnectionDistance = 10f; // 웨이포인트 간 최대 연결 거리
        
    private void OnDrawGizmos()
    {
        if (waypoints == null) return;
            
        // 모든 웨이포인트 표시
        Gizmos.color = Color.white;
        foreach (Transform waypoint in waypoints)
        {
            if (waypoint != null)
            {
                Gizmos.DrawWireSphere(waypoint.position, 0.3f);
            }
        }
            
        // 연결 가능한 웨이포인트들 간의 연결선 표시
        Gizmos.color = Color.gray;
        for (int i = 0; i < waypoints.Count; i++)
        {
            if (waypoints[i] == null) continue;
                
            for (int j = i + 1; j < waypoints.Count; j++)
            {
                if (waypoints[j] == null) continue;
                    
                float distance = Vector3.Distance(waypoints[i].position, waypoints[j].position);
                if (distance <= maxConnectionDistance)
                {
                    Gizmos.DrawLine(waypoints[i].position, waypoints[j].position);
                }
            }
        }
    }
}