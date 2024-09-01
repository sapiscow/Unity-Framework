namespace Sapiscow.Framework.Boot
{
    /// <summary>
    /// Define all global scoped dependencies in class that implements this.
    /// Required to be implemented as your IGameMain.Init() will automatically launched.
    /// </summary>
    public interface IGameMain
    {
        void Init();
    }
}