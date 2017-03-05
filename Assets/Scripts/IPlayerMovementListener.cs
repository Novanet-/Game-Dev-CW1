namespace Assets.Scripts
{
    public interface IPlayerMovementListener
    {
        #region Public Methods

        void PlayerLandsOn(PlayerController player);

        void PlayerLeaves(PlayerController player);

        void PlayerRemainsOn(PlayerController player);

        #endregion Public Methods
    }
}