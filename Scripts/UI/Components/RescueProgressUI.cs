using UnityEngine;
using UnityEngine.UI;

namespace Jobworld
{
    public class RescueProgressUI : MonoBehaviour
    {
        [Header("UI 설정")]
        [Tooltip("구출 진도를 표시할 Filled 타입 이미지")]
        [SerializeField] private Image fillImage;

        [Header("애니메이션 설정 (선택)")]
        [Tooltip("fillAmount 변경 시 부드럽게 애니메이션 적용")]
        [SerializeField] private bool useSmoothAnimation = true;
        
        [Tooltip("부드러운 애니메이션 속도")]
        [SerializeField] private float smoothSpeed = 10f;

        [Header("UI 표시 설정")]
        [Tooltip("구출 중이 아닐 때 UI 숨기기")]
        [SerializeField] private bool hideWhenNotRescuing = true;
        
        [Tooltip("UI를 숨길 때 사용할 CanvasGroup (선택사항)")]
        [SerializeField] private CanvasGroup canvasGroup;

        private float _targetFillAmount = 0f;
        private Victim _currentVictim;

        private void Start()
        {
            // fillImage 초기화
            if (fillImage != null)
            {
                fillImage.fillAmount = 0f;
            }
            else
            {
                Debug.LogError("RescueProgressUI: Fill Image가 설정되지 않았습니다!");
            }

            // CanvasGroup이 없으면 자동 생성
            if (hideWhenNotRescuing && canvasGroup == null)
            {
                canvasGroup = GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = gameObject.AddComponent<CanvasGroup>();
                }
            }

            // 초기에는 숨김
            if (hideWhenNotRescuing && canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        private void Update()
        {
            // 부드러운 애니메이션 적용
            if (useSmoothAnimation && fillImage != null)
            {
                fillImage.fillAmount = Mathf.Lerp(
                    fillImage.fillAmount, 
                    _targetFillAmount, 
                    Time.deltaTime * smoothSpeed
                );
            }
        }

        /// <summary>
        /// Victim 이벤트 등록 (Victim이 직접 호출)
        /// </summary>
        public void RegisterVictimEvents(Victim victim)
        {
            if (victim == null) return;

            // 기존 victim이 있다면 이벤트 해제
            if (_currentVictim != null)
            {
                UnregisterVictimEvents(_currentVictim);
            }

            _currentVictim = victim;

            // 이벤트 등록
            _currentVictim.OnRescueProgress += OnRescueProgress;
            _currentVictim.OnRescueStarted += OnRescueStarted;
            _currentVictim.OnRescueCancelled += OnRescueCancelled;
            _currentVictim.OnRescueCompleted += OnRescueCompleted;

            Debug.Log($"RescueProgressUI: {victim.name}의 구출 진도 추적 시작");
        }

        /// <summary>
        /// Victim 이벤트 해제 (Victim이 직접 호출)
        /// </summary>
        public void UnregisterVictimEvents(Victim victim)
        {
            if (victim == null) return;

            victim.OnRescueProgress -= OnRescueProgress;
            victim.OnRescueStarted -= OnRescueStarted;
            victim.OnRescueCancelled -= OnRescueCancelled;
            victim.OnRescueCompleted -= OnRescueCompleted;

            // 현재 추적 중인 victim이면 초기화
            if (_currentVictim == victim)
            {
                _currentVictim = null;
            }

            Debug.Log($"RescueProgressUI: {victim.name}의 구출 진도 추적 해제");
        }

        private void OnRescueProgress(float progress)
        {
            // 진도에 따라 fillAmount 업데이트 (0.0 ~ 1.0)
            if (fillImage != null)
            {
                if (useSmoothAnimation)
                {
                    _targetFillAmount = progress;
                }
                else
                {
                    fillImage.fillAmount = progress;
                }
            }
        }

        private void OnRescueStarted()
        {
            Debug.Log("RescueProgressUI: 구출 시작!");
            
            // UI 표시
            if (hideWhenNotRescuing && canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
            }
        }

        private void OnRescueCancelled()
        {
            Debug.Log("RescueProgressUI: 구출 취소됨. Fill 초기화");
            
            // fillAmount를 0으로 리셋
            if (fillImage != null)
            {
                if (useSmoothAnimation)
                {
                    _targetFillAmount = 0f;
                }
                else
                {
                    fillImage.fillAmount = 0f;
                }
            }

            // UI 숨김
            if (hideWhenNotRescuing && canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        private void OnRescueCompleted()
        {
            Debug.Log("RescueProgressUI: 구출 완료!");
            
            // fillAmount를 1.0으로 설정 (완료)
            if (fillImage != null)
            {
                if (useSmoothAnimation)
                {
                    _targetFillAmount = 1f;
                }
                else
                {
                    fillImage.fillAmount = 1f;
                }
            }

            // 잠시 후 UI 숨김
            if (hideWhenNotRescuing && canvasGroup != null)
            {
                Invoke(nameof(HideUI), 0.5f);
            }
        }

        private void HideUI()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
            }
        }

        private void OnDestroy()
        {
            // 이벤트 해제
            if (_currentVictim != null)
            {
                UnregisterVictimEvents(_currentVictim);
            }
        }
    }
}