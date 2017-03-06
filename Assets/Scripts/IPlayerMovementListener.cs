namespace Assets.Scripts
{
    public interface IPlayerMovementListener
    {
        #region Public Methods

        /// <summary>
        /// Players the lands on.
        /// </summary>
        /// <param name="player">The player.</param>
        void PlayerLandsOn(PlayerController player);

        /// <summary>
        /// Players the leaves.
        /// </summary>
        /// <param name="player">The player.</param>
        void PlayerLeaves(PlayerController player);

        /// <summary>
        /// Players the remains on.
        /// </summary>
        /// <param name="player">The player.</param>
        void PlayerRemainsOn(PlayerController player);

        #endregion Public Methods
    }
}