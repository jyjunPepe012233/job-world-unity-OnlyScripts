using UnityEngine;
using AYellowpaper.SerializedCollections;

namespace Jobworld
{
    [CreateAssetMenu(fileName = "SoundDataWrapper", menuName = "Jobworld/SoundDataWrapper")]
    public class SoundDataWrapper : ScriptableObject
    {
        public SerializedDictionary<string, SoundData> soundDataDictionary;
    }
}
