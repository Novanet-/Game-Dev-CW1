using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    #region Public Fields

    [HideInInspector] public GameController GameController { get; set; }

    #endregion Public Fields

    #region Private Fields

    [SerializeField] private GameObject _pnlScoreboard;
    private ScoreboardController _scoreboardController;
    [SerializeField] private Text _txtCurrentPlayer;
    [SerializeField] private Text _txtDie1;
    [SerializeField] private Text _txtDie2;

    [SerializeField] private Button _btnRollDice;
    [SerializeField] private Button _btnDie1;
    [SerializeField] private Button _btnDie2;
    [SerializeField] private Text _txtMovesLeft;
    [SerializeField] private Text _txtTurnNumber;

    #endregion Private Fields

    #region Public Methods

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
        }
        ToggleSelectDie(false);
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
        ToggleSelectDie(true);
        GameController.CurrentPlayer.GetAvailibleMoves(die1, die2, GameController);
    }

    public void UpdateUI(GameController gameController)
    {
        _txtCurrentPlayer.text = gameController.CurrentPlayer.Id.ToString();
        string playerMovesLeftString = gameController.PlayerMovesLeft == -1 ? "??" : gameController.PlayerMovesLeft.ToString();
        _txtMovesLeft.text = playerMovesLeftString;
        _txtTurnNumber.text = gameController.TurnNumber.ToString();
        _scoreboardController.UpdateScoreboard(gameController.PlayerControllers);
        _scoreboardController.UpdateCurrentTurn(gameController.CurrentPlayer);
    }

    #endregion Public Methods

    #region Private Methods

    public bool ToggleRollDice(bool buttonEnabled)
    {
        return _btnRollDice.interactable = buttonEnabled;
    }

    public bool ToggleSelectDie(bool buttonEnabled)
    {
        _btnDie1.interactable = buttonEnabled;
        return _btnDie2.interactable = buttonEnabled;
    }

    private void Start()
    {
        _scoreboardController = _pnlScoreboard.GetComponent<ScoreboardController>();
        ToggleSelectDie(false);
    }

    #endregion Private Methods
}