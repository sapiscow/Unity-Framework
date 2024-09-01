using System.Collections.Generic;
using UnityEngine;

namespace Sapiscow.Module.Audios
{
    public class AudioPlayer : MonoBehaviour
    {
        private Dictionary<string, AudioSource> _bgmPool = new();

        private List<AudioSource> _sfxPool = new();
        private Dictionary<string, AudioClip> _sfxCaches = new();

        private AudioPlayerPrefsData _prefsData;

        public void Init(AudioPlayerPrefsData prefsData)
        {
            _prefsData = prefsData;
        }

        #region Bgm
        public void PrepareBgm(string bgmName, AudioClip bgmClip)
        {
            AudioSource source = CreateAudioSource(AudioType.Bgm);
            source.clip = bgmClip;
            source.loop = true;

            _bgmPool.Add(bgmName, source);
        }

        public void PlayBgm(string bgmName, bool stopOthers = true, bool forceRestart = false)
        {
            if (!IsBgmPrepared(bgmName))
            {
                Debug.LogWarning($"[Audios] {bgmName} hasn't be prepared yet.");
                return;
            }

            if (!forceRestart && IsPlayingBgm(bgmName)) return;

            if (stopOthers) StopAllBgm();

            AudioSource source = _bgmPool[bgmName];
            if (!source.isPlaying) source.Play();
        }

        public void StopBgm(string bgmName)
        {
            if (!IsBgmPrepared(bgmName)) return;

            AudioSource source = _bgmPool[bgmName];
            if (source.isPlaying) source.Stop();
        }

        public void StopAllBgm()
        {
            foreach (AudioSource source in _bgmPool.Values)
                source.Stop();
        }

        public bool IsBgmPrepared(string bgmName) => _bgmPool.ContainsKey(bgmName);

        public bool IsPlayingBgm(string bgmName) => _bgmPool[bgmName].isPlaying;
        #endregion

        #region Sfx
        public void PrepareSfx(string sfxName, AudioClip sfxClip)
        {
            if (!IsSfxPrepared(sfxName))
                _sfxCaches.Add(sfxName, sfxClip);
        }

        public int PlaySfx(string sfxName, bool isLooping = false)
        {
            if (!IsSfxPrepared(sfxName))
            {
                Debug.LogWarning($"[Audios] {sfxName} hasn't be prepared yet.");
                return -1;
            }

            AudioSource source = _sfxPool.Find(s => !s.isPlaying);
            if (source == null)
            {
                source = CreateAudioSource(AudioType.Sfx);
                _sfxPool.Add(source);
            }

            AudioClip clip = _sfxCaches[sfxName];
            source.clip = clip;
            source.loop = isLooping;

            if (isLooping) source.Play();
            else source.PlayOneShot(clip);

            return _sfxPool.IndexOf(source);
        }

        public void StopSfx(string sfxName)
        {
            if (!IsSfxPrepared(sfxName)) return;

            foreach (AudioSource source in _sfxPool)
                if (source.clip == _sfxCaches[sfxName] && source.isPlaying)
                    source.Stop();
        }

        public void StopSfx(int sfxId)
        {
            if (sfxId >= 0 && sfxId < _sfxPool.Count && _sfxPool[sfxId].isPlaying)
                _sfxPool[sfxId].Stop();
        }

        public void StopAllSfx()
        {
            foreach (AudioSource source in _sfxPool)
                source.Stop();
        }

        public void SetSpecificSfxVolume(int sfxId, float volume)
        {
            if (sfxId >= 0 && sfxId < _sfxPool.Count)
                _sfxPool[sfxId].volume = volume;
        }

        public bool IsSfxPrepared(string sfxName) => _sfxCaches.ContainsKey(sfxName);
        #endregion

        public void SetVolume(AudioType audioType, float volume)
        {
            switch (audioType)
            {
                case AudioType.Bgm:
                    foreach (AudioSource source in _bgmPool.Values)
                        source.volume = volume;
                    break;

                case AudioType.Sfx:
                    foreach (AudioSource source in _sfxPool)
                        source.volume = volume;
                    break;
            }
        }

        private AudioSource CreateAudioSource(AudioType audioType)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = _prefsData.GetVolume(audioType);
            source.priority = audioType == AudioType.Bgm ? 255 : 0;
            source.playOnAwake = false;

            return source;
        }
    }
}