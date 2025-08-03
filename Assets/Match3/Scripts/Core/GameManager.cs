using Cysharp.Threading.Tasks;
using Match3.Scripts.Core;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Enums;
using UnityCoreModules.Services;
using UnityCoreModules.Services.EventBus;
using UnityEngine;

public class GameManager : MonoBehaviour, IService
{
    #region Fields
    private GameState _currentState;
    private IEventPublisher _publisher;
    private UIManager _uiManager;
    private ILevelManager _levelManager;
    private ISceneLoader _sceneLoader;
    #endregion

    private void Awake()
    {
        // ÖNEMLİ DÜZELTME: Eksik olan servisleri de burada alıyoruz.
        _publisher = ServiceLocator.Get<IEventPublisher>();
        _uiManager = ServiceLocator.Get<UIManager>();
        _levelManager = ServiceLocator.Get<ILevelManager>();
        _sceneLoader = ServiceLocator.Get<ISceneLoader>();
    }
    
    // UI veya diğer sistemlerden gelen istekler bu public metotları çağırır.
    public void RequestStartLevel(int levelIndex)
    {
        TransitionWorkflowAsync(GameState.Gameplay, levelIndex).Forget();
    }

    public void RequestReturnToMainMenu()
    {
        TransitionWorkflowAsync(GameState.MainMenu).Forget();
    }

    private async UniTaskVoid TransitionWorkflowAsync(GameState targetState, int levelIndex = -1)
    {
        if (_currentState == GameState.Loading || _currentState == targetState)
            return;

        try
        {
            SetState(GameState.Loading);
            await _uiManager.PlayLoadingTransitionAsync(true);

            switch (targetState)
            {
                case GameState.MainMenu:
                    await _sceneLoader.LoadSceneByIndexAsync((int)SceneIndex.MainMenu);
                    break;
                case GameState.Gameplay:
                    await _levelManager.LoadAndSetupLevelAsync(levelIndex);
                    break;
            }

            await _uiManager.PlayLoadingTransitionAsync(false);
            SetState(targetState);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to transition to state {targetState}. Error: {e.Message}\n{e.StackTrace}");
            if (_currentState != GameState.MainMenu)
            {
                await _uiManager.PlayLoadingTransitionAsync(false);
                RequestReturnToMainMenu();
                SetState(GameState.MainMenu);
            }
        }
    }

    private void SetState(GameState newState)
    {
        if (_currentState == newState) return;
        Debug.Log($"State changing to {newState}");
        _currentState = newState;
        _publisher.Fire(new GameStateChangedEvent(newState));
    }
}