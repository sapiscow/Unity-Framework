using UnityEngine;

namespace Sapiscow.Framework.Systems
{
    /// <summary>
    /// Inherit this class for a main controller (system) in a module.
    /// Use this if your controller needs MonoBehaviour stuffs.
    /// </summary>
    public abstract class BaseSystemMono : MonoBehaviour, ISystem { }
}