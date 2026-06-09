using UnityEngine;
using System;

namespace Jobworld
{
    public class FireGroup : MonoBehaviour
    {
        public event Action OnAllFiresExtinguished;

        [SerializeField] private Fire[] fires;

        private bool _completed = false;

        private void Start()
        {
            // 인스펙터에 안 넣었으면 자동으로 찾기
            if (fires == null || fires.Length == 0)
                fires = GetComponentsInChildren<Fire>();
        }

        private void Update()
        {
            if (_completed) return;

            if (AllFiresExtinguished())
            {
                _completed = true;
                Debug.Log("A fire group has been fully extinguished.");
                OnAllFiresExtinguished?.Invoke();
            }
        }

        private bool AllFiresExtinguished()
        {
            foreach (var fire in fires)
            {
                if (fire != null && !fire.IsExtinguished)
                    return false;
            }
            return true;
        }
    }
}