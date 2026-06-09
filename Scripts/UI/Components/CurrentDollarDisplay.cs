using TMPro;
using UnityEngine;
using System.Collections;

namespace Jobworld
{
    public class CurrentDollarDisplay : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI dollarText;
        [SerializeField] private string prefix = "$";
        [SerializeField] private bool autoUpdate = true;
        [SerializeField] private float updateInterval = 1f;
        
        [Header("Animation Settings")]
        [SerializeField] private bool enableAnimation = true;
        [SerializeField] private float animationDuration = 0.8f;
        [SerializeField] private float scaleMultiplier = 1.1f;
        
        private int _lastDollarValue = 0;
        private bool _isAnimating = false;

        private void Start()
        {
            if (dollarText == null)
            {
                Debug.LogError($"CurrentDollarDisplay: TextMeshProUGUI를 찾을 수 없습니다! {gameObject.name}");
                enabled = false;
                return;
            }

            // 약간의 딜레이를 두고 초기화 (Firebase 초기화 대기)
            StartCoroutine(DelayedInitialization());
        }
        
        private IEnumerator DelayedInitialization()
        {
            // Firebase와 세션 초기화를 위해 잠시 대기
            yield return new WaitForSeconds(0.5f);
            
            // 즉시 업데이트 (초기값 설정)
            int initialDollar = GetCurrentDollar();
            _lastDollarValue = initialDollar;
            
            if (initialDollar == -1)
            {
                dollarText.text = "No Session";
            }
            else
            {
                dollarText.text = prefix + " " + initialDollar;
            }

            // 자동 업데이트 시작
            if (autoUpdate)
            {
                InvokeRepeating(nameof(UpdateDollarDisplay), updateInterval, updateInterval);
            }
        }

        private void UpdateDollarDisplay()
        {
            int dollarValue = GetCurrentDollar();
            
            // 값이 변경되었을 때만 업데이트
            if (dollarValue != _lastDollarValue)
            {
                if (enableAnimation && !_isAnimating)
                {
                    // 애니메이션과 함께 업데이트
                    StartCoroutine(AnimatedDollarUpdate(_lastDollarValue, dollarValue));
                }
                else
                {
                    // 즉시 업데이트
                    if (dollarValue == -1)
                    {
                        dollarText.text = "No Session";
                    }
                    else
                    {
                        dollarText.text = prefix + " " + dollarValue.ToString();
                    }
                }
                
                _lastDollarValue = dollarValue;
            }
        }

        private int GetCurrentDollar()
        {
            // GuestClientLocalInfoHolder에서 현재 플레이어 달러 가져오기
            if (GuestClientLocalInfoHolder.playData != null)
            {
                return GuestClientLocalInfoHolder.playData.dollar;
            }
            
            // playData가 null인 경우 (세션에 참가하지 않은 상태)
            return -1; // -1로 표시해서 "No Session" 상태임을 나타냄
        }
        
        private IEnumerator AnimatedDollarUpdate(int fromValue, int toValue)
        {
            _isAnimating = true;
            
            // -1은 "No Session" 상태이므로 애니메이션 없이 즉시 표시
            if (fromValue == -1 || toValue == -1)
            {
                dollarText.text = toValue == -1 ? "No Session" : prefix + " " + toValue.ToString();
            }
            else if (fromValue != toValue)
            {
                // 숫자 카운트와 스케일 애니메이션을 동시에 실행
                yield return StartCoroutine(CountAndScaleAnimation(fromValue, toValue));
            }
            else
            {
                // 같은 값이면 즉시 변경
                dollarText.text = prefix + " " + toValue.ToString();
            }
            
            _isAnimating = false;
        }
        
        private IEnumerator CountAnimation(int fromValue, int toValue)
        {
            float elapsedTime = 0f;
            
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / animationDuration;
                
                // Ease Out 효과
                progress = 1f - Mathf.Pow(1f - progress, 3f);
                
                int currentValue = Mathf.RoundToInt(Mathf.Lerp(fromValue, toValue, progress));
                dollarText.text = prefix + " " + currentValue.ToString();
                
                yield return null;
            }
            
            // 최종 값으로 확실히 설정
            dollarText.text = prefix + " " + toValue.ToString();
        }
        
        private IEnumerator CountAndScaleAnimation(int fromValue, int toValue)
        {
            Vector3 originalScale = dollarText.transform.localScale;
            Vector3 targetScale = originalScale * scaleMultiplier;
            float elapsedTime = 0f;
            
            // 스케일 애니메이션 설정
            float scaleUpDuration = animationDuration * 0.2f;  // 20%는 커지는 시간
            float scaleDownStart = animationDuration * 0.7f;   // 70% 지점부터 작아지기 시작
            
            while (elapsedTime < animationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / animationDuration;
                
                // 숫자 카운트 애니메이션 (Ease Out 효과)
                float easedProgress = 1f - Mathf.Pow(1f - progress, 3f);
                int currentValue = Mathf.RoundToInt(Mathf.Lerp(fromValue, toValue, easedProgress));
                dollarText.text = prefix + " " + currentValue.ToString();
                
                // 스케일 애니메이션 (동시 진행)
                Vector3 currentScale;
                if (elapsedTime <= scaleUpDuration)
                {
                    // 커지는 단계 (0% ~ 20%)
                    float scaleProgress = elapsedTime / scaleUpDuration;
                    currentScale = Vector3.Lerp(originalScale, targetScale, scaleProgress);
                }
                else if (elapsedTime >= scaleDownStart)
                {
                    // 작아지는 단계 (70% ~ 100%)
                    float scaleDownProgress = (elapsedTime - scaleDownStart) / (animationDuration - scaleDownStart);
                    currentScale = Vector3.Lerp(targetScale, originalScale, scaleDownProgress);
                }
                else
                {
                    // 유지 단계 (20% ~ 70%)
                    currentScale = targetScale;
                }
                
                dollarText.transform.localScale = currentScale;
                yield return null;
            }
            
            // 최종 값으로 확실히 설정
            dollarText.text = prefix + " " + toValue.ToString();
            dollarText.transform.localScale = originalScale;
        }
        
        private IEnumerator ScaleAnimation()
        {
            Vector3 originalScale = dollarText.transform.localScale;
            Vector3 targetScale = originalScale * scaleMultiplier;
            
            // 커지는 애니메이션
            float elapsedTime = 0f;
            float scaleDuration = animationDuration * 0.3f;
            
            while (elapsedTime < scaleDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / scaleDuration;
                dollarText.transform.localScale = Vector3.Lerp(originalScale, targetScale, progress);
                yield return null;
            }
            
            // 작아지는 애니메이션
            elapsedTime = 0f;
            while (elapsedTime < scaleDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / scaleDuration;
                dollarText.transform.localScale = Vector3.Lerp(targetScale, originalScale, progress);
                yield return null;
            }
            
            dollarText.transform.localScale = originalScale;
        }

        // 수동으로 업데이트 호출
        public void RefreshDisplay()
        {
            UpdateDollarDisplay();
        }

        // 자동 업데이트 켜기/끄기
        public void SetAutoUpdate(bool enable)
        {
            autoUpdate = enable;
            
            if (enable)
            {
                if (!IsInvoking(nameof(UpdateDollarDisplay)))
                {
                    InvokeRepeating(nameof(UpdateDollarDisplay), updateInterval, updateInterval);
                }
            }
            else
            {
                CancelInvoke(nameof(UpdateDollarDisplay));
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            CancelInvoke();
        }
        
        private void OnDisable()
        {
            StopAllCoroutines();
            _isAnimating = false;
        }
    }
}