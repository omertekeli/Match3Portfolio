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
                if (mb is not IService service) continue;
                var type = service.GetType();
                typeof(ServiceLocator)
                    .GetMethod("Register")
                    ?.MakeGenericMethod(type)
                    .Invoke(null, new object[] { service, false });
            }
        }

    }
}

