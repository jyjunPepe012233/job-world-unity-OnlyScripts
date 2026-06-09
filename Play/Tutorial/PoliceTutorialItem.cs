using System;
using UnityEngine;
using System.Collections;

namespace Jobworld
{
    public class PoliceTutorialItem : TutorialItem
    {
        private GameObject _spawnedNpc;
        private Coroutine _guideCoroutine;

        // 튜토리얼 시작 시 안내 텍스트와 TTS 출력 등 연출
        public override void OnTutorialStart()
        {
            SpawnNpc();
            // 기존 안내 텍스트/TTS 출력
            if (setting.guideTexts != null && setting.guideTexts.Length > 0)
            {
                if (_guideCoroutine != null)
                {
                    StopCoroutine(_guideCoroutine);
                }
                _guideCoroutine = StartCoroutine(ShowGuidesWithDelay());
            }
            else
            {
                SetConditionMet(true);
            }
        }

        private void SpawnNpc()
        {
            // 기존에 소환된 NPC가 있다면 모두 삭제
            if (_spawnedNpc != null)
            {
                Destroy(_spawnedNpc);
                _spawnedNpc = null;
            }
            // 여러 명 소환을 위해 배열 전체 순회
            if (setting.npcPrefabs != null && setting.npcSpawnTransformNames != null)
            {
                int count = Mathf.Min(setting.npcPrefabs.Length, setting.npcSpawnTransformNames.Length);
                for (int i = 0; i < count; i++)
                {
                    string spawnName = setting.npcSpawnTransformNames[i];
                    if (string.IsNullOrEmpty(spawnName)) continue;
                    var go = GameObject.Find(spawnName);
                    if (go == null) continue;
                    Transform spawnTransform = go.transform;
                    if (spawnTransform == null) continue;
                    if (setting.npcPrefabs[i] == null) continue;

                    Transform playerTransform = tutorialManager != null ? tutorialManager.transform.parent : null;
                    // 부모가 없으면 null로(월드에 소환)
                    var npc = Instantiate(setting.npcPrefabs[i], spawnTransform.position, spawnTransform.rotation, playerTransform);
                    Suspect suspect = npc.GetComponent<Suspect>();
                    if (suspect != null)
                    {
                        suspect.policeTutorialItem = this;
                    }
                    if (tutorialManager != null)
                    {
                        tutorialManager.RegisterNpc(npc);
                    }
                }
            }
        }

        private IEnumerator ShowGuidesWithDelay()
        {
            for (int i = 0; i < setting.guideTexts.Length; i++)
            {
                string ttsName = (setting.guideTTSClips != null && i < setting.guideTTSClips.Length) ? setting.guideTTSClips[i] : null;
                bool useSFX = (setting.isSFX == null || i >= setting.isSFX.Length) || setting.isSFX[i];
                yield return StartCoroutine(ShowGuide(setting.guideTexts[i], ttsName, useSFX));
                yield return new WaitForSeconds(guideTextDelay);
            }
            // 대사만 하는 튜토리얼이면 자동 조건 충족
            if (setting.isGuideOnly)
            {
                SetConditionMet(true);
            }
        }

        public override void OnTutorialStay() {}

        public override void OnTutorialFinish()
        {
            if (_guideCoroutine != null)
            {
                StopCoroutine(_guideCoroutine);
                _guideCoroutine = null;
            }
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

        protected virtual void LateUpdate()
        {
            if (tutorialManager != null && tutorialManager.playerTransform != null)
            {
                transform.position = tutorialManager.playerTransform.position;
            }
        }
    }
}
