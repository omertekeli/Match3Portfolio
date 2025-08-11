using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3.Scripts.Configs;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Enums;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityCoreModules.Services.ObjectPool;
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

        #region Pool
        [Header("Pool Prefabs")]
        [SerializeField] private GameObject _genericGemPrefab;
        [SerializeField] private GameObject _gemPopVFXPrefab;

        [Header("Initial Pool Sizes")]
        [SerializeField] private int _gemPoolSize = 50;
        [SerializeField] private int _gemPopVfxPoolSize = 10;
        #endregion

        private void Awake()
        {
            TryToRegisterServices();
            CreatePools();
            DOTween.Init();
            ServiceLocator.Get<ISceneLoader>().LoadSceneByIndexAsync((int)SceneIndex.MainMenu).Forget();
        }

        private void CreatePools()
        {
            var poolManager = ServiceLocator.Get<IPoolManager>();
            poolManager.CreatePool(_genericGemPrefab, _gemPoolSize);
            poolManager.CreatePool(_gemPopVFXPrefab, _gemPopVfxPoolSize);
        }

        private void TryToRegisterServices()
        {
            RegisterPOCOServices();
            RegisterMonoServices();
        }

        private void RegisterPOCOServices()
        {
            var eventBus = new EventBus();
            ServiceLocator.Register<IEventPublisher>(eventBus);
            ServiceLocator.Register<IEventSubscriber>(eventBus);

            var poolManager = new PoolManager();
            ServiceLocator.Register<IPoolManager>(poolManager);

            var gemSpriteProvider = new GemSpriteProvider(_gemSpriteLibrary);
            ServiceLocator.Register<GemSpriteProvider>(gemSpriteProvider);

            var sceneLoader = new SceneLoader();
            ServiceLocator.Register<ISceneLoader>(sceneLoader);

            var inputSystem = new InputSystem();
            ServiceLocator.Register<IInputSystem>(inputSystem);

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

