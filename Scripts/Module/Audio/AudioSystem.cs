using Sapiscow.Framework.Systems;
using UnityEngine;

namespace Sapiscow.Module.Audios
{
    public class AudioSystem : BaseSystemMono, IAudioSystem
    {
        protected virtual string _bgmResourcesPath => "Bgm/";
        protected virtual string _bgmPrefix => "bgm_";
        protected virtual string _sfxResourcesPath => "Sfx/";
        protected virtual string _sfxPrefix => "sfx_";

        protected AudioPlayer _player;
        protected AudioPlayerPrefsData _playerPrefsData;

        protected virtual void Awake()
        {
            _playerPrefsData = new();
            _playerPrefsData.Load();

            _player = gameObject.AddComponent<AudioPlayer>();
            _player.Init(_playerPrefsData);
        }

        #region Bgm
        public void LoadBgm(string bgmName)
        {
            bgmName = _bgmPrefix + bgmName.ToLower();
            if (!_player.IsBgmPrepared(bgmName))
            {
                AudioClip clip = Resources.Load<AudioClip>(_bgmResourcesPath + bgmName);
                _player.PrepareBgm(bgmName, clip);
            }
        }

        public ResourceRequest LoadBgmAsync(string bgmName)
        {
            bgmName = _bgmPrefix + bgmName.ToLower();
            if (!_player.IsBgmPrepared(bgmName))
            {
                ResourceRequest request = Resources.LoadAsync<AudioClip>(_bgmResourcesPath + bgmName);
                request.completed += async => _player.PrepareBgm(bgmName, (AudioClip)request.asset);

                return request;
            }

            return null;
        }

        public void PlayBgm(string bgmName, bool stopOthers = true, bool forceRestart = false)
            => _player.PlayBgm(_bgmPrefix + bgmName.ToLower(), stopOthers, forceRestart);

        public void StopBgm(string bgmName)
            => _player.StopBgm(_bgmPrefix + bgmName.ToLower());

        public void StopAllBgm()
            => _player.StopAllBgm();
        #endregion

        #region Sfx
        public void LoadSfx(string sfxName)
        {
            sfxName = _sfxPrefix + sfxName.ToLower();
            if (!_player.IsSfxPrepared(sfxName))
            {
                AudioClip clip = Resources.Load<AudioClip>(_sfxResourcesPath + sfxName);
                _player.PrepareSfx(sfxName, clip);
            }
        }

        public ResourceRequest LoadSfxAsync(string sfxName)
        {
            sfxName = _sfxPrefix + sfxName.ToLower();
            if (!_player.IsSfxPrepared(sfxName))
            {
                ResourceRequest request = Resources.LoadAsync<AudioClip>(_sfxResourcesPath + sfxName);
                request.completed += async => _player.PrepareSfx(sfxName, (AudioClip)request.asset);

                return request;
            }

            return null;
        }

        public int PlaySfx(string sfxName, bool isLooping = false)
            => _player.PlaySfx(_sfxPrefix + sfxName.ToLower(), isLooping);

        public void StopSfx(string sfxName)
            => _player.StopSfx(_sfxPrefix + sfxName.ToLower());

        public void StopSfx(int sfxId)
            => _player.StopSfx(sfxId);

        public void StopAllSfx()
            => _player.StopAllSfx();
        #endregion

        public void SetVolume(AudioType audioType, float volume)
        {
            _playerPrefsData.SetVolume(audioType, volume);
            _player.SetVolume(audioType, volume);
        }

        public float GetVolume(AudioType audioType)
            => _playerPrefsData.GetVolume(audioType);

        public bool IsMuted(AudioType audioType)
            => GetVolume(audioType) == 0f;
    }
}