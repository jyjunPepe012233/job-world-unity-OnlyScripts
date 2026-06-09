using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Jobworld
{
    public class ShopElement : MonoBehaviour
    {
        [SerializeField] private Image m_iconImage;
        [SerializeField] private TextMeshProUGUI m_nameLevelText;      // "속도 Lv. 2/5"
        [SerializeField] private TextMeshProUGUI m_currentEffectText;  // "속도 10% 증가"
        [SerializeField] private GameObject m_arrowObject;             // 화살표 오브젝트
        [SerializeField] private TextMeshProUGUI m_nextEffectText;     // "속도 20% 증가"
        [SerializeField] private TextMeshProUGUI m_costText;           // "$ 200"
        [SerializeField] private Button m_levelUpButton;
        
        private ILevelingModel m_levelingModel;
        private ILevelingObject m_targetLevelingObject;
        private PlayerLevelingManager m_levelingManager;
        
        public void Initialize(ILevelingModel levelingModel, ILevelingObject targetLevelingObject)
        {
            m_levelingModel = levelingModel;
            m_targetLevelingObject = targetLevelingObject;
            m_levelingManager = FindObjectOfType<PlayerLevelingManager>();
            
            UpdateDisplay();
            
            if (m_levelUpButton != null)
            {
                m_levelUpButton.onClick.AddListener(OnLevelUpButtonClicked);
            }
        }
        
        public void UpdateDisplay()
        {
            if (m_targetLevelingObject == null) return;
            
            var setting = m_targetLevelingObject.levelingSetting;
            if (setting == null) return;
            
            // 아이콘 업데이트
            if (m_iconImage != null)
            {
                if (setting.Icon != null)
                {
                    m_iconImage.sprite = setting.Icon;
                    m_iconImage.preserveAspect = true; // 원본 비율 유지
                }
                m_iconImage.gameObject.SetActive(setting.Icon != null);
            }
            
            // 이름 + 레벨 통합 텍스트 업데이트
            if (m_nameLevelText != null)
            {
                m_nameLevelText.text = $"{setting.Name} Lv. {m_targetLevelingObject.currentLevel}/{setting.MaxLevel}";
            }
            
            // 레벨업 가능 여부 체크
            bool canLevelUp = m_targetLevelingObject.currentLevel < setting.MaxLevel;
            
            // 현재 레벨 효과 설명
            if (m_currentEffectText != null)
            {
                int currentLevelIndex = m_targetLevelingObject.currentLevel - 1;
                if (setting.LevelEffectDescriptions != null && currentLevelIndex >= 0 && 
                    currentLevelIndex < setting.LevelEffectDescriptions.Length &&
                    !string.IsNullOrEmpty(setting.LevelEffectDescriptions[currentLevelIndex]))
                {
                    m_currentEffectText.text = setting.LevelEffectDescriptions[currentLevelIndex];
                    m_currentEffectText.gameObject.SetActive(true);
                }
                else
                {
                    m_currentEffectText.gameObject.SetActive(false);
                }
            }
            
            // 다음 레벨 효과 설명 및 화살표
            if (canLevelUp)
            {
                // 화살표 표시
                if (m_arrowObject != null)
                    m_arrowObject.SetActive(true);
                
                // 다음 효과 표시
                if (m_nextEffectText != null)
                {
                    int nextLevelIndex = m_targetLevelingObject.currentLevel;
                    if (setting.LevelEffectDescriptions != null && nextLevelIndex >= 0 && 
                        nextLevelIndex < setting.LevelEffectDescriptions.Length &&
                        !string.IsNullOrEmpty(setting.LevelEffectDescriptions[nextLevelIndex]))
                    {
                        m_nextEffectText.text = setting.LevelEffectDescriptions[nextLevelIndex];
                        m_nextEffectText.gameObject.SetActive(true);
                    }
                    else
                    {
                        m_nextEffectText.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                // 최대 레벨일 때는 화살표와 다음 효과 숨김
                if (m_arrowObject != null)
                    m_arrowObject.SetActive(false);
                
                if (m_nextEffectText != null)
                    m_nextEffectText.gameObject.SetActive(false);
            }
            
            // 레벨업 비용 표시
            if (m_costText != null)
            {
                if (canLevelUp)
                {
                    int costIndex = m_targetLevelingObject.currentLevel - 1;
                    if (setting.LevelUpCosts != null && costIndex >= 0 && 
                        costIndex < setting.LevelUpCosts.Length)
                    {
                        int cost = setting.LevelUpCosts[costIndex];
                        m_costText.text = $"$ {cost}";
                        m_costText.gameObject.SetActive(true);
                        
                        // 달러가 부족하면 빨간색, 충분하면 흰색
                        if (HasEnoughDollars(cost))
                        {
                            m_costText.color = Color.white;
                        }
                        else
                        {
                            m_costText.color = Color.red;
                        }
                    }
                    else
                    {
                        m_costText.gameObject.SetActive(false);
                    }
                }
                else
                {
                    // 최대 레벨일 때는 비용 숨김
                    m_costText.gameObject.SetActive(false);
                }
            }
            
            // 레벨업 버튼 상태 업데이트
            if (m_levelUpButton != null)
            {
                canLevelUp = m_targetLevelingObject.currentLevel < setting.MaxLevel;
                m_levelUpButton.interactable = canLevelUp;
                
                // 버튼 텍스트도 업데이트
                var buttonText = m_levelUpButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = canLevelUp ? "레벨 업" : "최대 레벨";
                }
            }
        }
        
        private void OnLevelUpButtonClicked()
        {
            if (m_targetLevelingObject?.levelingSetting == null || m_levelingManager == null) return;
            
            int nextLevel = m_targetLevelingObject.currentLevel + 1;
            if (nextLevel <= m_targetLevelingObject.levelingSetting.MaxLevel)
            {
                // 레벨업 비용 확인
                int costIndex = m_targetLevelingObject.currentLevel - 1;
                var setting = m_targetLevelingObject.levelingSetting;
                
                if (setting.LevelUpCosts != null && costIndex >= 0 && costIndex < setting.LevelUpCosts.Length)
                {
                    int cost = setting.LevelUpCosts[costIndex];
                    
                    // 현재 달러 확인
                    if (HasEnoughDollars(cost))
                    {
                        // 달러 차감
                        DeductDollars(cost);
                        
                        // PlayerLevelingManager를 통해 레벨업 (이벤트 발생)
                        m_levelingManager.SetLevel(m_targetLevelingObject.levelingSetting.Id, nextLevel);
                        
                        Debug.Log($"{setting.Name} 레벨업 완료! {cost}달러 차감됨");
                    }
                    else
                    {
                        Debug.Log($"달러가 부족합니다! 필요: {cost}달러");
                        ShowInsufficientFundsEffect();
                    }
                }
            }
        }
        
        private bool HasEnoughDollars(int requiredAmount)
        {
            if (GuestClientLocalInfoHolder.playData != null)
            {
                return GuestClientLocalInfoHolder.playData.dollar >= requiredAmount;
            }
            return false;
        }
        
        private async void DeductDollars(int amount)
        {
            if (GuestClientLocalInfoHolder.playData != null)
            {
                int currentDollars = GuestClientLocalInfoHolder.playData.dollar;
                int newAmount = currentDollars - amount;
                
                // GuestClientService를 통해 달러 설정 (네트워크 동기화)
                await GuestClientService.SetDollar(newAmount);
                
                Debug.Log($"달러 차감 완료: {currentDollars} → {newAmount} (-{amount})");
            }
        }
        
        private void ShowInsufficientFundsEffect()
        {
            // 버튼 흔들기 효과나 빨간색 깜빡임 등
            if (m_levelUpButton != null)
            {
                StartCoroutine(ButtonShakeEffect());
            }
        }
        
        private System.Collections.IEnumerator ButtonShakeEffect()
        {
            Vector3 originalPosition = m_levelUpButton.transform.localPosition;
            float shakeIntensity = 5f;
            float shakeDuration = 0.3f;
            float elapsed = 0f;
            
            while (elapsed < shakeDuration)
            {
                float x = originalPosition.x + Random.Range(-shakeIntensity, shakeIntensity);
                float y = originalPosition.y + Random.Range(-shakeIntensity, shakeIntensity);
                m_levelUpButton.transform.localPosition = new Vector3(x, y, originalPosition.z);
                
                elapsed += Time.deltaTime;
                yield return null;
            }
            
            m_levelUpButton.transform.localPosition = originalPosition;
        }
        
        private void OnDestroy()
        {
            if (m_levelUpButton != null)
            {
                m_levelUpButton.onClick.RemoveListener(OnLevelUpButtonClicked);
            }
        }
    }
}