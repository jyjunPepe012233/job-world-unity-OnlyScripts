using System.Collections;
using UnityEngine;

namespace Jobworld
{
    public class CookPizzaTutorialItem : TutorialItem
    {
        private GameObject _spawnedNpc;
        private Coroutine _guideCoroutine;
        
        private PizzaManager _pizzaManager;
        private Bell _bell;

        private bool _subscribedEvents = false;

        // 튜토리얼 시작 시 안내 텍스트와 TTS 출력 등 연출
        public override void OnTutorialStart()
        {
            SpawnNpc();
            _pizzaManager = FindObjectOfType<PizzaManager>();
            if (_pizzaManager != null)
            {
                _pizzaManager.OrderConfirmed += OnOrderConfirmed;
                _pizzaManager.BakedFromOven += OnBakedFromOven;
                _subscribedEvents = true;
            }

            _bell = FindObjectOfType<Bell>();
            if (_bell != null)
            {
                _bell.CustomerSpawned += OnCustomerSpawned;
                _subscribedEvents = true;
            }
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
                    var npc = Instantiate(setting.npcPrefabs[i], spawnTransform.position, spawnTransform.rotation);
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
            
            if (_subscribedEvents)
            {
                if (_pizzaManager != null)
                {
                    _pizzaManager.OrderConfirmed -= OnOrderConfirmed;
                    _pizzaManager.BakedFromOven -= OnBakedFromOven;
                }
                if (_bell != null)
                {
                    _bell.CustomerSpawned -= OnCustomerSpawned;
                }
                _subscribedEvents = false;
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

        private void OnCustomerSpawned(GameObject customer)
        {
            // One of the tutorial conditions: customer spawned
            SetConditionMet(true);
        }

        private void OnBakedFromOven()
        {
            // One of the tutorial conditions: baked pizza was removed from oven
            SetConditionMet(true);
        }

        private void OnOrderConfirmed(bool success)
        {
            if (success)
            {
                SetConditionMet(true);
            }
        }

        protected virtual void LateUpdate()
        {
            if (tutorialManager != null && tutorialManager.playerTransform != null)
            {
                transform.position = tutorialManager.playerTransform.position;
            }
        }

        private void OnDestroy()
        {
            // Safety unsubscribe
            if (_pizzaManager != null)
            {
                _pizzaManager.OrderConfirmed -= OnOrderConfirmed;
                _pizzaManager.BakedFromOven -= OnBakedFromOven;
            }
            if (_bell != null)
            {
                _bell.CustomerSpawned -= OnCustomerSpawned;
            }
        }
    }
}
