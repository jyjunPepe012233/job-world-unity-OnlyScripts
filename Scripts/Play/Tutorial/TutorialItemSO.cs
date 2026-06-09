using System;
using UnityEngine;

namespace Jobworld
{
    // 튜토리얼 아이템의 정보를 ScriptableObject로 관리하는 클래스
    [CreateAssetMenu(fileName = "New Tutorial Item", menuName = "Jobworld/Tutorial Item")]
    public class TutorialItemSO : ScriptableObject
    {
        [Header("Tutorial Information")]
        // 튜토리얼 제목
        public string tutorialTitle;
        // 튜토리얼 설명
        [TextArea(3, 5)]
        public string tutorialDescription;
        // 튜토리얼 순서
        public int tutorialOrder;

        // 단계별 안내 텍스트 및 TTS
        [TextArea(2, 5)]
        public string[] guideTexts; // 텍스트와 TTS를 함께 사용
        public string[] guideTTSClips; // 각 텍스트에 대응되는 TTS 음성 클립
        public bool[] isSFX; // 각 TTS 클립이 SFX인지 UI인지 구분

        public Boolean isGuideOnly; // 안내 텍스트/TTS만 있는 튜토리얼인지 여부(조건 없음)
        public GameObject[] npcPrefabs;
        public string[] npcSpawnTransformNames;
        public string destinationTransformName;

        // 튜토리얼 시작 이벤트
        public event Action started;
        // 튜토리얼 종료 이벤트
        public event Action finished;

        // 튜토리얼 시작 이벤트 호출
        public void InvokeStartEvent()
        {
            started?.Invoke();
        }

        // 튜토리얼 종료 이벤트 호출
        public void InvokeFinishEvent()
        {
            finished?.Invoke();
        }

        // 에디터에서 값이 변경될 때 호출되는 함수
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(tutorialTitle))
                tutorialTitle = name;

            // isSFX 배열 크기를 guideTTSClips 배열 크기와 동기화
            if (guideTTSClips != null)
            {
                Array.Resize(ref isSFX, guideTTSClips.Length);
            }
        }
    }
}