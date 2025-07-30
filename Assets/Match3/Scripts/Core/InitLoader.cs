using Match3.Scripts.Configs;
using Match3.Scripts.Enums;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityEngine;

namespace Match3.Scripts.Core
{
    [DefaultExecutionOrder(-99999)]
    public class InitLoader : MonoBehaviour
    {
        [SerializeField] private MonoBehaviour[] _services;
        [SerializeField] private GemSpriteLibrarySO _gemSpriteLibrary;

        private void Awake()
        {
            TryToRegisterServices();
            ServiceLocator.Get<SceneLoader>().LoadSceneByIndex((int)SceneIndex.MainMenu);
        }

        private void TryToRegisterServices()
        {
            RegisterPOCOServices();
            RegisterMonoServices();
        }

        private void RegisterPOCOServices()
        {
            var gemSpriteProvider = new GemSpriteProvider(_gemSpriteLibrary);
            ServiceLocator.Register<GemSpriteProvider>(gemSpriteProvider);

            var eventBus = new EventBus();
            ServiceLocator.Register<IEventPublisher>(eventBus);
            ServiceLocator.Register<IEventSubscriber>(eventBus);
        }

        private void RegisterMonoServices()
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

