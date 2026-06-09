using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Jobworld
{
    /// <summary>
    /// FiremanMissionManager의 남은 시간을 이벤트 방식으로 표시하는 클래스
    /// FireAlertPanelController가 패널을 표시하는 동안은 타이머를 숨김
    /// </summary>
    public class MissionTimerDisplay : MonoBehaviour
    {
        [Header("UI Components")]
        [SerializeField] private Image timerImage;
        [SerializeField] private TextMeshProUGUI timerText;
        
        [Header("Color Settings")]
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color warningColor = Color.yellow;
        [SerializeField] private Color dangerColor = Color.red;
        
        [Header("Threshold Settings")]
        [SerializeField] private float warningThreshold = 180f; // 3분
        [SerializeField] private float dangerThreshold = 60f;   // 1분
        
        private FiremanMissionManager _missionManager;
        private FireAlertPanelController _alertPanelController;
        private bool _isRestTime = false;
        private bool _isMissionActive = false;
        
        private void Start()
        {
            // FiremanMissionManager 찾기
            _missionManager = FindObjectOfType<FiremanMissionManager>();
            
            if (_missionManager == null)
            {
                Debug.LogWarning("FiremanMissionManager를 찾을 수 없습니다!");
            }
            
            // FireAlertPanelController 찾기
            _alertPanelController = FindObjectOfType<FireAlertPanelController>();
            
            if (_alertPanelController == null)
            {
                Debug.LogWarning("FireAlertPanelController를 찾을 수 없습니다!");
            }
            
            // 이벤트 구독
            if (_missionManager != null)
            {
                _missionManager.OnTimerUpdate += UpdateRemainingTime;
            }
            
            // 전역 이벤트 구독
            FiremanMissionManager.OnMissionCompleteGlobal += OnMissionComplete;
            FiremanMissionManager.OnMissionFailedGlobal += OnMissionFailed;
            FiremanMissionManager.OnMissionStartGlobal += OnMissionStart;
            
            // 초기 상태는 빈 값
            ClearTimer();
        }
        
        private void OnDestroy()
        {
            // 이벤트 구독 해제
            if (_missionManager != null)
            {
                _missionManager.OnTimerUpdate -= UpdateRemainingTime;
            }
            
            // 전역 이벤트 구독 해제
            FiremanMissionManager.OnMissionCompleteGlobal -= OnMissionComplete;
            FiremanMissionManager.OnMissionFailedGlobal -= OnMissionFailed;
            FiremanMissionManager.OnMissionStartGlobal -= OnMissionStart;
        }
        
        private void OnMissionComplete()
        {
            // 미션 완료 후 쉬는 시간 시작
            _isRestTime = true;
            _isMissionActive = false;
            ClearTimer();
        }
        
        private void OnMissionFailed()
        {
            // 미션 실패 후 쉬는 시간 시작
            _isRestTime = true;
            _isMissionActive = false;
            ClearTimer();
        }
        
        private void OnMissionStart()
        {
            // 새로운 미션 시작
            _isRestTime = false;
            _isMissionActive = true;
        }
        
        /// <summary>
        /// 타이머 텍스트를 비움
        /// </summary>
        private void ClearTimer()
        {
            if (timerImage != null)
            {
                timerImage.enabled = false;
            }
            if (timerText != null)
            {
                timerText.text = "";
            }
        }
        
        /// <summary>
        /// 알림 패널이 현재 표시되고 있는지 확인
        /// </summary>
        private bool IsAlertPanelShowing()
        {
            if (_alertPanelController == null)
            {
                return false;
            }
            
            return _alertPanelController.IsShowingPanel();
        }
        
        /// <summary>
        /// 남은 시간 업데이트 (이벤트로 호출됨)
        /// </summary>
        private void UpdateRemainingTime(float remainingSeconds)
        {
            // timerText가 없으면 빈 값
            if (timerText == null)
            {
                return;
            }
            
            // 쉬는 시간이거나 미션이 활성화되지 않았으면 빈 값
            if (_isRestTime || !_isMissionActive)
            {
                timerImage.enabled = false;
                timerText.text = "";
                return;
            }
            
            // 알림 패널이 표시되고 있으면 타이머 숨김
            if (IsAlertPanelShowing())
            {
                timerImage.enabled = false;
                timerText.text = "";
                return;
            }
            
            // 시간이 0 이하면 빈 값
            if (remainingSeconds <= 0)
            {
                timerImage.enabled = false;
                timerText.text = "";
                return;
            }
            
            // 시간 포맷팅 (분:초)
            int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
            int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
            
            // 색상 변경
            UpdateTimerColor(remainingSeconds);
            
            // 텍스트 업데이트
            timerText.text = $"{minutes:D2}:{seconds:D2}";
            if (!timerImage.enabled)
            {
                timerImage.enabled = true;
            }
        }
        
        /// <summary>
        /// 남은 시간에 따라 타이머 색상 변경
        /// </summary>
        private void UpdateTimerColor(float remainingTime)
        {
            if (timerText == null) return;
            
            if (remainingTime <= dangerThreshold)
            {
                timerText.color = dangerColor;
            }
            else if (remainingTime <= warningThreshold)
            {
                timerText.color = warningColor;
            }
            else
            {
                timerText.color = normalColor;
            }
        }
    }
}