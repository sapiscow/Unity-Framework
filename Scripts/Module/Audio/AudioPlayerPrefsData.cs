using Sapiscow.Framework.SerializableData;

namespace Sapiscow.Module.Audios
{
    [System.Serializable]
    public class AudioPlayerPrefsData : BaseSerializablePlayerPrefs
    {
        private float _bgmVolume = 1f;
        private float _sfxVolume = 1f;

        public override string GetKeyword() => "_Audios_";

        public void SetVolume(AudioType audioType, float volume)
        {
            switch (audioType)
            {
                case AudioType.Bgm: _bgmVolume = volume; break;
                case AudioType.Sfx: _sfxVolume = volume; break;
            }

            Save();
        }

        public float GetVolume(AudioType audioType)
            => audioType switch
            {
                AudioType.Bgm => _bgmVolume,
                AudioType.Sfx => _sfxVolume,
                _ => 0f
            };
    }
}