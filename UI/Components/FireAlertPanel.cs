using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Jobworld
{
    /// <summary>
    /// 화재 알림 패널의 UI를 관리하는 클래스
    /// WristUI 또는 일반 Canvas에서 사용 가능
    /// </summary>
    public class FireAlertPanel : MonoBehaviour
    {
        [Header("UI Text Components")]
        [SerializeField] private TextMeshProUGUI titleText; // 패널 제목
        [SerializeField] private TextMeshProUGUI scaleText; // 화재 규모
        [SerializeField] private TextMeshProUGUI locationText; // 위치 이름
        [SerializeField] private TextMeshProUGUI descriptionText; // 상황 설명
        [SerializeField] private TextMeshProUGUI remainingTimeText; // 남은 시간
        [SerializeField] private Image labelImage; // 배경 이미지

        private FiremanMissionManager _missionManager;
        private FireSceneSpawner _fireSceneSpawner;
        private bool _isRestTime = false;

        private void Start()
        {
            // FiremanMissionManager 찾기
            _missionManager = FindObjectOfType<FiremanMissionManager>();
            
            if (_missionManager == null)
            {
                Debug.LogWarning("FiremanMissionManager를 찾을 수 없습니다!");
            }
            
            // FireSceneSpawner 찾기
            _fireSceneSpawner = FindObjectOfType<FireSceneSpawner>();
            
            if (_fireSceneSpawner == null)
            {
                Debug.LogWarning("FireSceneSpawner를 찾을 수 없습니다!");
            }

            // 이벤트 구독
            if (_missionManager != null)
            {
                _missionManager.OnTimerUpdate += UpdateRemainingTime;
            }
            
            // 전역 이벤트 구독 (쉬는 시간 감지용)
            FiremanMissionManager.OnMissionCompleteGlobal += OnMissionComplete;
            FiremanMissionManager.OnMissionFailedGlobal += OnMissionFailed;
            FiremanMissionManager.OnMissionStartGlobal += OnMissionStart;

            // 패널 초기화
            UpdateFireSceneInfo();
            
            if (_missionManager != null)
            {
                UpdateRemainingTime(_missionManager.GetRemainingTime());
            }
        }

        private void OnDestroy()
        {
            // 패널이 파괴될 때 이벤트 구독 해제
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
            ShowPeacefulState();
        }
        
        private void OnMissionFailed()
        {
            // 미션 실패 후 쉬는 시간 시작
            _isRestTime = true;
            ShowPeacefulState();
        }
        
        private void OnMissionStart()
        {
            // 새로운 미션 시작 시 쉬는 시간 종료
            _isRestTime = false;
            UpdateFireSceneInfo();
        }
        
        /// <summary>
        /// 평화로운 상태 표시 (쉬는 시간)
        /// </summary>
        private void ShowPeacefulState()
        {
            if (titleText != null)
                titleText.text = "";
            
            if (scaleText != null)
                scaleText.text = "";
            
            if (locationText != null)
                locationText.text = "평화롭습니다.";
            
            if (descriptionText != null)
                descriptionText.text = "";
            
            if (remainingTimeText != null)
                remainingTimeText.text = "";
            
            // Label 이미지 비활성화
            if (labelImage != null)
                labelImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// 화재 현장 정보를 UI에 업데이트
        /// </summary>
        private void UpdateFireSceneInfo()
        {
            // 쉬는 시간이면 평화로운 상태 표시
            if (_isRestTime)
            {
                ShowPeacefulState();
                return;
            }
            
            if (titleText != null)
                titleText.text = "화재 발생!";
            
            // Label 이미지 활성화
            if (labelImage != null)
                labelImage.gameObject.SetActive(true);
            
            if (_fireSceneSpawner == null)
            {
                SetDefaultFireInfo();
                return;
            }

            var sceneInfo = _fireSceneSpawner.GetCurrentSceneInfo();
            
            if (sceneInfo == null)
            {
                SetDefaultFireInfo();
                return;
            }

            // 규모 텍스트 설정
            string scaleText = GetScaleText(sceneInfo.sceneType);
            if (this.scaleText != null)
                this.scaleText.text = $"규모 : {scaleText}";

            // 위치 텍스트 설정
            if (locationText != null)
                locationText.text = $"위치 : {sceneInfo.locationName}";

            // 설명 텍스트 설정
            string description = string.IsNullOrEmpty(sceneInfo.description) 
                ? "화재 발생!" 
                : sceneInfo.description;
            if (descriptionText != null)
                descriptionText.text = description;

            Debug.Log($"화재 정보 업데이트: {scaleText} / {sceneInfo.locationName} / {description}");
        }

        /// <summary>
        /// 화재 규모를 한글로 변환
        /// </summary>
        private string GetScaleText(FireSceneSpawner.FireSceneType sceneType)
        {
            switch (sceneType)
            {
                case FireSceneSpawner.FireSceneType.Small:
                    return "소";
                case FireSceneSpawner.FireSceneType.Medium:
                    return "중";
                case FireSceneSpawner.FireSceneType.Large:
                    return "대";
                default:
                    return "알 수 없음";
            }
        }

        /// <summary>
        /// 남은 시간 업데이트 (매 프레임마다 호출됨)
        /// </summary>
        private void UpdateRemainingTime(float remainingSeconds)
        {
            // 쉬는 시간이면 남은 시간 표시하지 않음
            if (_isRestTime || remainingTimeText == null) return;

            // 시간 포맷팅 (분:초)
            int minutes = Mathf.FloorToInt(remainingSeconds / 60f);
            int seconds = Mathf.FloorToInt(remainingSeconds % 60f);
            
            // 색상 변경 (시간이 적을수록 빨강)
            if (remainingSeconds <= 60f)
            {
                remainingTimeText.color = Color.red;
            }
            else if (remainingSeconds <= 180f)
            {
                remainingTimeText.color = Color.yellow;
            }
            else
            {
                remainingTimeText.color = Color.white;
            }
            
            remainingTimeText.text = $"{minutes:D2}:{seconds:D2}";
        }

        /// <summary>
        /// 기본 화재 정보 설정 (씬 정보를 가져올 수 없을 때)
        /// </summary>
        private void SetDefaultFireInfo()
        {
            if (titleText != null)
                titleText.text = "";
            
            if (scaleText != null)
                scaleText.text = "";
            
            if (locationText != null)
                locationText.text = "평화롭습니다.";
            
            if (descriptionText != null)
                descriptionText.text = "";
            
            if (remainingTimeText != null)
                remainingTimeText.text = "";
            
            // Label 이미지 비활성화
            if (labelImage != null)
                labelImage.gameObject.SetActive(false);
        }
    }
}