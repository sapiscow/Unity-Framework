using Sapiscow.Framework.Injection;
using UnityEngine;

namespace Sapiscow.Framework.Boot
{
    /// <summary>
    /// Required to be inherited by any scene launcher or initializer.
    /// Define all scene scoped dependencies in this inherited class.
    /// </summary>
    public class BaseSceneLauncher : MonoBehaviour
    {
        protected virtual void Awake()
        {
            DependencyInjection.Register(this, false);
            DependencyInjection.Inject(this);
        }

        protected virtual void OnDestroy()
        {
            DependencyInjection.Unregister(this);
        }
    }
}