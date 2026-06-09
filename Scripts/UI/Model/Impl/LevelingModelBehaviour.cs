using System;
using UnityEngine;

namespace Jobworld
{
    public class LevelingModelBehaviour : MonoBehaviour, ILevelingModel
    {
        [SerializeField] private PlayerLevelingManager m_playerLevelingManager;
        
        public event Action<string> levelUpdated;
        public event Action levelingObjectsChanged;
        public ILevelingObject[] levelingObjects => m_playerLevelingManager?.levelingObjects ?? new ILevelingObject[0];
        
        private void Awake()
        {
            if (m_playerLevelingManager == null)
            {
                m_playerLevelingManager = FindObjectOfType<PlayerLevelingManager>();
            }
        }
        
        private void OnEnable()
        {
            if (m_playerLevelingManager != null)
            {
                m_playerLevelingManager.levelUpdated += OnLevelUpdated;
                m_playerLevelingManager.levelingObjectsChanged += OnLevelingObjectsChanged;
            }
        }
        
        private void OnDisable()
        {
            if (m_playerLevelingManager != null)
            {
                m_playerLevelingManager.levelUpdated -= OnLevelUpdated;
                m_playerLevelingManager.levelingObjectsChanged -= OnLevelingObjectsChanged;
            }
        }
        
        private void OnLevelUpdated(string id)
        {
            levelUpdated?.Invoke(id);
        }
        
        private void OnLevelingObjectsChanged()
        {
            levelingObjectsChanged?.Invoke();
        }
    }
}