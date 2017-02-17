using JetBrains.Annotations;

public interface PlayerMovementListener
{
    void PlayerLandsOn(PlayerController player);
    void PlayerRemainsOn(PlayerController player);
    void PlayerLeaves(PlayerController player);
}