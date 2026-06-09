using UnityEngine;

namespace Jobworld
{
    // 튜토리얼 아이템의 동작을 정의하는 추상 클래스
    // 각 직업별로 상속하여 구체적인 튜토리얼 단계를 구현
    public abstract class TutorialItem : MonoBehaviour
    { 
        // 튜토리얼 설정 데이터
        // 각 튜토리얼 단계의 텍스트, TTS, 순서 등 정보를 담고 있음
        protected TutorialItemSO setting;

        // SO 이벤트 무시 여부
        // true면 ScriptableObject의 started/finished 이벤트를 구독하지 않음
        [SerializeField] private bool m_ignoreSoEvent;
        public bool ignoreSoEvent
        {
            get => m_ignoreSoEvent;
            set
            {
                if (value != m_ignoreSoEvent)
                {
                    m_ignoreSoEvent = value;
                    if (value)
                        UnsubscribeSoEvent();
                    else
                        SubscribeSoEvent();
                }
            }
        }

        // 튜토리얼 진행 중 여부
        private bool m_isTutorialProgressing;
        public bool isTutorialProgressing => m_isTutorialProgressing;
        // 현재 할당된 튜토리얼 설정 SO
        public TutorialItemSO Setting => setting;

        // TutorialManager 참조
        protected TutorialManager tutorialManager;

        // 텍스트 간 딜레이(초)
        [SerializeField] protected float guideTextDelay = 1.0f;

        // ScriptableObject의 이벤트 구독
        private void SubscribeSoEvent()
        {
            if (setting != null)
            {
                setting.started += StartTutorial;
                setting.finished += FinishTutorial;
            }
        }
        // ScriptableObject의 이벤트 해제
        private void UnsubscribeSoEvent()
        {
            if (setting != null)
            {
                setting.started -= StartTutorial;
                setting.finished -= FinishTutorial;
            }
        }

        // 컴포넌트 초기화 시 이벤트 구독 및 TutorialManager 참조
        private void Awake()
        {
            if (!m_ignoreSoEvent) 
                SubscribeSoEvent();
            tutorialManager = GetComponentInParent<TutorialManager>();
        }

        // 컴포넌트 파괴 시 이벤트 해제
        private void OnDestroy()
        {
            UnsubscribeSoEvent();
        }

        // 매 프레임마다 튜토리얼 진행 체크
        // 조건 달성 시 TutorialManager에 다음 단계 요청
        private void Update()
        {
            if (m_isTutorialProgressing)
            {
                OnTutorialStay();
                if (CheckStepCondition())
                {
                    if (tutorialManager != null)
                        tutorialManager.StartNextTutorial();
                }
            }
        }

        // 튜토리얼 조건 충족 여부 (공통)
        private bool isConditionMet = false;
        // 외부에서 조건 충족 여부를 세팅하는 메서드 (공통)
        public void SetConditionMet(bool value)
        {
            isConditionMet = value;
        }

        // 각 단계별 조건 체크
        protected virtual bool CheckStepCondition() { return isConditionMet; }

        // 외부에서 SO를 동적으로 할당하는 메서드
        // 튜토리얼 단계 변경 시 사용
        public void SetTutorialItemSO(TutorialItemSO so)
        {
            if (setting != null && setting != so)
            {
                UnsubscribeSoEvent();
            }
            setting = so;
            SubscribeSoEvent();
            // 단계 시작 시 조건 초기화
            isConditionMet = false;
        }

        // 튜토리얼 시작 (SO 이벤트 또는 직접 호출)
        public void StartTutorial()
        {
            if (m_isTutorialProgressing)
            {
                return;
            }
            m_isTutorialProgressing = true;
            OnTutorialStart();
        }

        // 튜토리얼 종료 (SO 이벤트 또는 직접 호출)
        public void FinishTutorial()
        {
            if (!m_isTutorialProgressing)
            {
                return;
            }
            m_isTutorialProgressing = false;
            OnTutorialFinish();
        }

        // 각 단계별 시작/진행/종료 동작
        public abstract void OnTutorialStart();
        public abstract void OnTutorialStay();
        public abstract void OnTutorialFinish();
    }
}