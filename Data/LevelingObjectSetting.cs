using UnityEngine;

namespace Jobworld
{
    [CreateAssetMenu(fileName = "JOBNAME_LevelingSetting_ID", menuName = "Jobworld/Setting/Leveling Setting")]
    public class LevelingObjectSetting : ScriptableObject
    {
        [SerializeField] private string id;
        [SerializeField] private string name;
        [SerializeField] private int maxLevel = 1;
        [SerializeField] private Sprite icon;
        [SerializeField] private int[] levelUpCosts = new int[0];
        [SerializeField] private string[] levelEffectDescriptions = new string[0];

        public string Id => id;
        public string Name => name;
        public int MaxLevel => maxLevel;
        public Sprite Icon => icon;
        public int[] LevelUpCosts => levelUpCosts;
        public string[] LevelEffectDescriptions => levelEffectDescriptions;

        private void OnValidate()
        {
            if (maxLevel <= 0)
            {
                maxLevel = 1;
            }

            // 레벨업 비용 배열 크기 조정 (maxLevel - 1)
            int levelUpCostsLength = maxLevel - 1;
            if (levelUpCosts.Length != levelUpCostsLength)
            {
                System.Array.Resize(ref levelUpCosts, levelUpCostsLength);
            }

            // 레벨 효과 설명 배열 크기 조정 (maxLevel)
            int effectDescriptionsLength = maxLevel;
            if (levelEffectDescriptions.Length != effectDescriptionsLength)
            {
                System.Array.Resize(ref levelEffectDescriptions, effectDescriptionsLength);
            }
        }
    }
}