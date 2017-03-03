using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Private Fields

    [SerializeField] private string _playerWinString = "Player {0} wins!";

    [SerializeField] private Button _btnDie1;
    [SerializeField] private Button _btnDie2;
    [SerializeField] private Button _btnRollDice;
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

    private static UIController _uiController;
    #endregion Private Fields

    #region Public Properties

    [HideInInspector] public GameController GameController { get; set; }
    public bool IsInteractable { get; set; }

    #endregion Public Properties

    #region Public Methods

    public void ToggleInteraction(bool interactable)
    {
        ToggleRollDice(interactable);
        IsInteractable = interactable;
    }

    public void ToggleButtonGlowing(Button button, bool glowing)
    {
        button.GetComponent<Outline>().enabled = glowing;
    }


    public void HideWinSplash()
    {
        _txtWinSplash.enabled = false;
    }

    /// <exception cref="Exception">Invalid dice choice</exception>
    public void OnClickChooseDice(int dieNumber)
    {
        //        CurrentPlayer.PlayerMoves = RollDice(_dieNumber)

        switch (dieNumber)
        {
            case 1:
                GameController.CurrentPlayer.PlayerMoves = Convert.ToInt32(_txtDie1.text);
                break;

            case 2:
                GameController.CurrentPlayer.PlayerMoves = Convert.ToInt32(_txtDie2.text);
                break;

            default:
                throw new Exception("Invalid dice choice");
        }
    }

    public void OnClickRollDice()
    {
        //        CurrentPlayer.PlayerMoves = RollDice(_dieNumber)
        int dieNumber = GameController.DieNumber;
        int die1 = GameController.RollDice(dieNumber);
        int die2 = GameController.RollDice(dieNumber);
        _txtDie1.text = die1.ToString();
        _txtDie2.text = die2.ToString();
        ToggleRollDice(false);
//        ToggleSelectDie(true);
        GameController.CurrentPlayer.GetAvailibleMoves(die1, die2, GameController);
    }

    public void ShowWinSplash(PlayerController winningPlayer)
    {
        _txtWinSplash.text = string.Format(_playerWinString, winningPlayer.Id);
        _txtWinSplash.enabled = true;
    }

    public bool ToggleRollDice(bool interactable)
    {
        return _btnRollDice.interactable = interactable;
    }


    public void Update()
    {
        _txtCurrentPlayer.text = GameController.CurrentPlayer.Id.ToString();
        string playerMovesLeftString = GameController.PlayerMovesLeft == -1 ? "??" : GameController.PlayerMovesLeft.ToString();
        _txtMovesLeft.text = playerMovesLeftString;
        _txtTurnNumber.text = GameController.TurnNumber.ToString();
        _scoreboardController.UpdateScoreboard(GameController.PlayerControllers);
        _scoreboardController.UpdateCurrentTurn(GameController.CurrentPlayer);
    }

    #endregion Public Methods

    #region Private Methods

    public static UIController GetUIController()
    {
        return _uiController;
    }

    private void Start()
    {
        _uiController = this;
        _scoreboardController = _pnlScoreboard.GetComponent<ScoreboardController>();
        _tutorialController = _pnlTutorial.GetComponent<TutorialController>();
    }

    #endregion Private Methods
}