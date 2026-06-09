using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Jobworld
{
    public class FiremanMissionUI : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI timerText;
        public TextMeshProUGUI missionStatusText;
        public Slider progressSlider;
        public GameObject missionCompletePanel;
        public GameObject missionFailedPanel;
        
        [Header("Log Display")]
        public TextMeshProUGUI victimSafetyText;
        public TextMeshProUGUI damageManagementText;
        public TextMeshProUGUI fireRatioText;
        public TextMeshProUGUI victimRatioText;
        public TextMeshProUGUI completionTimeText;
        public TextMeshProUGUI resourceManagementText;

        private FiremanMissionManager _missionManager;
        private bool _isActive = false;

        private void Start()
        {
            _missionManager = FindObjectOfType<FiremanMissionManager>();
            if (_missionManager != null)
            {
                _missionManager.OnTimerUpdate += UpdateTimer;
                _missionManager.OnMissionComplete += OnMissionComplete;
                _missionManager.OnMissionFailed += OnMissionFailed;
                _isActive = true;
            }

            // 초기 UI 상태 설정
            if (missionCompletePanel != null) missionCompletePanel.SetActive(false);
            if (missionFailedPanel != null) missionFailedPanel.SetActive(false);
        }

        private void OnDestroy()
        {
            if (_missionManager != null)
            {
                _missionManager.OnTimerUpdate -= UpdateTimer;
                _missionManager.OnMissionComplete -= OnMissionComplete;
                _missionManager.OnMissionFailed -= OnMissionFailed;
            }
        }

        private void Update()
        {
            if (_isActive && _missionManager != null)
            {
                UpdateProgressSlider();
                UpdateMissionStatus();
                UpdateLiveStats();
            }
        }

        private void UpdateTimer(float remainingTime)
        {
            if (timerText != null)
            {
                int minutes = Mathf.FloorToInt(remainingTime / 60);
                int seconds = Mathf.FloorToInt(remainingTime % 60);
                timerText.text = $"{minutes:00}:{seconds:00}";
                
                // 시간이 얼마 남지 않으면 빨간색으로 표시
                if (remainingTime <= 60f)
                {
                    timerText.color = Color.red;
                }
                else if (remainingTime <= 120f)
                {
                    timerText.color = Color.yellow;
                }
                else
                {
                    timerText.color = Color.white;
                }
            }
        }

        private void UpdateProgressSlider()
        {
            if (progressSlider != null && _missionManager != null)
            {
                float elapsedTime = _missionManager.GetElapsedTime();
                float progress = elapsedTime / _missionManager.goldenTime;
                progressSlider.value = progress;
            }
        }

        private void UpdateMissionStatus()
        {
            if (missionStatusText != null)
            {
                missionStatusText.text = "화재 진압 및 인명 구조 진행 중...";
            }
        }

        private void UpdateLiveStats()
        {
            if (_missionManager != null && _missionManager.missionLog != null)
            {
                var log = _missionManager.missionLog;
                
                if (victimSafetyText != null)
                    victimSafetyText.text = $"피해자 안정성: {log.victimSafetyIndex}";
                
                if (damageManagementText != null)
                    damageManagementText.text = $"피해 관리: {log.damageManagementIndex}";
            }
        }

        private void OnMissionComplete()
        {
            _isActive = false;
            
            if (missionStatusText != null)
                missionStatusText.text = "미션 완료!";
            
            if (missionCompletePanel != null)
            {
                missionCompletePanel.SetActive(true);
                DisplayFinalResults();
            }
        }

        private void OnMissionFailed()
        {
            _isActive = false;
            
            if (missionStatusText != null)
                missionStatusText.text = "미션 실패!";
            
            if (missionFailedPanel != null)
            {
                missionFailedPanel.SetActive(true);
                DisplayFinalResults();
            }
        }

        private void DisplayFinalResults()
        {
            if (_missionManager != null && _missionManager.missionLog != null)
            {
                var log = _missionManager.missionLog;
                
                if (fireRatioText != null)
                    fireRatioText.text = $"소화 비율: {log.fireExtinguishRatio:F1}%";
                
                if (victimRatioText != null)
                    victimRatioText.text = $"구출 비율: {log.victimRescueRatio:F1}%";
                
                if (completionTimeText != null)
                    completionTimeText.text = $"완료 시간: {log.completionTimePercent:F1}%";
                
                if (resourceManagementText != null)
                    resourceManagementText.text = $"자원 관리: {log.resourceManagementIndex}";
            }
        }

        public void OnRestartMission()
        {
            // 미션 재시작 로직
            if (_missionManager != null)
            {
                // 현재 씬 다시 로드하거나 새로운 미션 시작
                UnityEngine.SceneManagement.SceneManager.LoadScene(
                    UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
                );
            }
        }

        public void OnReturnToMenu()
        {
            // 메인 메뉴로 돌아가기
            // 필요에 따라 메인 메뉴 씬 이름을 수정하세요
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }
    }
}