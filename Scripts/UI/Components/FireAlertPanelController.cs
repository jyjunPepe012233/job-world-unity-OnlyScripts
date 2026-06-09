using UnityEngine;
using System.Collections;

namespace Jobworld
{
    /// <summary>
    /// 화재 발생 시 알림 패널을 일정 시간 동안 표시하는 컨트롤러
    /// </summary>
    public class FireAlertPanelController : MonoBehaviour
    {
        [Header("Alert Panel Settings")]
        [SerializeField] private GameObject fireAlertPanelPrefab; // 패널 프리팹
        [SerializeField] private Transform panelParent; // 패널이 생성될 부모 (Canvas 등)
        [SerializeField] private float displayDuration = 3f; // 패널 표시 시간 (초)

        [Header("Animation Settings")]
        [SerializeField] private string showAnimationTrigger = "Show"; // 등장 애니메이션 트리거 이름
        [SerializeField] private string hideAnimationTrigger = "Hide"; // 사라짐 애니메이션 트리거 이름
        [SerializeField] private float hideAnimationDuration = 0.5f; // 사라짐 애니메이션 길이 (초)

        private GameObject _currentPanelInstance; // 현재 생성된 패널 인스턴스
        private Animator _currentAnimator; // 현재 패널의 Animator
        private Coroutine _hideCoroutine;

        private void Awake()
        {
            // 전역 이벤트를 사용하여 모든 화재 발생을 감지
            FiremanMissionManager.OnMissionStartGlobal += OnFireOccurred;
            Debug.Log("FireAlertPanelController: 전역 이벤트 구독 완료");
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            FiremanMissionManager.OnMissionStartGlobal -= OnFireOccurred;
            
            // 진행 중인 코루틴 정리
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }
            
            // 남아있는 패널 인스턴스 정리
            DestroyCurrentPanel();
        }

        private void Start()
        {
            // 프리팹 검증
            if (fireAlertPanelPrefab == null)
            {
                Debug.LogWarning("FireAlertPanel Prefab이 할당되지 않았습니다!");
            }
            
            // panelParent가 없으면 자동으로 Canvas 찾기
            if (panelParent == null)
            {
                Canvas canvas = FindObjectOfType<Canvas>();
                if (canvas != null)
                {
                    panelParent = canvas.transform;
                    Debug.Log("panelParent가 자동으로 Canvas로 설정되었습니다.");
                }
                else
                {
                    Debug.LogWarning("Canvas를 찾을 수 없습니다! 패널 부모를 수동으로 설정하세요.");
                }
            }
        }

        /// <summary>
        /// 화재 발생 시 호출되는 메서드
        /// </summary>
        private void OnFireOccurred()
        {
            Debug.Log("FireAlertPanelController: 화재 발생 이벤트 수신!");
            ShowAlert();
        }

        /// <summary>
        /// 알림 패널 표시
        /// </summary>
        public void ShowAlert()
        {
            if (fireAlertPanelPrefab == null)
            {
                Debug.LogWarning("FireAlertPanel Prefab이 없어서 생성할 수 없습니다!");
                return;
            }

            if (panelParent == null)
            {
                Debug.LogWarning("Panel Parent가 없어서 패널을 생성할 수 없습니다!");
                return;
            }

            // 이전에 실행 중이던 숨김 코루틴이 있다면 중지
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }

            // 기존 패널 인스턴스가 있다면 제거
            DestroyCurrentPanel();

            // 새 패널 인스턴스 생성
            _currentPanelInstance = Instantiate(fireAlertPanelPrefab, panelParent);
            
            // RectTransform 설정 (Canvas 중앙 배치 등)
            if (_currentPanelInstance.TryGetComponent<RectTransform>(out var rectTransform))
            {
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }

            // Animator 컴포넌트 가져오기
            _currentAnimator = _currentPanelInstance.GetComponent<Animator>();
            
            if (_currentAnimator != null)
            {
                // 등장 애니메이션 재생
                _currentAnimator.CrossFade(showAnimationTrigger, 0f);
                Debug.Log($"화재 알림 패널 생성 및 등장 애니메이션 재생! ({displayDuration}초간 표시)");
            }
            else
            {
                Debug.LogWarning("패널에 Animator 컴포넌트가 없습니다! 애니메이션이 재생되지 않습니다.");
            }

            // 일정 시간 후 자동으로 제거
            _hideCoroutine = StartCoroutine(HideAlertAfterDelay());
        }

        /// <summary>
        /// 일정 시간 후 알림 패널을 제거하는 코루틴
        /// </summary>
        private IEnumerator HideAlertAfterDelay()
        {
            yield return new WaitForSeconds(displayDuration);
            yield return StartCoroutine(HideAlertWithAnimation());
        }

        /// <summary>
        /// 알림 패널 제거 (애니메이션 없이 즉시)
        /// </summary>
        public void HideAlert()
        {
            DestroyCurrentPanel();
            Debug.Log("화재 알림 패널 제거");
            _hideCoroutine = null;
        }

        /// <summary>
        /// 애니메이션과 함께 패널 제거
        /// </summary>
        private IEnumerator HideAlertWithAnimation()
        {
            if (_currentAnimator != null && _currentPanelInstance != null)
            {
                // 사라짐 애니메이션 재생
                _currentAnimator.CrossFade(hideAnimationTrigger, 0f);
                Debug.Log("사라짐 애니메이션 재생 중...");
                
                // 애니메이션이 끝날 때까지 대기
                yield return new WaitForSeconds(hideAnimationDuration);
            }

            // 애니메이션 완료 후 패널 제거
            DestroyCurrentPanel();
            Debug.Log("화재 알림 패널 제거 완료");
            _hideCoroutine = null;
        }

        /// <summary>
        /// 현재 패널 인스턴스 제거
        /// </summary>
        private void DestroyCurrentPanel()
        {
            if (_currentPanelInstance != null)
            {
                Destroy(_currentPanelInstance);
                _currentPanelInstance = null;
                _currentAnimator = null;
            }
        }

        /// <summary>
        /// 현재 패널이 표시되고 있는지 확인
        /// </summary>
        public bool IsShowingPanel()
        {
            return _currentPanelInstance != null;
        }

        /// <summary>
        /// 표시 시간 동적 변경
        /// </summary>
        public void SetDisplayDuration(float duration)
        {
            displayDuration = Mathf.Max(0.1f, duration);
        }

        /// <summary>
        /// 수동으로 즉시 제거 (긴급 상황용)
        /// </summary>
        public void HideImmediately()
        {
            if (_hideCoroutine != null)
            {
                StopCoroutine(_hideCoroutine);
                _hideCoroutine = null;
            }

            DestroyCurrentPanel();
            Debug.Log("화재 알림 패널 즉시 제거");
        }

        /// <summary>
        /// 사라짐 애니메이션 길이 설정
        /// </summary>
        public void SetHideAnimationDuration(float duration)
        {
            hideAnimationDuration = Mathf.Max(0.1f, duration);
        }
    }
}