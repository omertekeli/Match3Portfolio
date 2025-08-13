using UnityCoreModules.Services.EventBus;

public struct LevelEnd : IEvent
{
    public readonly bool WasWon;
    public LevelEnd(bool wasWon)
    {
        WasWon = wasWon;
    }
}