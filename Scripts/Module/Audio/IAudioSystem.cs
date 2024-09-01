using Sapiscow.Framework.Systems;
using UnityEngine;

namespace Sapiscow.Module.Audios
{
    public interface IAudioSystem : ISystem
    {
        void LoadBgm(string bgmName);
        ResourceRequest LoadBgmAsync(string bgmName);
        void PlayBgm(string bgmName, bool stopOthers = true, bool forceRestart = false);
        void StopBgm(string bgmName);
        void StopAllBgm();

        void LoadSfx(string sfxName);
        ResourceRequest LoadSfxAsync(string sfxName);
        int PlaySfx(string sfxName, bool isLooping);
        void StopSfx(string sfxName);
        void StopSfx(int sfxId);
        void StopAllSfx();

        void SetVolume(AudioType audioType, float volume);
        float GetVolume(AudioType audioType);
        bool IsMuted(AudioType audioType);
    }
}