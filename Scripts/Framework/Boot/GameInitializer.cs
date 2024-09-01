using Sapiscow.Framework.Injection;
using UnityEngine;

namespace Sapiscow.Framework.Boot
{
    public static class GameInitializer
    {
        /// <summary>
        /// Initial booter of the whole game, start from any scene, this always be executed.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Main()
        {
            System.Type classType = ReflectionHelper.GetImplementedClassType(typeof(IGameMain));
            if (classType == null)
            {
                Debug.LogError("There is no class that implemented IGameMain, game couldn't be started!");
                return;
            }

            IGameMain gameMain = System.Activator.CreateInstance(classType) as IGameMain;
            DependencyInjection.Register(gameMain, true);
            DependencyInjection.Inject(gameMain);
            gameMain.Init();
        }
    }
}