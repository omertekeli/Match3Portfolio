using Match3.Scripts.Core;
using Match3.Scripts.Core.Events;
using Match3.Scripts.Core.Interfaces;
using Match3.Scripts.Systems.Board.Events;
using UnityCoreModules.Services.EventBus;

public class ScoreSystem : IScoreSystem
{
    #region Fields
    private readonly IEventBus _eventBus;
    private int _currentScore;
    private const int POINTS_PER_PIECE = 10;
    #endregion

    #region Properties
    public int CurrentScore => _currentScore;
    #endregion
    
    public ScoreSystem(IEventBus eventBus)
    {
        _eventBus = eventBus;
        _eventBus.Subscribe<PiecesCleared>(OnPiecesCleared);
    }

    #region Methods
    public void Initialize()
    {
        _currentScore = 0;
        _eventBus.Fire(new ScoreUpdated(_currentScore));
    }

    public void Shutdown()
    {
        _eventBus.Unsubscribe<PiecesCleared>(OnPiecesCleared);
    }

    private void OnPiecesCleared(PiecesCleared eventData)
    {
        int pointsToAdd = 0;
        foreach (var pair in eventData.ClearedPieces)
        {
            pointsToAdd += pair.Value * POINTS_PER_PIECE;
        }

        //TODO: combo calculating

        _currentScore += pointsToAdd;
        _eventBus.Fire(new ScoreUpdated(_currentScore));
    }
    #endregion
}