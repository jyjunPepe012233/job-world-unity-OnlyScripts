using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// 손목 UI의 패널과 버튼 활성화 상태를 관리하는 클래스
/// OVR Input의 3줄 버튼으로 토글
/// </summary>
public class WristUI : MonoBehaviour
{
    [SerializeField] private Canvas wristButtonCanvas;
    [SerializeField] private Canvas wristPanelCanvas;
    [SerializeField] private Transform buttonsParent; // 버튼들의 부모 오브젝트
    [SerializeField] private List<WristButton> buttons = new List<WristButton>();
    
    [Header("Animation Settings")]
    [SerializeField] private string showAnimationTrigger = "Show"; // 등장 애니메이션 트리거 이름
    [SerializeField] private string hideAnimationTrigger = "Hide"; // 사라짐 애니메이션 트리거 이름
    [SerializeField] private float hideAnimationDuration = 0.3f; // 사라짐 애니메이션 길이 (초)

    [Header("OVR Input Settings")] 
    [SerializeField] private OVRInput.RawButton toggleButton = OVRInput.RawButton.Start;
    
    [Header("Button Color Settings")]
    [SerializeField] private Color selectedColor = new Color(0.902f, 0.098f, 0.231f); // #E6193B
    
    private GameObject currentPanel;
    private Animator currentAnimator;
    private WristButton currentButton;
    private Coroutine hideCoroutine;
    private bool isButtonCanvasActive = false;
    
    // 버튼의 원래 색상 저장용
    private Dictionary<WristButton, Color> originalButtonColors = new Dictionary<WristButton, Color>();

    private void Start()
    {
        // 모든 버튼에 WristUI 참조 전달 및 원래 색상 저장
        foreach (var button in buttons)
        {
            button.SetWristUI(this);
            
            // 버튼의 Image 컴포넌트에서 원래 색상 저장
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                originalButtonColors[button] = buttonImage.color;
            }
        }
        
        // ButtonsParent 자동으로 찾기 (할당되지 않은 경우)
        if (buttonsParent == null && buttons.Count > 0)
        {
            buttonsParent = buttons[0].transform.parent;
            Debug.Log($"ButtonsParent 자동 찾기 완료: {buttonsParent.name}");
        }
        
        // 초기 상태: 버튼 캔버스 비활성화
        if (buttonsParent != null)
        {
            buttonsParent.gameObject.SetActive(false);
            isButtonCanvasActive = false;
        }
    }
    
    private void Update()
    {
        CheckToggleInput();
    }

    private void OnDestroy()
    {
        // 진행 중인 코루틴 정리
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }

    /// <summary>
    /// OVR Input 버튼 입력 확인 및 토글
    /// </summary>
    private void CheckToggleInput()
    {
        if (OVRInput.GetDown(toggleButton))
        {
            ToggleButtonCanvas();
        }
    }

    /// <summary>
    /// 버튼 캔버스 토글
    /// </summary>
    private void ToggleButtonCanvas()
    {
        if (buttonsParent == null) return;

        isButtonCanvasActive = !isButtonCanvasActive;
        buttonsParent.gameObject.SetActive(isButtonCanvasActive);
        
        Debug.Log($"[WristUI] 버튼 캔버스 토글: {isButtonCanvasActive}");
        
        // 버튼 캔버스가 비활성화되면 열려있는 패널도 닫기
        if (!isButtonCanvasActive && currentPanel != null)
        {
            CloseCurrentPanel();
        }
    }

    /// <summary>
    /// 패널을 WristCanvas에 띄우기
    /// 같은 버튼이면 토글, 다른 버튼이면 교체
    /// </summary>
    public void ShowPanel(GameObject panelPrefab, WristButton button)
    {
        if (panelPrefab == null)
        {
            Debug.LogWarning("패널 프리팹이 null입니다!");
            return;
        }

        // 같은 버튼을 다시 누른 경우 토글
        if (currentButton == button && currentPanel != null)
        {
            CloseCurrentPanel();
            return;
        }

        // 이전 버튼 색상 복원
        if (currentButton != null)
        {
            RestoreButtonColor(currentButton);
        }

        // 진행 중인 숨김 코루틴이 있다면 중지
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // 기존 패널이 있다면 즉시 삭제
        if (currentPanel != null)
        {
            Destroy(currentPanel);
            currentPanel = null;
            currentAnimator = null;
        }

        // 새 패널 생성
        currentPanel = Instantiate(panelPrefab, wristPanelCanvas.transform);
        currentButton = button;
        
        // 새 버튼 색상 변경
        SetButtonColor(currentButton, selectedColor);
        
        // RectTransform 설정
        RectTransform rt = currentPanel.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }

        // Animator 컴포넌트 가져오기
        currentAnimator = currentPanel.GetComponent<Animator>();
        
        if (currentAnimator != null)
        {
            // 등장 애니메이션 재생
            currentAnimator.CrossFade(showAnimationTrigger, 0f);
            Debug.Log($"손목 UI 패널 등장 애니메이션 재생: {panelPrefab.name}");
        }
        else
        {
            Debug.LogWarning($"패널 '{panelPrefab.name}'에 Animator 컴포넌트가 없습니다!");
        }
    }

    /// <summary>
    /// 현재 패널 닫기 (애니메이션 포함)
    /// </summary>
    public void CloseCurrentPanel()
    {
        if (currentPanel != null)
        {
            // 이미 닫는 중이라면 무시
            if (hideCoroutine != null)
            {
                return;
            }

            hideCoroutine = StartCoroutine(CloseWithAnimation());
        }
    }

    /// <summary>
    /// 애니메이션과 함께 패널 닫기
    /// </summary>
    private IEnumerator CloseWithAnimation()
    {
        if (currentAnimator != null && currentPanel != null)
        {
            // 사라짐 애니메이션 재생
            currentAnimator.CrossFade(hideAnimationTrigger, 0f);
            Debug.Log("손목 UI 패널 사라짐 애니메이션 재생 중...");
            
            // 애니메이션이 끝날 때까지 대기
            yield return new WaitForSeconds(hideAnimationDuration);
        }

        // 버튼 색상 복원
        if (currentButton != null)
        {
            RestoreButtonColor(currentButton);
        }

        // 애니메이션 완료 후 패널 제거
        if (currentPanel != null)
        {
            Destroy(currentPanel);
            currentPanel = null;
            currentAnimator = null;
            currentButton = null;
        }

        hideCoroutine = null;
        Debug.Log("손목 UI 패널 닫기 완료");
    }

    /// <summary>
    /// 패널을 즉시 닫기 (애니메이션 없이)
    /// </summary>
    public void CloseImmediately()
    {
        // 진행 중인 코루틴 중지
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }

        // 버튼 색상 복원
        if (currentButton != null)
        {
            RestoreButtonColor(currentButton);
        }

        // 즉시 제거
        if (currentPanel != null)
        {
            Destroy(currentPanel);
            currentPanel = null;
            currentAnimator = null;
            currentButton = null;
        }

        Debug.Log("손목 UI 패널 즉시 닫기");
    }

    /// <summary>
    /// 버튼 색상 변경
    /// </summary>
    private void SetButtonColor(WristButton button, Color color)
    {
        if (button == null) return;
        
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null)
        {
            buttonImage.color = color;
        }
    }

    /// <summary>
    /// 버튼 색상을 원래대로 복원
    /// </summary>
    private void RestoreButtonColor(WristButton button)
    {
        if (button == null) return;
        
        if (originalButtonColors.ContainsKey(button))
        {
            Image buttonImage = button.GetComponent<Image>();
            if (buttonImage != null)
            {
                buttonImage.color = originalButtonColors[button];
            }
        }
    }

    /// <summary>
    /// 사라짐 애니메이션 길이 설정
    /// </summary>
    public void SetHideAnimationDuration(float duration)
    {
        hideAnimationDuration = Mathf.Max(0.1f, duration);
    }

    /// <summary>
    /// 현재 열려있는 패널이 있는지 확인
    /// </summary>
    public bool HasOpenPanel()
    {
        return currentPanel != null;
    }

    /// <summary>
    /// 현재 활성화된 버튼 가져오기
    /// </summary>
    public WristButton GetCurrentButton()
    {
        return currentButton;
    }

    /// <summary>
    /// 버튼 캔버스가 활성화되어 있는지 확인
    /// </summary>
    public bool IsButtonCanvasActive()
    {
        return isButtonCanvasActive;
    }

    public Canvas WristPanelCanvas => wristPanelCanvas;
}