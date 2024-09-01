using UnityEngine;

namespace Sapiscow.Framework.SerializableData
{
    /// <summary>
    /// Serializeable class for saved to and loaded from PlayerPrefs.
    /// </summary>
    public abstract class BaseSerializablePlayerPrefs : BaseSerializableData
    {
        public abstract string GetKeyword();

        public override void Load()
        {
            if (PlayerPrefs.HasKey(GetKeyword())) JsonParser.FromJsonOverwrite(PlayerPrefs.GetString(GetKeyword()), this);
            else Save();
        }

        public override void Save()
            => PlayerPrefs.SetString(GetKeyword(), JsonParser.ToJson(this));
    }
}