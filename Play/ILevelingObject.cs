using UnityEngine;

namespace Jobworld
{
    public interface ILevelingObject
    {
        LevelingObjectSetting levelingSetting { get; }
        int currentLevel { get; }
        
        int GetLevel();
        void SetLevel(int level);
    }
}