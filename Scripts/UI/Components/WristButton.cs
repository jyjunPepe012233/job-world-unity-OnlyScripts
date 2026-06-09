using UnityEngine;

/// <summary>
/// 버튼을 누르면 WristCanvas에 패널 프리팹을 띄우는 클래스
/// </summary>
public class WristButton : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab;

    private WristUI wristUI;

    public void SetWristUI(WristUI ui)
    {
        wristUI = ui;
    }

    public void OnClick()
    {
        if (wristUI == null)
        {
            Debug.LogError("WristUI가 설정되지 않았습니다!");
            return;
        }

        if (panelPrefab == null)
        {
            Debug.LogWarning("패널 프리팹이 설정되지 않았습니다!");
            return;
        }

        wristUI.ShowPanel(panelPrefab, this);
    }
}