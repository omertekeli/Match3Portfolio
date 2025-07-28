namespace Match3.Scripts.Enums
{
    public enum CellTypes
{
    Empty,       // Usable cell but currently contains nothing. Can receive falling gems.
    Normal,      // Standard cell that holds a gem and participates in matches.
    Blocked,     // A destructible obstacle (e.g. crate, wood) that must be cleared.
    Locked,      // Contains a locked gem. Requires specific match or action to unlock.
    Generator,   // Entry point for new gems. Usually at the top of the board.
    Receiver,    // Accepts falling gems but cannot generate new ones.
    Unused       // Visually present but not part of gameplay. Treated as outside the board.
}
}
