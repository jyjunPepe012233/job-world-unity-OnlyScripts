using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using AYellowpaper.SerializedCollections;

namespace Jobworld
{
    public class SoundManager : MonoBehaviourSingleton<SoundManager>
    {
        private AudioMixer m_audioMixer; // 오디오 믹서
        private AudioMixerGroup m_sfxGroup; // SFX 오디오 믹서 그룹
        private AudioMixerGroup m_uiGroup; // UI 오디오 믹서 그룹

        private AudioSource m_bgmSource; // BGM 오디오 소스
        private GameObject m_audioSourcePrefab; // 오디오 소스 프리팹

        private int m_maxAudioSourceCount = 10; // 최대 오디오 소스 개수
        private int m_audioSourceSummonAmount = 2; // 오디오 소스 생성 단위

        private SerializedDictionary<string, SoundData> m_bgmAudios; // BGM 데이터
        private SerializedDictionary<string, SoundData> m_sfxAudios; // SFX 데이터
        private SerializedDictionary<string, SoundData> m_uiAudios; // UI 데이터

        private readonly Dictionary<string, AudioSource> m_loopingSfxSources = new();

        private readonly Queue<AudioSource> m_audioSourcePool = new();
        private readonly List<AudioSource> m_allSources = new();

        private bool m_isMuted;
        public bool isMuted
        {
            get => m_isMuted;
            set
            {
                m_isMuted = value;
                m_audioMixer.SetFloat("MasterVolume", value ? -80f : 0f);
            }
        }

        private bool m_isInitialized = false;
        private AudioSource lastPlayedSFXSource;
        private Coroutine m_bgmCoroutine;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        // 본 클래스의 수정이 필요함. (자동 객체 생성 방식에 적응하지 못한 클래스)
        private static void InitializeOnLoad()
        {
            InitializeSingleton();
        }

        private void LoadDependencies()
        {
            // AudioMixer 및 AudioMixerGroup 로드
            m_audioMixer = Resources.Load<AudioMixer>("Audio/AudioMixer");
            m_sfxGroup = m_audioMixer?.FindMatchingGroups("SFX")[0];
            m_uiGroup = m_audioMixer?.FindMatchingGroups("UI")[0];

            // AudioSource 프리팹 로드
            m_audioSourcePrefab = Resources.Load<GameObject>("Audio/PooledAudioSource");

            // BGM, SFX, UI SoundData 로드
            var bgmWrapper = Resources.Load<SoundDataWrapper>("Audio/BGMSoundData");
            var sfxWrapper = Resources.Load<SoundDataWrapper>("Audio/SFXSoundData");
            var uiWrapper = Resources.Load<SoundDataWrapper>("Audio/UISoundData");

            if (bgmWrapper != null) m_bgmAudios = bgmWrapper.soundDataDictionary;
            if (sfxWrapper != null) m_sfxAudios = sfxWrapper.soundDataDictionary;
            if (uiWrapper != null) m_uiAudios = uiWrapper.soundDataDictionary;

            // BGM AudioSource 생성
            GameObject bgmObject = new GameObject("BGMSource");
            m_bgmSource = bgmObject.AddComponent<AudioSource>();
            m_bgmSource.outputAudioMixerGroup = m_audioMixer?.FindMatchingGroups("BGM")[0];
            DontDestroyOnLoad(bgmObject);
        }

        public void Awake()
        {
            if (!m_isInitialized)
            {
                LoadDependencies(); // 의존성 로드
                CreateAudioSourcePool(m_audioSourceSummonAmount);
                m_isInitialized = true;
            }
        }

        private void CreateAudioSourcePool(int amount)
        {
            for (int i = 0; i < amount && m_allSources.Count < m_maxAudioSourceCount; i++)
            {
                GameObject obj = Instantiate(m_audioSourcePrefab, transform);
                AudioSource source = obj.GetComponent<AudioSource>();
                source.playOnAwake = false;
                m_audioSourcePool.Enqueue(source);
                m_allSources.Add(source);
            }
        }

        private AudioSource GetAvailableSource()
        {
            foreach (var source in m_allSources)
            {
                if (!source.isPlaying)
                    return source;
            }

            if (m_allSources.Count < m_maxAudioSourceCount)
            {
                CreateAudioSourcePool(m_audioSourceSummonAmount);
                return GetAvailableSource();
            }

            var fallback = m_allSources[^1];
            fallback.Stop();
            return fallback;
        }

        private bool TryGetValidSoundData(SerializedDictionary<string, SoundData> dict, string name, out SoundData data)
        {
            if (dict.TryGetValue(name, out data) && data.audioClip != null)
                return true;
            data = null;
            return false;
        }

        public void PlayBGM(string name, float fadeTime = 0f)
        {
            if (!TryGetValidSoundData(m_bgmAudios, name, out var data))
                return;
            if (m_bgmCoroutine != null)
                CoroutineHandler.singleton.StopCoroutineSafe(m_bgmCoroutine);
            m_bgmCoroutine = StartCoroutine(FadeInBGM(data.audioClip, data.volumeOffset, fadeTime));
        }

        public void StopBGM(string name, float fadeTime = 2f)
        {
            if (m_bgmSource.clip == null || m_bgmSource.clip.name != name)
                return;

            if (m_bgmCoroutine != null)
                CoroutineHandler.singleton.StopCoroutineSafe(m_bgmCoroutine);

            m_bgmCoroutine = CoroutineHandler.singleton.StartCoroutineSafe(FadeOutBGM(fadeTime), true);
        }

        private IEnumerator FadeInBGM(AudioClip clip, float volume, float duration)
        {
            m_bgmSource.clip = clip;
            m_bgmSource.volume = 0f;
            m_bgmSource.Play();

            float timer = 0f;
            while (timer < duration)
            {
                m_bgmSource.volume = Mathf.Lerp(0, volume, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            m_bgmSource.volume = volume;
        }

        private IEnumerator FadeOutBGM(float duration)
        {
            float startVolume = m_bgmSource.volume;
            float timer = 0f;

            while (timer < duration)
            {
                m_bgmSource.volume = Mathf.Lerp(startVolume, 0, timer / duration);
                timer += Time.deltaTime;
                yield return null;
            }

            m_bgmSource.Stop();
            m_bgmSource.clip = null;
        }
        
        public AudioSource PlaySFX(string name, Vector3 position)
        {
            if (!TryGetValidSoundData(m_sfxAudios, name, out var data))
                return null;
            var source = GetAvailableSource();
            source.transform.position = position;
            PlayClipOnSource(source, data, spatial: true, m_sfxGroup);
            lastPlayedSFXSource = source;
            return source;
        }

        public AudioSource PlaySFX(string name, Vector3 position, Transform parent)
        {
            var source = PlaySFX(name, position);
            if (source == null) return null;
            source.transform.SetParent(parent);
            source.transform.localPosition = Vector3.zero;
            CoroutineHandler.singleton.StartCoroutineSafe(DetachAfterPlay(source), true);
            return source;
        }

        private IEnumerator DetachAfterPlay(AudioSource source)
        {
            yield return new WaitUntil(() => !source.isPlaying);
            source.transform.SetParent(transform);
        }
        
        public void PlayLoopingSFX(string name, Vector3 position, Transform parent = null)
        {
            if (m_loopingSfxSources.ContainsKey(name))
                return;
            if (!TryGetValidSoundData(m_sfxAudios, name, out var data))
                return;
            var source = GetAvailableSource();
            source.transform.position = position;
            if (parent != null)
            {
                source.transform.SetParent(parent);
                source.transform.localPosition = Vector3.zero;
            }
            PlayClipOnSource(source, data, spatial: true, m_sfxGroup);
            source.loop = true;
            m_loopingSfxSources[name] = source;
        }
        
        public void StopLoopingSFX(string name)
        {
            if (!m_loopingSfxSources.TryGetValue(name, out var source))
                return;
            source.loop = false;
            source.Stop();
            source.clip = null;
            source.transform.SetParent(transform);
            m_loopingSfxSources.Remove(name);
        }
        
        public AudioSource PlayUISound(string name)
        {
            if (!TryGetValidSoundData(m_uiAudios, name, out var data))
                return null;
            var source = GetAvailableSource();
            source.transform.position = Vector3.zero;
            PlayClipOnSource(source, data, spatial: false, m_uiGroup);
            return source;
        }

        private void PlayClipOnSource(AudioSource source, SoundData data, bool spatial, AudioMixerGroup group)
        {
            source.clip = data.audioClip;
            source.volume = data.volumeOffset;
            source.spatialBlend = spatial ? 1f : 0f;
            source.outputAudioMixerGroup = group;
            source.Play();
        }

        public void SetVolume(string group, float value)
        {
            float dB = Mathf.Lerp(-80f, 0f, Mathf.Clamp01(value));
            m_audioMixer.SetFloat($"{group}Volume", dB);
        }

        public void Initialize(
            AudioMixer audioMixer,
            AudioMixerGroup sfxGroup,
            AudioMixerGroup uiGroup,
            AudioSource bgmSource,
            GameObject audioSourcePrefab,
            SerializedDictionary<string, SoundData> bgmAudios,
            SerializedDictionary<string, SoundData> sfxAudios,
            SerializedDictionary<string, SoundData> uiAudios
        )
        {
            m_audioMixer = audioMixer;
            m_sfxGroup = sfxGroup;
            m_uiGroup = uiGroup;
            m_bgmSource = bgmSource;
            m_audioSourcePrefab = audioSourcePrefab;
            m_bgmAudios = bgmAudios;
            m_sfxAudios = sfxAudios;
            m_uiAudios = uiAudios;
        }
    }
}
