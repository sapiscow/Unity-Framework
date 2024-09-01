namespace Sapiscow.Framework.SerializableData
{
    /// <summary>
    /// Serializeable class that could be loaded and saved as you want.
    /// </summary>
    [System.Serializable]
    public abstract class BaseSerializableData
    {
        public abstract void Load();
        public abstract void Save();
    }
}