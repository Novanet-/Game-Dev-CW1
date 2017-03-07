using System;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIController : MonoBehaviour
    {
        #region Private Fields

        private static UIController _uiController;
        [SerializeField] private Button _btnDie1;
        [SerializeField] private Button _btnDie2;
        [SerializeField] private Button _btnRollDice;
        [SerializeField] private string _playerWinString = "Player {0} wins!";
        [SerializeField] private GameObject _pnlScoreboard;
        [SerializeField] private GameObject _pnlTutorial;
        private ScoreboardController _scoreboardController;
        private TutorialController _tutorialController;
        [SerializeField] private Text _txtCurrentPlayer;
        [SerializeField] private Text _txtDie1;
        [SerializeField] private Text _txtDie2;
        [SerializeField] private Text _txtMovesLeft;
        [SerializeField] private Text _txtTurnNumber;
        [SerializeField] private Text _txtWinSplash;

        #endregion Private Fields

        #region Public Properties

        public bool IsInteractable { get; set; }

        #endregion Public Properties

        #region Private Properties

        [HideInInspector] private GameController GameController { get; set; }

        #endregion Private Properties

        #region Public Methods

        /// <summary>
        /// Gets the UI controller.
        /// </summary>
        /// <returns></returns>
        public static UIController GetUIController()
        {
            return _uiController;
        }

        public void HideWinSplash()
        {
            _txtWinSplash.enabled = false;
        }

        /// <summary>
        /// Called when [click choose dice].
        /// </summary>
        /// <param name="dieNumber">The die number.</param>
        /// <exception cref="System.Exception">Invalid dice choice</exception>
        /// <exception cref="Exception">Invalid dice choice</exception>
        public void OnClickChooseDice(int dieNumber)
        {
            //        ActivePlayer.PlayerMoves = RollDice(_dieNumber)
        }

        /// <summary>
        /// Called when [click roll dice].
        /// </summary>
        public void OnClickRollDice()
        {
            //        ActivePlayer.PlayerMoves = RollDice(_dieNumber)
            int dieNumber = GameController.DieNumber;
            int die1 = GameController.RollDice(dieNumber);
            int die2 = GameController.RollDice(dieNumber);
            _txtDie1.text = die1.ToString();
            _txtDie2.text = die2.ToString();
            ToggleRollDice(false);
            //        ToggleSelectDie(true);
            GameController.ActivePlayer.GetAvailibleMoves(die1, die2);
        }

        /// <summary>
        /// Called when [click roll dice].
        /// </summary>
        public void OnClickEndTurn()
        {
            GameController.ActivePlayer.StayStill();
            GameController.NextTurn();
        }

        /// <summary>
        /// Shows the win splash.
        /// </summary>
        /// <param name="winningPlayer">The winning player.</param>
        public void ShowWinSplash(PlayerController winningPlayer)
        {
            _txtWinSplash.text = string.Format(_playerWinString, winningPlayer.Id);
            _txtWinSplash.enabled = true;
        }

        /// <summary>
        /// Toggles the button glowing.
        /// </summary>
        /// <param name="button">The button.</param>
        /// <param name="glowing">if set to <c>true</c> [glowing].</param>
        public void ToggleButtonGlowing(Button button, bool glowing)
        {
            ColorBlock oldColour = button.colors;
            ColorBlock newColour = oldColour;
            if (glowing)
            {
                newColour.colorMultiplier = 2;
                button.colors = newColour;
            }
            else
            {
                newColour.colorMultiplier = 1;
                button.colors = newColour;
            }
        }

        /// <summary>
        /// Toggles the interaction.
        /// </summary>
        /// <param name="interactable">if set to <c>true</c> [interactable].</param>
        public void ToggleInteraction(bool interactable)
        {
            ToggleRollDice(interactable);
            IsInteractable = interactable;
        }

        /// <summary>
        /// Toggles the roll dice.
        /// </summary>
        /// <param name="interactable">if set to <c>true</c> [interactable].</param>
        /// <returns></returns>
        public bool ToggleRollDice(bool interactable)
        {
            return _btnRollDice.interactable = interactable;
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        public void Update()
        {
            _txtCurrentPlayer.text = GameController.ActivePlayer.Id.ToString();
            string playerMovesLeftString = GameController.PlayerMovesLeft == -1 ? "??" : GameController.PlayerMovesLeft.ToString();
            _txtMovesLeft.text = playerMovesLeftString;
            _txtTurnNumber.text = GameController.TurnNumber.ToString();
            _scoreboardController.UpdateScoreboard(GameController.PlayerControllers);
            _scoreboardController.UpdateCurrentTurn(GameController.ActivePlayer);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            _uiController = this;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            GameController = GameController.GetGameController();
            _scoreboardController = _pnlScoreboard.GetComponent<ScoreboardController>();
            _tutorialController = _pnlTutorial.GetComponent<TutorialController>();
        }

        #endregion Private Methods
    }
}