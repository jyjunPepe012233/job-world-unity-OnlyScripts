using System.Collections;
using UnityEngine;
using System.Linq;

namespace Jobworld
{
    public class TutorialManager : MonoBehaviour
    {
        // 자동 튜토리얼 시작 여부
        [SerializeField] public bool autoStartTutorial;
        // 튜토리얼 간 딜레이(초)
        [SerializeField] private float m_delayBetweenTutorials = 1f;
        
        public Transform playerTransform;
        
        // 현재 플레이어의 직업 SO
        [SerializeField] private JobSO m_job;
        // 현재 플레이어의 튜토리얼 인덱스
        private int m_currentTutorialIndex = -1;
        // 튜토리얼 시퀀스 진행 중 여부
        private bool m_isTutorialSequenceActive;

        // 현재 진행 중인 튜토리얼 아이템
        private TutorialItem m_currentTutorialItem;
        // 튜토리얼 아이템 프리팹
        [SerializeField] private TutorialItem m_tutorialItemPrefabs;
        
        private SessionPlayerData m_localSavedPlayerData;

        // 현재 튜토리얼 인덱스 프로퍼티
        public int currentTutorialIndex => m_currentTutorialIndex;
        // 튜토리얼 시퀀스 진행 중 여부 프로퍼티
        public bool isTutorialSequenceActive => m_isTutorialSequenceActive;
        
        // 도착지점 체크용 컴포넌트
        [SerializeField] private DestinationChecker m_destinationChecker;

        private readonly System.Collections.Generic.List<GameObject> _spawnedNpcs = new System.Collections.Generic.List<GameObject>();
        private void Start()
        {
            m_currentTutorialItem = CreateTutorialItem();
            if (autoStartTutorial && HasTutorials())
            {
                StartTutorialSequence();
            }
        }
        public void RegisterNpc(GameObject npc)
        {
            if (npc != null && !_spawnedNpcs.Contains(npc))
                _spawnedNpcs.Add(npc);
        }
        private void DestroyAllNpcs()
        {
            foreach (var npc in _spawnedNpcs)
            {
                if (npc != null)
                    Destroy(npc);
            }
            _spawnedNpcs.Clear();
        }

        // 튜토리얼 아이템 생성 및 할당
        private TutorialItem CreateTutorialItem()
        {
            // 인스펙터에서 할당된 튜토리얼 아이템 프리팹을 그대로 인스턴스화하여 반환
            if (m_tutorialItemPrefabs == null) return null;
            var instance = Instantiate(m_tutorialItemPrefabs, transform);
            if (instance != null && m_destinationChecker != null)
            {
                m_destinationChecker.SetTutorialItem(instance);
            }
            return instance;
        }

        // 튜토리얼 시퀀스 시작
        public void StartTutorialSequence()
        {
            if (!HasTutorials()) return;
            if (m_isTutorialSequenceActive) return;
            m_isTutorialSequenceActive = true;
            m_currentTutorialIndex = 0;
            StartCoroutine(StartTutorialWithDelay(m_currentTutorialIndex));
        }

        // 튜토리얼 시퀀스 중단
        public void StopTutorialSequence()
        {
            m_isTutorialSequenceActive = false;
            if (m_currentTutorialIndex >= 0 && m_currentTutorialIndex < m_job.tutorialItemSettings.Length)
            {
                m_job.tutorialItemSettings[m_currentTutorialIndex].InvokeFinishEvent();
            }
            m_currentTutorialIndex = -1;
        }

        // 다음 튜토리얼 시작
        public void StartNextTutorial()
        {
            if (!m_isTutorialSequenceActive) return;
            if (m_currentTutorialIndex >= 0 && m_currentTutorialIndex < m_job.tutorialItemSettings.Length)
            {
                m_job.tutorialItemSettings[m_currentTutorialIndex].InvokeFinishEvent();
            }
            m_currentTutorialIndex++;
            if (m_currentTutorialIndex < m_job.tutorialItemSettings.Length)
            {
                StartCoroutine(StartTutorialWithDelay(m_currentTutorialIndex));
            }
            else
            {
                CompleteTutorialSequence();
            }
        }

        // 튜토리얼 시작 딜레이 코루틴
        private IEnumerator StartTutorialWithDelay(int index)
        {
            yield return new WaitForSeconds(m_delayBetweenTutorials);
            if (m_isTutorialSequenceActive && index < m_job.tutorialItemSettings.Length)
            {
                if (m_currentTutorialItem != null)
                {
                    m_currentTutorialItem.SetTutorialItemSO(m_job.tutorialItemSettings[index]);
                    m_job.tutorialItemSettings[index].InvokeStartEvent();
                }
                // 목표지점 할당 및 도달 상태 초기화
                if (m_destinationChecker != null)
                {
                    // string 이름으로 Transform 찾기
                    Transform destTransform = null;
                    string destName = m_job.tutorialItemSettings[index].destinationTransformName;
                    if (!string.IsNullOrEmpty(destName))
                    {
                        var go = GameObject.Find(destName);
                        if (go != null) destTransform = go.transform;
                    }
                    m_destinationChecker.destinationTransform = destTransform;
                    m_destinationChecker.ResetArrival();
                }
            }
        }
        // 특정 인덱스 튜토리얼 시작
        public void StartSpecificTutorial(int index)
        {
            if (!HasTutorials() || index < 0 || index >= m_job.tutorialItemSettings.Length)
            {
                return;
            }
            StopTutorialSequence();
            m_isTutorialSequenceActive = true;
            m_currentTutorialIndex = index;
            if (m_currentTutorialItem != null)
            {
                m_currentTutorialItem.SetTutorialItemSO(m_job.tutorialItemSettings[index]);
                m_job.tutorialItemSettings[index].InvokeStartEvent();
            }
        }

        // 튜토리얼 시퀀스 완료 처리
        private void CompleteTutorialSequence()
        {
            m_isTutorialSequenceActive = false;
            m_currentTutorialIndex = -1;
            DestroyAllNpcs();
            OnTutorialSequenceComplete();
        }

        // 튜토리얼 존재 여부 확인
        private bool HasTutorials()
        {
            return m_job != null && m_job.tutorialItemSettings != null && m_job.tutorialItemSettings.Length > 0;
        }

        // 튜토리얼 시퀀스 완료 시 호출
        private void OnTutorialSequenceComplete()
        {
            SafeLoading.LoadSceneWithLoading("SampleScene");
        }
    }
}