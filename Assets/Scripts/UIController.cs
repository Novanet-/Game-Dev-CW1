using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] private Text _txtCurrentPlayer;
    [SerializeField] private Text _txtMovesLeft;
    [SerializeField] private Text _txtTurnNumber;
    [SerializeField] private Text _txtDie1;
    [SerializeField] private Text _txtDie2;
    private ScoreboardController _scoreboard;

    [SerializeField]
    private GameObject _pnlScoreboard;

    [HideInInspector] public GameController _gameController;


    public void OnClickRollDice()
    {
//        CurrentPlayer.PlayerMoves = RollDice(_dieNumber)
        var dieNumber = _gameController._dieNumber;
        _txtDie1.text = _gameController.RollDice(dieNumber).ToString();
        _txtDie2.text = _gameController.RollDice(dieNumber).ToString();
    }

    public void OnClickChooseDice(int dieNumber)
    {
//        CurrentPlayer.PlayerMoves = RollDice(_dieNumber)

        if (dieNumber == 1)
        {
            _gameController.CurrentPlayer.PlayerMoves = Convert.ToInt32(_txtDie1.text);
        }
        else if (dieNumber == 2)
        {
            _gameController.CurrentPlayer.PlayerMoves = Convert.ToInt32(_txtDie2.text);

        }
    }

    public void UpdateUI(GameController gameController)
    {
        this._txtCurrentPlayer.text = gameController.CurrentPlayer.Id.ToString();
        this._txtMovesLeft.text = gameController.PlayerMovesLeft.ToString();
        this._txtTurnNumber.text = gameController.TurnNumber.ToString();
        this._scoreboard.UpdateScoreboard(gameController.PlayerControllers);
        this._scoreboard.UpdateCurrentTurn(gameController.CurrentPlayer);
    }

    void Start()
    {
        _scoreboard = _pnlScoreboard.GetComponent<ScoreboardController>();
    }
}

