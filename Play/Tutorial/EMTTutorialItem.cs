using UnityEngine;
using System.Collections;

namespace Jobworld
{
    public class EMTTutorialItem : TutorialItem
    {
        // 튜토리얼 시작 시 안내 텍스트와 TTS 출력 등 연출
        public override void OnTutorialStart()
        {
            // SO에 저장된 안내 텍스트와 TTS를 순서대로 출력
            if (setting.guideTexts != null && setting.guideTexts.Length > 0)
            {
                StartCoroutine(ShowGuidesWithDelay());
            }
            else
            {
                SetConditionMet(true);
            }
        }

        private IEnumerator ShowGuidesWithDelay()
        {
            for (int i = 0; i < setting.guideTexts.Length; i++)
            {
                string ttsName = (setting.guideTTSClips != null && i < setting.guideTTSClips.Length) ? setting.guideTTSClips[i] : null;
                bool useSFX = (setting.isSFX != null && i < setting.isSFX.Length) ? setting.isSFX[i] : true;
                yield return StartCoroutine(ShowGuide(setting.guideTexts[i], ttsName, useSFX));
                yield return new WaitForSeconds(guideTextDelay);
            }
            SetConditionMet(true);
        }

        public override void OnTutorialStay() {}

        public override void OnTutorialFinish()
        {
            // 튜토리얼 종료 시 연출 (완료 UI만 출력)
            ShowMissionCompleteUI();
        }

        // 텍스트와 TTS를 함께 출력하고, TTS가 끝날 때까지 기다리는 코루틴
        IEnumerator ShowGuide(string text, string ttsName, bool useSFX)
        {
            // 텍스트 출력 UI 처리
            Debug.Log("Guide: " + text);

            // TTS 사운드 출력
            if (!string.IsNullOrEmpty(ttsName))
            {
                AudioSource source = null;
                if (useSFX)
                {
                    source = SoundManager.singleton.PlaySFX(ttsName, transform.position);
                }
                else
                {
                    source = SoundManager.singleton.PlayUISound(ttsName);
                }

                // source가 null이 아니고, 재생이 끝날 때까지 기다립니다.
                if (source != null)
                {
                    yield return new WaitUntil(() => !source.isPlaying);
                }
            }
        }
        
        void ShowMissionCompleteUI() {}
    }
}


