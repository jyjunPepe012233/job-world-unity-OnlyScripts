using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jobworld
{
    public class PizzaManager : MonoBehaviour
    {
        public event System.Action<bool> OrderConfirmed;
        public event System.Action BakedFromOven;

        [SerializeField] private GameObject ovenObject; // 화덕 오브젝트
        [SerializeField] private GameObject trashBinObject; // 쓰레기통 오브젝트

        [SerializeField] private GameObject pizzaPrefab; // 피자 프리팹
        [SerializeField] private ParticleSystem doughParticle; // 도우 생성 파티클
        [SerializeField] private ParticleSystem fireParticle;

        [SerializeField] private List<GameObject> ingredientPrefabs; // 재료 프리팹 리스트
        [SerializeField] private List<Transform> ingredientSpawnPoints; // 재료 생성 위치 리스트
        
        [Header("Baking Settings")]
        [SerializeField] private Material burnedDoughMaterial; // 탄 도우 머테리얼 (인스펙터에 할당)
        [SerializeField] private float timeToBake = 10f; // 오븐에 넣고 나서 구워질 때까지 시간(초)
        [SerializeField] private float timeToBurnAfterBaked = 10f; // 구워진 후 더 타기까지 시간(초)
        [SerializeField] private List<Material> bakedMaterials; // 각 구성요소별 베이크드 머테리얼 (인스펙터에서 순서대로 할당)

        private GameObject m_currentPizza;
        private Dictionary<string, GameObject> m_pizzaComponents = new Dictionary<string, GameObject>(); // 피자 구성 요소 관리
        private Dictionary<string, Transform> m_ingredientSpawnMap = new Dictionary<string, Transform>(); // 재료와 생성 위치 매핑
        private int m_currentStep; // 현재 피자 제작 단계 0: 도우, 1: 소스, 2: 치즈, 3: 토핑

        private Renderer m_doughRenderer;
        private Coroutine m_bakeCoroutine;

        private Dictionary<Renderer, (Material original, Material baked)> m_rendererMaterialMap = new();

        private enum BakeState { Raw, Baked, Burned }
        private BakeState m_bakeState = BakeState.Raw;
        private float m_timeAccumulated = 0f;

        private Dictionary<int, HashSet<string>> m_validSteps = new Dictionary<int, HashSet<string>>
        {
            { 0, new HashSet<string> { "Dough" } },
            { 1, new HashSet<string> { "Sauce" } },
            { 2, new HashSet<string> { "Cheese" } },
            { 3, new HashSet<string> { "Pepperoni", "Pepper", "Olive", "Pineapple" } }
        };
        
        private void Start()
        {
            SpawnPizza(); // 피자 스폰
            InitializePizzaComponents(); // 피자 구성 요소 초기화
            InitializeIngredientSpawnMap(); // 재료와 생성 위치 매핑 초기화

            SpawnIngredients(); // 초기 재료 스폰
        }

        private void SpawnIngredients()
        {
            if (ingredientPrefabs.Count != ingredientSpawnPoints.Count)
            {
                return;
            }

            for (int i = 0; i < ingredientPrefabs.Count; i++)
            {
                GameObject prefab = ingredientPrefabs[i];
                Transform spawnPoint = ingredientSpawnPoints[i];

                if (prefab != null && spawnPoint != null)
                {
                    Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
                }
            }
        }

        private void InitializePizzaComponents()
        {
            if (m_currentPizza == null)
            {
                return;
            }
            m_rendererMaterialMap.Clear();
            int bakedIdx = 0;
            // 피자 프리팹의 자식 객체를 순회하며 비활성화하고 딕셔너리에 저장
            foreach (Transform child in m_currentPizza.transform)
            {
                child.gameObject.SetActive(false);
                m_pizzaComponents[child.name] = child.gameObject;
                var r = child.GetComponent<Renderer>();
                if (r != null)
                {
                    Material original = r.sharedMaterial;
                    Material baked = (bakedIdx < bakedMaterials.Count) ? bakedMaterials[bakedIdx] : null;
                    m_rendererMaterialMap[r] = (original, baked);
                    // 도우라면 m_doughRenderer에 저장
                    if (child.name == "Dough")
                    {
                        m_doughRenderer = r;
                    }
                    bakedIdx++;
                }
            }
        }

        private void InitializeIngredientSpawnMap()
        {
            // 재료 프리팹과 생성 위치의 개수가 일치하지 않으면 오류 출력
            if (ingredientPrefabs.Count != ingredientSpawnPoints.Count)
            {
                return;
            }

            // 재료 이름과 생성 위치를 매핑하여 딕셔너리에 저장
            for (int i = 0; i < ingredientPrefabs.Count; i++)
            {
                string ingredientName = ingredientPrefabs[i].name;
                m_ingredientSpawnMap[ingredientName] = ingredientSpawnPoints[i];
            }
        }

        public void AddComponent(string componentName, GameObject grabbedObject)
        {
            if (componentName.EndsWith("(Clone)"))
            {
                componentName = componentName.Replace("(Clone)", "").Trim();
            }

            // 현재 피자가 없으면 오류 출력
            if (m_currentPizza == null)
            {
                return;
            }

            // 추가하려는 구성 요소가 유효하지 않으면 오류 출력
            if (!m_pizzaComponents.ContainsKey(componentName))
            {
                return;
            }

            // 현재 단계에서 추가 가능한 구성 요소인지 확인
            if (!IsValidStep(componentName))
            {
                return;
            }
            
            // 이미 구워지거나 탄 상태이면 더 이상 진행하지 않음
            if (m_bakeState == BakeState.Baked || m_bakeState == BakeState.Burned)
            {
                return;
            }

            ActivatePizzaComponent(componentName); // 구성 요소 활성화
            Destroy(grabbedObject); // 잡고 있던 객체 삭제
            RespawnIngredient(componentName); // 재료 재생성
        }

        private bool IsValidStep(string componentName)
        {
            return m_validSteps.TryGetValue(m_currentStep, out var validComponents) && validComponents.Contains(componentName);
        }

        private void ActivatePizzaComponent(string componentName)
        {
            // 구성 요소를 활성화하고 제작 단계를 증가
            if (m_pizzaComponents.TryGetValue(componentName, out var component))
            {
                if (component.activeSelf)
                {
                    return;
                }
                if (componentName == "Dough" && doughParticle != null)
                {
                    doughParticle.Play();
                }
                SoundManager.singleton.PlaySFX("CP_S_Pop", transform.position);
                component.SetActive(true);
                if (m_currentStep < 3)
                {
                    m_currentStep++;
                }
            }
        }

        public void PlaceInOven()
        {
            if (m_currentStep < 3)
            {
                return;
            }

            if (m_currentPizza == null)
            {
                return;
            }
            
            // 이미 구워지거나 탄 상태이면 더 이상 진행하지 않음
            if (m_bakeState == BakeState.Baked || m_bakeState == BakeState.Burned)
            {
                return;
            }
            
            if (m_bakeCoroutine == null)
            {
                SoundManager.singleton.PlaySFX("CP_S_Fire", transform.position);
                m_bakeCoroutine = StartCoroutine(MonitorOvenPresence());
            }
        }

        private IEnumerator MonitorOvenPresence()
        {
            if (fireParticle != null)
            {
                fireParticle.Play();
            }
            m_timeAccumulated = 0f;

            var ovenCollider = ovenObject.GetComponent<Collider>();
            if (ovenCollider == null)
            {
                yield break;
            }

            var pizzaManagerCollider = GetComponent<Collider>();
            if (pizzaManagerCollider == null)
            {
                yield break;
            }
            
            while (true)
            {
                bool isOverlapping = false;
                Vector3 direction;
                float distance;
                isOverlapping = Physics.ComputePenetration(
                    ovenCollider, ovenCollider.transform.position, ovenCollider.transform.rotation,
                    pizzaManagerCollider, pizzaManagerCollider.transform.position, pizzaManagerCollider.transform.rotation,
                    out direction, out distance);

                if (isOverlapping)
                {
                    // 누적 시간 증가
                    m_timeAccumulated += Time.deltaTime;

                    // 아직 구워지지 않았고 임계값 도달 시 구움 적용
                    if (m_bakeState == BakeState.Raw && m_timeAccumulated >= timeToBake)
                    {
                        ApplyBakedState();
                        m_bakeState = BakeState.Baked;
                    }

                    // 이미 구워진 상태이고 누적 시간이 총(구움+추가) 도달하면 탄 상태
                    if (m_bakeState == BakeState.Baked && m_timeAccumulated >= timeToBake + timeToBurnAfterBaked)
                    {
                        ApplyBurnedState();
                        m_bakeState = BakeState.Burned;
                        m_bakeCoroutine = null;
                        yield break;
                    }
                }
                else
                {
                    if (fireParticle != null)
                    {
                        fireParticle.Stop();
                    }
                    // 오븐에서 빠져나오면 동작: 아직 구워지지 않았다면 누적 리셋하고 코루틴 종료
                    if (m_bakeState == BakeState.Raw)
                    {
                        m_timeAccumulated = 0f;
                        m_bakeCoroutine = null;
                        yield break;
                    }

                    // 구워진 상태에서 빼면 그 상태로 고정하고 종료
                    if (m_bakeState == BakeState.Baked)
                    {
                        // Notify listeners (tutorials) that baked pizza was taken out
                        BakedFromOven?.Invoke();
                        m_bakeCoroutine = null;
                        yield break;
                    }

                    // 탄 상태면 이미 끝났으므로 종료
                    if (m_bakeState == BakeState.Burned)
                    {
                        m_bakeCoroutine = null;
                        yield break;
                    }
                }
                yield return null;
            }
        }


        private void ApplyBakedState()
        {
            foreach (var kv in m_rendererMaterialMap)
            {
                var renderer = kv.Key;
                var baked = kv.Value.baked;
                if (renderer != null && baked != null)
                {
                    renderer.material = baked;
                }
            }
            SoundManager.singleton.PlaySFX("CP_S_Oven", transform.position);
        }

        private void ApplyBurnedState()
        {
            if (m_doughRenderer == null)
            {
                return;
            }
            if (burnedDoughMaterial == null)
            {
                return;
            }
            SoundManager.singleton.PlaySFX("CP_S_Fire", transform.position);
            m_doughRenderer.material = burnedDoughMaterial;

            // 도우 제외한 모든 구성요소 비활성화
            foreach (var kv in m_pizzaComponents)
            {
                if (kv.Key == "Dough") continue;
                if (kv.Value == null) continue;
                kv.Value.SetActive(false);
            }
        }

        public void RespawnIngredient(string ingredientName)
        {
            if (m_ingredientSpawnMap.TryGetValue(ingredientName, out Transform spawnPoint))
            {
                GameObject prefab = ingredientPrefabs.Find(prefab => prefab.name == ingredientName);
                if (prefab != null)
                {
                    Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
                }
            }
        }

        public void ResetPizza()
        {
            foreach (var component in m_pizzaComponents)
            {
                if (component.Value == null) continue;
                component.Value.SetActive(false);
            }
            m_currentStep = 0;
            if (m_bakeCoroutine != null)
            {
                StopCoroutine(m_bakeCoroutine);
                m_bakeCoroutine = null;
            }
            // 모든 구성요소의 머테리얼 원복
            foreach (var kv in m_rendererMaterialMap)
            {
                var renderer = kv.Key;
                var original = kv.Value.original;
                if (renderer != null && original != null)
                {
                    renderer.material = original;
                }
            }
            m_bakeState = BakeState.Raw;
            m_timeAccumulated = 0f;
        }

        private void OnTriggerEnter(Collider other)
        {
            // 트리거가 오브젝트가 재료인지 확인
            if (other.CompareTag("Ingredient"))
            {
                // 재료 이름 가져오기
                string ingredientName = other.gameObject.name;

                // AddComponent 메서드를 호출하여 재료 추가
                AddComponent(ingredientName, other.gameObject);
            }

            // 화덕 콜라이더와 상호작용
            if (ovenObject != null && other == ovenObject.GetComponent<Collider>())
            {
                PlaceInOven();
            }

            // 쓰레기통 콜라이더와 상호작용
            if (trashBinObject != null && other == trashBinObject.GetComponent<Collider>())
            {
                ResetPizza();
            }
        }

        public List<string> GetCurrentPizzaToppings()
        {
            if (m_currentPizza == null)
            {
                return new List<string>();
            }

            List<string> toppings = new List<string>();

            foreach (var component in m_pizzaComponents)
            {
                if (component.Value.activeSelf)
                {
                    toppings.Add(component.Key);
                }
            }

            return toppings;
        }

        public void HandleBellSignal(Customer customer)
        {
            if (customer == null)
            {
                return;
            }

            List<string> pizzaToppings = GetCurrentPizzaToppings();
            if (pizzaToppings == null || pizzaToppings.Count == 0)
            {
                return;
            }

            if (customer.CheckOrder(pizzaToppings, m_bakeState == BakeState.Baked))
            {
                customer.ConfirmOrder(true);
                OrderConfirmed?.Invoke(true);
                ResetPizza();
            }
            else
            {
                customer.ConfirmOrder(false);
                OrderConfirmed?.Invoke(false);
            }
        }

        private void SpawnPizza()
        {
            if (pizzaPrefab == null)
            {
                return;
            }

            if (m_currentPizza != null)
            {
                ResetPizza();
            }

            Vector3 spawnPosition = transform.position + new Vector3(0, 0.02f, 0);
            m_currentPizza = Instantiate(pizzaPrefab, spawnPosition, transform.rotation, transform);
        }

        public GameObject currentPizza => m_currentPizza;
        
        public bool IsPizzaBaked()
        {
            return m_bakeState == BakeState.Baked;
        }
    }
}
