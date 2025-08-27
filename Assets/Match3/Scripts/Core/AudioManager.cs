using UnityEngine;
using Match3.Scripts.Core.Interfaces;
using System.Collections.Generic;
using Match3.Scripts.Enums;
using Match3.Scripts.Keys;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using Match3.Scripts.Core.Events;

namespace Match3.Scripts.Core
{
    public class AudioManager : MonoBehaviour, IAudioManager
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource _musicSource;
        [SerializeField] private AudioSource _sfxSource;

        [Header("Audio Clips")]
        [SerializeField] private List<MusicClipMapping> _musicClips;
        [SerializeField] private List<AudioClipMapping> _sfxClips;
        private Dictionary<MusicType, AudioClip> _musicClipsDict;
        private Dictionary<SfxType, AudioClip> _sfxClipsDict;

        private IEventSubscriber _subscriber;
        private const string MUSIC_ENABLED_KEY = "MusicEnabled";
        private const string SFX_ENABLED_KEY = "SfxEnabled";

        public bool IsMusicEnabled { get; private set; }
        public bool IsSfxEnabled { get; private set; }

        private void Awake()
        {
            _subscriber = ServiceLocator.Get<IEventSubscriber>();
            SetDictionaries();
            InitPlayerPrefers();
            ApplyAudioSettings();
            DontDestroyOnLoad(gameObject);
        }

        private void OnEnable()
        {
            _subscriber.Subscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        private void OnDisable()
        {
            _subscriber.Unsubscribe<GameStateChangedEvent>(OnGameStateChanged);
        }

        public void SetMusicEnabled(bool isEnabled)
        {
            IsMusicEnabled = isEnabled;
            _musicSource.mute = !IsMusicEnabled;
            PlayerPrefs.SetInt(MUSIC_ENABLED_KEY, isEnabled ? 1 : 0);
        }

        public void SetSfxEnabled(bool isEnabled)
        {
            IsSfxEnabled = isEnabled;
            _sfxSource.mute = !IsSfxEnabled;
            PlayerPrefs.SetInt(SFX_ENABLED_KEY, isEnabled ? 1 : 0);
        }

        public void PlaySfx(SfxType type)
        {
            if (IsSfxEnabled && _sfxClipsDict.TryGetValue(type, out AudioClip clip))
            {
                _sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayMusic(MusicType type)
        {
            if (_musicClipsDict.TryGetValue(type, out AudioClip clip))
            {
                if (_musicSource.clip == clip && _musicSource.isPlaying)
                {
                    return;
                }

                _musicSource.clip = clip;
                _musicSource.Play();
            }
            else if (type == MusicType.None)
            {
                _musicSource.Stop();
            }
            else
            {
                Debug.LogWarning($"Music type '{type}' not found in the AudioManager.");
            }
        }

        private void InitPlayerPrefers()
        {
            IsMusicEnabled = PlayerPrefs.GetInt(MUSIC_ENABLED_KEY, 1) == 1;
            IsSfxEnabled = PlayerPrefs.GetInt(SFX_ENABLED_KEY, 1) == 1;
        }

        private void SetDictionaries()
        {
            _sfxClipsDict = new Dictionary<SfxType, AudioClip>();
            foreach (var mapping in _sfxClips)
            {
                _sfxClipsDict[mapping.Type] = mapping.Clip;
            }
            _musicClipsDict = new Dictionary<MusicType, AudioClip>();
            foreach (var mapping in _musicClips)
            {
                _musicClipsDict[mapping.Type] = mapping.Clip;
            }
        }
        private void OnGameStateChanged(GameStateChangedEvent eventData)
        {
            switch (eventData.NewState)
            {
                case GameState.MainMenu:
                    //PlayMusic(MusicType.MainMenu);
                    PlayMusic(MusicType.None);
                    break;
                case GameState.Gameplay:
                    PlayMusic(MusicType.Gameplay);
                    break;
                case GameState.Victory:
                    PlaySfx(SfxType.WinnerPopupOpen);
                    break;
                case GameState.GameOver:
                    PlaySfx(SfxType.LoserPopupOpen);
                    break;
                    //case GameState.Paused:
                    //_musicSource.Pause();
                    //break;
            }
        }

        private void ApplyAudioSettings()
        {
            _musicSource.mute = !IsMusicEnabled;
            _sfxSource.mute = !IsSfxEnabled;
        }
    }
}