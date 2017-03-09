using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class ScoreboardController : MonoBehaviour
    {
        #region Private Fields

        [SerializeField] private Text[] _txtCurrentPlayerIndicatorArray;
        [SerializeField] private Text[] _txtPlayerScoresArray;

        #endregion Private Fields


        #region Public Methods

        /// <summary>
        /// Updates the current turn.
        /// </summary>
        /// <param name="currentPlayer">The current player.</param>
        public void UpdateCurrentTurn(PlayerController currentPlayer)
        {
            for (var i = 0; i < _txtCurrentPlayerIndicatorArray.Length; i++) {
                _txtCurrentPlayerIndicatorArray[i].enabled = Equals(i + 1, currentPlayer.Id);
            }
        }

        /// <summary>
        /// Updates the scoreboard.
        /// </summary>
        /// <param name="players">The players.</param>
        public void UpdateScoreboard(List<PlayerController> players)
        {
            for (var i = 0; i < players.Count; i++) { _txtPlayerScoresArray[i].text = players[i].Money.ToString(); }
        }

        #endregion Public Methods


        #region Private Methods

        // Use this for initialization
        private void Start() { }

        // Update is called once per frame
        private void Update() { }

        #endregion Private Methods
    }
}