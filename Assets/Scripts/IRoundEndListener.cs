namespace Assets.Scripts
{
    public interface IRoundEndListener
    {
        #region Public Methods

        /// <summary>
        /// Called when [round end].
        /// </summary>
        /// <param name="roundNumber">The round number.</param>
        void OnRoundEnd(int roundNumber);

        #endregion Public Methods
    }
}