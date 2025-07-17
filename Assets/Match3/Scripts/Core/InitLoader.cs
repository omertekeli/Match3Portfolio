using Match3.Scripts.Enums;
using UnityCoreModules.Services;
using UnityEngine;

namespace Match3.Scripts.Core
{
    [DefaultExecutionOrder(-99999)]
    public class InitLoader: MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _services;
        
        private void Awake()
        {
            TryToRegisterServices();
            ServiceLocator.Get<SceneLoader>().LoadSceneByIndex((int)SceneIndex.MainMenu);
        }

        private void TryToRegisterServices()
        {
            foreach (var mb in _services)
            {   
                Debug.Log($"Try to registering service for level {mb.gameObject.name}");
                if (!mb || !mb.enabled || mb is not IService service) continue;
                var type = service.GetType();
                Debug.Log($"Registering service '{type.Name}'");
                typeof(ServiceLocator)
                    .GetMethod("Register")
                    ?.MakeGenericMethod(type)
                    .Invoke(null, new object[] { service, false });
            }
        }

    }
}

