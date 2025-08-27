using Cysharp.Threading.Tasks;
using DG.Tweening;
using Match3.Scripts.Configs;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Enums;
using Match3.Scripts.Systems.Board.Data;
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
        [SerializeField] private PiecePrefabDB _piecePrefabDB;
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
        }

        private void CreatePools()
        {
            var poolManager = ServiceLocator.Get<IPoolManager>();
            poolManager.CreatePool(_piecePrefabDB.GenericGemPrefab, _gemPoolSize);
            poolManager.CreatePool(_gemPopVFXPrefab, _gemPopVfxPoolSize);
            //TODO: create pool for boardPowers
            //TODO: create pool for obstacles
        }

        private void TryToRegisterServices()
        {
            RegisterPOCOServices();
            RegisterMonoServices();
        }

        private void RegisterPOCOServices()
        {
            var eventBus = new EventBus();
            ServiceLocator.Register<IEventBus>(eventBus);
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

            var goalSystem = new GoalSystem(eventBus);
            ServiceLocator.Register<GoalSystem>(goalSystem);

            var scoreSystem = new ScoreSystem(eventBus);
            ServiceLocator.Register<IScoreSystem>(scoreSystem);
        }

        private void RegisterMonoServices()
        {
            var registerMethod = typeof(ServiceLocator).GetMethod("Register");

            foreach (var mb in _monoServices)
            {
                if (!mb || !mb.enabled || mb is not IService service)
                    continue;

                var concreteType = service.GetType();
                var interfaces = concreteType.GetInterfaces();

                bool wasRegistered = false;
                foreach (var interfaceType in interfaces)
                {
                    if (typeof(IService).IsAssignableFrom(interfaceType) && interfaceType != typeof(IService))
                    {
                        Debug.Log($"Registering service '{concreteType.Name}' as interface '{interfaceType.Name}'");

                        registerMethod?.MakeGenericMethod(interfaceType)
                                      .Invoke(null, new object[] { service, false });

                        wasRegistered = true;
                    }
                }

                if (!wasRegistered)
                {
                    registerMethod?.MakeGenericMethod(concreteType)
                                .Invoke(null, new object[] { service, false });
                    Debug.LogWarning($"Service '{concreteType.Name}' implements IService but has no specific service interface (e.g., IAudioManager). It will not be accessible via ServiceLocator.");
                }
            }
        }
    }
}

