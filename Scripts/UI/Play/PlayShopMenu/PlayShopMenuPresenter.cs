using Oculus.Interaction;
using UnityEngine;

namespace Jobworld
{
    public class PlayShopMenuPresenter : MonoBehaviour
    {
        [SerializeField] private PlayShopMenuView m_view;
        [SerializeField, Interface(typeof(ILevelingModel))] private Object m_modelObject;
        
        private ILevelingModel m_model;
        
        private void Awake()
        {
            m_model = m_modelObject as ILevelingModel;
            
            if (m_model != null && m_view != null)
            {
                m_view.Initialize(m_model);
                SubscribeModelEvents();
            }
        }
        
        private void OnEnable()
        {
            if (m_view != null)
            {
                m_view.RefreshShopElements();
            }
        }
        
        private void SubscribeModelEvents()
        {
            if (m_model != null)
            {
                m_model.levelUpdated += OnLevelUpdated;
                m_model.levelingObjectsChanged += OnLevelingObjectsChanged;
            }
        }
        
        private void OnDisable()
        {
            if (m_model != null)
            {
                m_model.levelUpdated -= OnLevelUpdated;
                m_model.levelingObjectsChanged -= OnLevelingObjectsChanged;
            }
        }
        
        private void OnLevelUpdated(string id)
        {
            if (m_view != null)
            {
                // 레벨 업데이트 시에도 전체 UI 갱신
                m_view.RefreshShopElements();
            }
        }
        
        private void OnLevelingObjectsChanged()
        {
            if (m_view != null)
            {
                m_view.RefreshShopElements();
            }
        }
    }
}