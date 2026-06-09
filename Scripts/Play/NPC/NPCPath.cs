using UnityEngine;

namespace Jobworld
{
    public class NPCPath : MonoBehaviour
    {
        [Header("NPC 이동 경로 타겟")]
        public Transform[] targets;

        private void OnDrawGizmos()
        {
            if (targets == null || targets.Length < 2) return;

            // 기본 경로를 그립니다.
            Gizmos.color = Color.yellow;
            for (int i = 0; i < targets.Length - 1; i++)
            {
                if (targets[i] != null && targets[i + 1] != null)
                {
                    Gizmos.DrawLine(targets[i].position, targets[i + 1].position);
                    Gizmos.DrawSphere(targets[i].position, 0.1f);
                }
            }

            if (targets[targets.Length - 1] != null)
                Gizmos.DrawSphere(targets[targets.Length - 1].position, 0.1f);
            
            if (targets[targets.Length - 1] != null && targets[0] != null)
            {
                Gizmos.color = Color.gray;
                Gizmos.DrawLine(targets[targets.Length - 1].position, targets[0].position);
            }
        }

        public Transform GetTarget(int index)
        {
            if (targets == null || targets.Length == 0) return null;
            return targets[Mathf.Clamp(index, 0, targets.Length - 1)];
        }

        public int Length => targets != null ? targets.Length : 0;
    }
}