using System.Collections.Generic;
using UnityEngine;

namespace Jobworld
{
    public class PlayShopMenuView : MonoBehaviour
    {
        [SerializeField] private Transform m_shopElementContainer;
        [SerializeField] private GameObject m_shopElementPrefab;
        
        private List<ShopElement> m_shopElements = new List<ShopElement>();
        private ILevelingModel m_levelingModel;
        
        public void Initialize(ILevelingModel levelingModel)
        {
            m_levelingModel = levelingModel;
            RefreshShopElements();
        }
        
        public void RefreshShopElements()
        {
            ClearShopElements();
            
            if (m_levelingModel?.levelingObjects != null)
            {
                foreach (var levelingObject in m_levelingModel.levelingObjects)
                {
                    if (levelingObject != null)
                    {
                        CreateShopElement(levelingObject);
                    }
                }
            }
        }
        
        private void CreateShopElement(ILevelingObject levelingObject)
        {
            if (m_shopElementPrefab == null || m_shopElementContainer == null || levelingObject == null) return;
            
            GameObject elementObj = Instantiate(m_shopElementPrefab, m_shopElementContainer);
            ShopElement shopElement = elementObj.GetComponent<ShopElement>();
            
            if (shopElement != null)
            {
                shopElement.Initialize(m_levelingModel, levelingObject);
                m_shopElements.Add(shopElement);
            }
            else
            {
                // ShopElement 컴포넌트가 없으면 생성된 오브젝트 제거
                DestroyImmediate(elementObj);
            }
        }
        
        private void ClearShopElements()
        {
            foreach (var shopElement in m_shopElements)
            {
                if (shopElement != null)
                {
                    DestroyImmediate(shopElement.gameObject);
                }
            }
            m_shopElements.Clear();
        }
        
        private void OnDestroy()
        {
            ClearShopElements();
        }
    }
}