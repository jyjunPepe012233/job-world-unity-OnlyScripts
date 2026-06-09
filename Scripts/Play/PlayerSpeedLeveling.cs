using UnityEngine;

namespace Jobworld
{
    public class PlayerSpeedLeveling : MonoBehaviour, ILevelingObject
    {
        [Header("Leveling Settings")]
        [SerializeField] private LevelingObjectSetting m_levelingSetting;
        [SerializeField] private int m_currentLevel = 1;
        
        [Header("Player References")]
        [SerializeField] private PlayerAttributeController m_playerAttributeController;
        
        private PlayerLevelingManager m_levelingManager;
        
        // ILevelingObject 구현
        public LevelingObjectSetting levelingSetting => m_levelingSetting;
        public int currentLevel => m_currentLevel;
        
        private void Start()
        {
            // PlayerAttributeController 자동 찾기
            if (m_playerAttributeController == null)
            {
                m_playerAttributeController = FindObjectOfType<PlayerAttributeController>();
            }
            
            // PlayerLevelingManager에 등록
            m_levelingManager = FindObjectOfType<PlayerLevelingManager>();
            if (m_levelingManager != null)
            {
                m_levelingManager.AddLevelingObject(this);
                Debug.Log("PlayerSpeedLeveling이 PlayerLevelingManager에 등록되었습니다.");
            }
            
            // 초기 속도 효과 적용
            ApplySpeedEffects();
        }
        
        private void OnDestroy()
        {
            // PlayerLevelingManager에서 제거
            if (m_levelingManager != null)
            {
                m_levelingManager.RemoveLevelingObject(this);
            }
        }
        
        // ILevelingObject 메서드 구현
        public int GetLevel()
        {
            return m_currentLevel;
        }
        
        public void SetLevel(int level)
        {
            if (level >= 1 && m_levelingSetting != null && level <= m_levelingSetting.MaxLevel)
            {
                m_currentLevel = level;
                ApplySpeedEffects();
                Debug.Log($"플레이어 속도 레벨 {m_currentLevel}로 업그레이드!");
            }
        }
        
        private void ApplySpeedEffects()
        {
            if (m_playerAttributeController == null) return;
            
            // 레벨에 따른 속도 증가 (레벨당 10% 증가)
            float speedMultiplier = 1f + (m_currentLevel - 1) * 0.1f;
            m_playerAttributeController.SetGlobalSpeedMultiplier(speedMultiplier);
        }
        
        // 외부에서 현재 속도 증가율을 확인할 수 있는 메서드
        public float GetSpeedMultiplier()
        {
            return 1f + (m_currentLevel - 1) * 0.1f;
        }
        
        // 현재 글로벌 속도 배율을 확인할 수 있는 메서드
        public float GetCurrentGlobalSpeedMultiplier()
        {
            return m_playerAttributeController?.GetGlobalSpeedMultiplier() ?? 1f;
        }
    }
}