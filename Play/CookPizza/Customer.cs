using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

namespace Jobworld
{
    public class Customer : MonoBehaviour
    {
        private readonly List<string> m_order = new List<string>();

        private Dictionary<string, string> m_toppingTranslations = new Dictionary<string, string>
        {
            { "Pepperoni", "페페로니" },
            { "Pepper", "피망" },
            { "Olive", "올리브" },
            { "Pineapple", "파인애플" }
        };

        [SerializeField] private TMP_Text orderUIText;
        
        private Bell ownerBell;

        public void SetBell(Bell bell)
        {
            ownerBell = bell;
        }

        public void ConfirmOrder(bool isSuccess)
        {
            if (isSuccess)
            {
                if (orderUIText != null) orderUIText.text = "주문 성공!";
            }
            else
            {
                if (orderUIText != null) orderUIText.text = "주문 실패";
            }
            
            StartCoroutine(RemoveCustomerAfterDelay(5));
        }

        private IEnumerator RemoveCustomerAfterDelay(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            ownerBell?.RemoveCustomer();
        }

        public void GenerateOrder()
        {
            // 랜덤으로 피자 주문 생성 (리스트에는 비교용으로 전체 구성 요소 포함)
            m_order.Clear();
            m_order.Add("Dough");
            m_order.Add("Sauce");
            m_order.Add("Cheese");
            if (Random.value > 0.5f) m_order.Add("Pepperoni");
            if (Random.value > 0.5f) m_order.Add("Pepper");
            if (Random.value > 0.5f) m_order.Add("Olive");
            if (Random.value > 0.5f) m_order.Add("Pineapple");

            Debug.Log("Customer order (full list for comparison): " + string.Join(", ", m_order));
        }
        
        public bool CheckOrder(List<string> pizzaToppings, bool isBaked)
        {
            if (!isBaked)
            {
                return false;
            }

            // 주문과 피자 토핑 비교
            HashSet<string> toppingSet = new HashSet<string>(pizzaToppings);
            foreach (var topping in m_order)
            {
                if (!toppingSet.Contains(topping))
                {
                    return false;
                }
            }
            return true;
        }

        public string GetToppingsInKorean()
        {
            List<string> translatedToppings = new List<string>();
            foreach (var topping in m_order)
            {
                if (topping == "Dough" || topping == "Sauce" || topping == "Cheese")
                {
                    continue;
                }

                if (m_toppingTranslations.TryGetValue(topping, out string korean))
                {
                    translatedToppings.Add(korean);
                }
                else
                {
                    translatedToppings.Add(topping);
                }
            }

            if (translatedToppings.Count == 0)
            {
                return "토핑은 없이 기본 피자로 주세요.";
            }

            return "토핑은 " + string.Join(", ", translatedToppings) + "(으)로 주세요.";
        }

        public void UpdateOrderUI()
        {
            if (orderUIText != null)
            {
                orderUIText.text = GetToppingsInKorean();
            }
        }

        public void ClearOrderUI()
        {
            if (orderUIText != null)
            {
                orderUIText.text = "";
            }
        }
    }
}
