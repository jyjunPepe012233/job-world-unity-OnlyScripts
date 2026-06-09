using System;
using Oculus.Interaction;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

namespace Jobworld
{
    public class Bell : MonoBehaviour
    {
        public event Action<GameObject> CustomerSpawned;

        [SerializeField] private PizzaManager pizzaManager;
        [SerializeField] private PokeInteractable bellPokeInteractable; // 포크 인터랙터블 추가
        [SerializeField] private Collider pizzaSubmissionCollider; // 피자 제출 콜라이더
        [SerializeField] private List<GameObject> customerPrefabs; // 손님 프리팹 리스트
        [SerializeField] private Transform customerSpawnPoint; // 손님 스폰 위치
        [SerializeField] private ParticleSystem bellParticle;
        [SerializeField] private ParticleSystem customerSpawnParticle;
        [SerializeField] public ParticleSystem pointArrowParticle;

        private GameObject m_currentCustomer; // 현재 활성화된 손님
        private bool m_hasSpawnedOnce = false;


        private void Start()
        {
            if (bellPokeInteractable != null)
            {
                bellPokeInteractable.WhenPointerEventRaised += OnBellPoked;
            }
        }

        private void OnDestroy()
        {
            if (bellPokeInteractable != null)
            {
                bellPokeInteractable.WhenPointerEventRaised -= OnBellPoked;
            }
        }

        public void PlayParticleAt(ParticleSystem prefab, Vector3 position, Transform parent = null)
        {
            if (prefab == null) return;
            var instance = Instantiate(prefab, position, Quaternion.identity, parent);
            instance.Play();

            var main = instance.main;
            float life = main.duration;
            var startLifetime = main.startLifetime;
            switch (startLifetime.mode)
            {
                case ParticleSystemCurveMode.Constant:
                    life += startLifetime.constant;
                    break;
                case ParticleSystemCurveMode.TwoConstants:
                    life += startLifetime.constantMax;
                    break;
                default:
                    life += startLifetime.constantMax;
                    break;
            }

            Destroy(instance.gameObject, life + 0.1f);
        }

        private void SpawnCustomer()
        {
            if (customerPrefabs != null && customerPrefabs.Count > 0 && customerSpawnPoint != null)
            {
                // 랜덤으로 손님 프리팹 선택 및 스폰
                int randomIndex = Random.Range(0, customerPrefabs.Count);
                m_currentCustomer = Instantiate(customerPrefabs[randomIndex], customerSpawnPoint.position, customerSpawnPoint.rotation);

                // Fire one-time CustomerSpawned event
                if (!m_hasSpawnedOnce)
                {
                    CustomerSpawned?.Invoke(m_currentCustomer);
                    m_hasSpawnedOnce = true;
                }

                if (customerSpawnParticle != null)
                {
                    PlayParticleAt(customerSpawnParticle, m_currentCustomer.transform.position);
                }

                // 손님 주문 생성
                var customerComponent = m_currentCustomer.GetComponent<Customer>();
                if (customerComponent != null)
                {
                    customerComponent.GenerateOrder();
                    customerComponent.UpdateOrderUI();
                    // Bell 레퍼런스 전달
                    customerComponent.SetBell(this);
                }
            }
        }
        
        public void RemoveCustomer()
        {
            if (m_currentCustomer == null) return;

            // 캐시 위치 및 컴포넌트
            var customerComponent = m_currentCustomer.GetComponent<Customer>();

            // 주문 UI 초기화
            if (customerComponent != null)
            {
                customerComponent.ClearOrderUI();
            }
            
            // 손님 파괴
            Destroy(m_currentCustomer);
            m_currentCustomer = null;
        }

        private void OnBellPoked(PointerEvent evt)
        {
            if (evt.Type == PointerEventType.Select)
            {
                if (bellParticle != null) bellParticle.Play();
                SoundManager.singleton.PlaySFX("CP_S_Bell", transform.position);
                if (m_currentCustomer == null)
                {

                    SpawnCustomer(); // 손님이 없을 때만 스폰
                }
                else if (pizzaManager != null && pizzaSubmissionCollider != null)
                {
                    // 피자가 피자 제출 장소에 있는지 확인
                    if (pizzaSubmissionCollider.bounds.Contains(pizzaManager.transform.position))
                    {
                        pizzaManager.HandleBellSignal(m_currentCustomer.GetComponent<Customer>()); // 주문 확인
                    }
                    else
                    {
                        Debug.Log("No pizza in the placement area.");
                    }
                }
                
                if (pointArrowParticle != null && pointArrowParticle.isPlaying)
                {
                    pointArrowParticle.Stop();
                }
            }
        }

        private void Update()
        {
            if (pizzaManager != null && pizzaSubmissionCollider != null)
            {
                if (pizzaSubmissionCollider.bounds.Contains(pizzaManager.transform.position))
                {
                    if (pizzaManager.IsPizzaBaked())
                    {
                        if (pointArrowParticle != null && !pointArrowParticle.isPlaying)
                        {
                            pointArrowParticle.Play();
                        }
                    }
                }
            }
        }
    }
}
