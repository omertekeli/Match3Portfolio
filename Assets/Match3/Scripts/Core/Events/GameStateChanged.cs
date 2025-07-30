using Match3.Scripts.Enums;
using UnityCoreModules.Services.EventBus;

namespace Match3.Scripts.Core.Events
{
    public struct GameStateChangedEvent : IEvent
    {
        public readonly GameState NewState;

        public GameStateChangedEvent(GameState newState)
        {
            NewState = newState;
        }
    }
}