using UnityEngine;

namespace Jobworld
{
    [CreateAssetMenu(fileName = "NewSoundData", menuName = "Jobworld/Sound Data")]
    public class SoundData : ScriptableObject
    {
        [Header("Audio Clip")]
        public AudioClip audioClip;

        [Header("Volume")]
        [Range(0f, 2f)]
        public float volumeOffset = 1f;
    }
}