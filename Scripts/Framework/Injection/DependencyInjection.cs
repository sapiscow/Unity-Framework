using Sapiscow.Framework.Singleton;
using Sapiscow.Framework.Systems;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Sapiscow.Framework.Injection
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Inject all dependencies that implements ISystem in <paramref name="target"/> class.
        /// </summary>
        public static void Inject(object target)
            => DependencyInjector.Instance.InjectDependencies(target);

        /// <summary>
        /// Register all dependencies that implements ISystem in <paramref name="target"/> class.
        /// </summary>
        /// <param name="isPersistent">Persistent means as its name (global scoped),
        /// and that dependency also couldn't be unregistered.</param>
        public static void Register(object target, bool isPersistent)
            => DependencyInjector.Instance.RegisterDependecies(target, isPersistent);

        /// <summary>
        /// Unregister all not-persistent dependencies that implements ISystem in <paramref name="target"/> class.
        /// </summary>
        public static void Unregister(object target)
            => DependencyInjector.Instance.UnregisterDependencies(target);

        internal class DependencyInjector : SingletonNative<DependencyInjector>
        {
            private Dictionary<System.Type, object> _dependencies = new();
            private Dictionary<System.Type, object> _persistentDependencies = new();

            private List<object> _injectedSources = new();

            private BindingFlags _acceptedFlags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;

            #region Inject
            public void InjectDependencies(object target)
            {
                if (_injectedSources.Contains(target)) return;

                _injectedSources.Add(target);

                InjectFields(target);
                InjectProperties(target);
            }

            private void InjectFields(object target)
            {
                FieldInfo[] fields = target.GetType().GetFields(_acceptedFlags);
                foreach (FieldInfo field in fields)
                {
                    if (IsSystemInterface(field.FieldType) && _dependencies.TryGetValue(field.FieldType, out object source))
                    {
                        field.SetValue(target, source);
                        InjectDependencies(source);
                    }
                }
            }

            private void InjectProperties(object target)
            {
                PropertyInfo[] properties = target.GetType().GetProperties(_acceptedFlags);
                foreach (PropertyInfo property in properties)
                {
                    if (IsSystemInterface(property.PropertyType) && _dependencies.TryGetValue(property.PropertyType, out object source))
                    {
                        property.SetValue(target, source);
                        InjectDependencies(source);
                    }
                }
            }
            #endregion

            #region Register
            public void RegisterDependecies(object target, bool isPersistent)
            {
                RegisterFields(target, isPersistent);
                RegisterProperties(target, isPersistent);
            }

            private void RegisterFields(object target, bool isPersistent)
            {
                FieldInfo[] fields = target.GetType().GetFields(_acceptedFlags);
                foreach (FieldInfo field in fields)
                    RegisterDependency(field.FieldType, isPersistent);
            }

            private void RegisterProperties(object target, bool isPersistent)
            {
                PropertyInfo[] properties = target.GetType().GetProperties(_acceptedFlags);
                foreach (PropertyInfo property in properties)
                    RegisterDependency(property.PropertyType, isPersistent);
            }

            private void RegisterDependency(System.Type interfaceType, bool isPersistent)
            {
                if (!IsSystemInterface(interfaceType) || _dependencies.ContainsKey(interfaceType))
                    return;

                System.Type classType = ReflectionHelper.GetImplementedClassType(interfaceType);
                object instance = null;

                if (classType.IsSubclassOf(typeof(BaseSystemNative)))
                    instance = System.Activator.CreateInstance(classType);
                else if (classType.IsSubclassOf(typeof(BaseSystemMono)))
                {
                    Object instanceObj;
                    if (isPersistent)
                    {
                        Object resourceObj = Resources.Load(classType.Name, classType);
                        if (resourceObj != null)
                            instanceObj = Object.Instantiate(resourceObj);
                        else
                        {
                            UntraceableLogger.LogWarning($"Prefab {classType.Name} is not found at Resources/{classType.Name}\nCreating a New GameObject ...");
                            GameObject newObj = new(classType.Name + "(New)");
                            instanceObj = newObj.AddComponent(classType);
                        }

                        Object.DontDestroyOnLoad(instanceObj);
                    }
                    else instanceObj = Object.FindObjectOfType(classType);

                    instance = instanceObj;
                }

                _dependencies.Add(interfaceType, instance);
                if (isPersistent) _persistentDependencies.Add(interfaceType, instance);
            }
            #endregion

            #region Unregister
            public void UnregisterDependencies(object target)
            {
                if (_injectedSources.Contains(target)) _injectedSources.Remove(target);

                UnregisterFields(target);
                UnregisterProperties(target);
            }

            private void UnregisterFields(object target)
            {
                FieldInfo[] fields = target.GetType().GetFields(_acceptedFlags);
                foreach (FieldInfo field in fields)
                    UnregisterDependency(field.FieldType);
            }

            private void UnregisterProperties(object target)
            {
                PropertyInfo[] properties = target.GetType().GetProperties(_acceptedFlags);
                foreach (PropertyInfo property in properties)
                    UnregisterDependency(property.PropertyType);
            }

            private void UnregisterDependency(System.Type type)
            {
                if (_dependencies.ContainsKey(type) && !_persistentDependencies.ContainsKey(type))
                    _dependencies.Remove(type);
            }
            #endregion

            private bool IsSystemInterface(System.Type type)
                => type.IsInterface && type.GetInterface(nameof(ISystem)) != null;
        }
    }
}