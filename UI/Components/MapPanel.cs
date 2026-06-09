using UnityEngine;

namespace Jobworld
{
    /// <summary>
    /// 미니맵 패널 - Instantiate될 때 MinimapSystem에 UI 등록
    /// </summary>
    public class MapPanel : MonoBehaviour
    {
        [Header("UI References")]
        [SerializeField] private RectTransform playerIcon;
        [SerializeField] private RectTransform destinationIcon;
        [SerializeField] private RectTransform minimapBounds;
        
        private MinimapSystem minimapSystem;
        
        private void Start()
        {
            // MinimapSystem 찾기 (플레이어 자식 카메라에 있음)
            minimapSystem = FindObjectOfType<MinimapSystem>();
            
            if (minimapSystem != null)
            {
                // UI 요소 등록
                minimapSystem.RegisterUI(playerIcon, destinationIcon, minimapBounds);
            }
            else
            {
                Debug.LogError("MinimapSystem을 찾을 수 없습니다!");
            }
        }
        
        private void OnDestroy()
        {
            // 패널이 파괴될 때 등록 해제
            if (minimapSystem != null)
            {
                minimapSystem.UnregisterUI();
            }
        }
    }
}