public enum ObstacleType
{
    None,           // No obstacle
    Ice,            // Ice: breaks with each match
    Chain,          // Chain: breaks only on direct match
    Crate,          // Crate: hides gem underneath, destroyed after hit
    Rock,           // Rock: indestructible or requires special power
    DoubleIce,      // Double-layered ice: needs 2 matches to clear
    Lock,           // Lock: opens with a key or special condition
    Vines,          // Vines: can spread over time
    PortalBlocker   // Temporary block that prevents gem portal output
}
