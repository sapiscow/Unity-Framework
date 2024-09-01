namespace Sapiscow.Framework.Singleton
{
    /// <summary>
    /// Singleton for native class, please don't abuse this.
    /// </summary>
    public abstract class SingletonNative<T> where T : SingletonNative<T>, new()
    {
        private static readonly object _lock = new();

        private static T _instance;
        public static T Instance
        {
            get
            {
                lock (_lock)
                {
                    _instance ??= new();

                    return _instance;
                }
            }
        }
    }
}