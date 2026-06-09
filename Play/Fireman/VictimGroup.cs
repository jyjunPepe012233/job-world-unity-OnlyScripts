
using UnityEngine;
using System.Collections.Generic;
using System;

namespace Jobworld
{
    public class VictimGroup : MonoBehaviour
    {
        public event Action OnAllVictimsRescued;

        [SerializeField] private List<Victim> _victims;
        private bool _completed = false;

        private void Start()
        {
            // 하위 오브젝트에서 Victim들을 찾아 리스트에 추가하고, 각 Victim에 이 그룹을 알려줍니다.
            if (_victims == null || _victims.Count == 0)
            {
                _victims = new List<Victim>(GetComponentsInChildren<Victim>());
            }

            foreach (var victim in _victims)
            {
                victim.SetGroup(this);
            }
        }

        // Victim이 구조되었을 때 호출될 메서드
        public void NotifyVictimRescued(Victim victim)
        {
            if (_completed) return;

            if (_victims.Contains(victim))
            {
                _victims.Remove(victim);
            }

            // 모든 Victim이 구조되었는지 확인
            if (_victims.Count == 0)
            {
                _completed = true;
                Debug.Log("A victim group has been fully rescued.");
                
                // 마지막 피해자의 애니메이션과 파티클을 위한 딜레이
                StartCoroutine(DelayedGroupCompletion());
            }
        }

        private System.Collections.IEnumerator DelayedGroupCompletion()
        {
            // 마지막 피해자의 애니메이션과 파티클이 재생될 시간을 기다림
            yield return new WaitForSeconds(1.0f);
            
            Debug.Log("VictimGroup 완료 이벤트 발생 (딜레이 후)");
            OnAllVictimsRescued?.Invoke();
        }
    }
}
