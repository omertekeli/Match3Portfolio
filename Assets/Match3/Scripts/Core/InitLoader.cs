using Cysharp.Threading.Tasks;
using Match3.Scripts.Configs;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Enums;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityEngine;

namespace Match3.Scripts.Core
{
    [DefaultExecutionOrder(-99999)]
    public class InitLoader : MonoBehaviour
    {   
        #region Fields
        [SerializeField] private GameObject _boardPrefab;
        [SerializeField] private MonoBehaviour[] _monoServices;
        [SerializeField] private GemSpriteLibrarySO _gemSpriteLibrary;
        [SerializeField] private LevelListSO _levelList;
        #endregion

        private void Awake()
        {
            TryToRegisterServices();
            ServiceLocator.Get<ISceneLoader>().LoadSceneByIndexAsync((int)SceneIndex.MainMenu).Forget();
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

            var sceneLoader = new SceneLoader();
            ServiceLocator.Register<ISceneLoader>(sceneLoader);

            var levelManager = new LevelManager(
                _boardPrefab,
                _levelList,
                ServiceLocator.Get<ISceneLoader>(), 
                ServiceLocator.Get<IEventPublisher>() 
            );
            ServiceLocator.Register<ILevelManager>(levelManager);
        }

        private void RegisterMonoServices()
        {
            foreach (var mb in _monoServices)
            {
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

