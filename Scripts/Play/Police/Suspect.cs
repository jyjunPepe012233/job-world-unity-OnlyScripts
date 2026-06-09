using UnityEngine;

namespace Jobworld
{
    public class Suspect : MonoBehaviour
    {
        [SerializeField] private Animator animator;
        
        public PoliceTutorialItem policeTutorialItem;
        private static readonly int Walk = Animator.StringToHash("Walk");

        // 테이저건에 맞았을 때 호출
        public void OnTased()
        {
            Debug.Log($"{gameObject.name}이(가) 테이저건에 맞음");
            if (animator != null)
            {
                animator.CrossFade("Tased", 0.1f);
                NotifyTutorialCondition();
            }
        }

        // 수갑에 채워졌을 때 호출
        public void OnHandcuffed()
        {
            if (animator != null)
            {
                Debug.Log("44");
                animator.CrossFade("Handcuffed", 0.1f);
                NotifyTutorialCondition();
            }
        }

        // 튜토리얼 조건 충족 알림
        private void NotifyTutorialCondition()
        {
            if (policeTutorialItem != null)
            {
                policeTutorialItem.SetConditionMet(true);
            }
        }
    }
}
