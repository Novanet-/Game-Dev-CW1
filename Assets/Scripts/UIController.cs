using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class UIController : MonoBehaviour, ITurnListener
    {
        #region Private Fields

        private static UIController _uiController;
        [SerializeField]
        private Button _btnDie1;
        [SerializeField]
        private Button _btnDie2;
        [SerializeField]
        private Button _btnRollDice;
        [SerializeField]
        private string _playerWinString = "Player {0} wins!";
        [SerializeField]
        private GameObject _pnlScoreboard;
        [SerializeField]
        private GameObject _pnlTutorial;
        [SerializeField]
        private GameObject _pnlPowerupBar;
        private ScoreboardController _scoreboardController;
        private TutorialController _tutorialController;
        [SerializeField]
        private Text _txtCurrentPlayer;
        [SerializeField]
        private Text _txtDie1;
        [SerializeField]
        private Text _txtDie2;
        [SerializeField]
        private Text _txtMovesLeft;
        [SerializeField]
        private Text _txtTurnNumber;
        [SerializeField]
        private Text _txtWinSplash;


        [SerializeField]
        private Toggle[] _aiToggleArray;
        private GUIStyle guiStyleFore;
        private GUIStyle guiStyleBack;

        #endregion Private Fields

        #region Public Properties

        public bool IsInteractable { get; set; }

        #endregion Public Properties

        #region Private Properties

        [HideInInspector]
        private GameController GameController { get; set; }

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

        public void OnAICheckboxValueChange(int toggleID)
        {
            if (GameController == null) return;

            List<PlayerController> playerControllers = GameController.PlayerControllers;
            if (playerControllers.Count > toggleID)
            {
                playerControllers[toggleID].IsAI = _aiToggleArray[toggleID].isOn;
            }
        }

        /// <summary>
        /// Called when [click roll dice].
        /// </summary>
        public void OnClickRollDice()
        {
            //        ActivePlayer.PlayerMoves = RollDice(_dieNumber)
            int dieSize = GameController.DieSize;
            int numberOfDice = GameController.NumberOfDice;
            List<int> dice = new List<int>();
            for (int i = 0; i < numberOfDice; i++)
            {
                int die = GameController.RollDice(dieSize);
                dice.Add(die);
            }
            SetDice(dice);
            ToggleRollDice(false);
            //        ToggleSelectDie(true);
            GameController.ActivePlayer.GetAvailibleMoves(dice);
        }

        public void SetDice(List<int> dice)
        {
            _txtDie1.text = dice[0].ToString();
            _txtDie2.text = dice[1].ToString();
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

        void OnGUI()
        {
            if (TooltipText != "")
            {
                var x = Event.current.mousePosition.x;
                var y = Event.current.mousePosition.y;
                GUI.Label(new Rect(x - 120, y + 10, 300, 60), TooltipText, guiStyleBack);
                GUI.Label(new Rect(x - 121, y + 10, 300, 60), TooltipText, guiStyleFore);
            }
        }

        public void UpdatePowerupBar(PlayerController controller)
        {
                Debug.Log("Updateing Player "+ controller.Id + " powerups!");
            List<Powerup> powerups = controller.Powerups;
            for (int i = 0; i < powerups.Count; i++)
            {
                Powerup powerup = powerups[i];
                Debug.Log(powerup);
                    powerup.transform.position = new Vector2(GameController.Width + i, 0);
            }
        }

        public string TooltipText { get; set; }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            _uiController = this;
            TooltipText = "";
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            GameController = GameController.GetGameController();
            _scoreboardController = _pnlScoreboard.GetComponent<ScoreboardController>();
            _tutorialController = _pnlTutorial.GetComponent<TutorialController>();

            for (var i = 0; i < _aiToggleArray.Length; i++)
            {
                bool isPlayer1 = i == 0;
                _aiToggleArray[i].isOn = !isPlayer1;
                OnAICheckboxValueChange(i);
            }

            guiStyleFore = new GUIStyle();
            guiStyleFore.normal.textColor = Color.white;
            guiStyleFore.alignment = TextAnchor.UpperCenter;
            guiStyleFore.wordWrap = true;
            guiStyleBack = new GUIStyle();
            guiStyleBack.normal.textColor = Color.black;
            guiStyleBack.alignment = TextAnchor.UpperCenter;
            guiStyleBack.wordWrap = true;

            GameController.AddTurnListener(this);
        }

        public void OnTurnStart(PlayerController player)
        {
            UpdatePowerupBar(player);
        }

        public void OnTurnEnd(PlayerController player)
        {
        }

        #endregion Private Methods
    }
}