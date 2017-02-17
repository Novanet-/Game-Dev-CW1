using System;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    #region Private Fields

    private GameController _gameController;

    [SerializeField] private int _money;

    private Vector2 _pos;

    #endregion Private Fields

    #region Public Properties

    public int Money
    {
        get { return _money; }
        set { _money = value; }
    }

    #endregion Public Properties

    #region Public Methods

    public void Move(Vector2 direction)
    {
        Vector2 oldPos = _pos;
        _pos = _pos + direction;
        _pos.x = Mathf.Clamp(_pos.x, 0, _gameController.Width - 1);
        _pos.y = Mathf.Clamp(_pos.y, 0, _gameController.Height - 1);
        var newTile = _gameController.GetGameTile((int)_pos.x, (int)_pos.y);
        if (newTile.CanLandOn())
        {
            _gameController.GetGameTile((int) oldPos.x, (int) oldPos.y).Player = null;
            newTile.Player = this;
            Debug.Log("Moving to:" + _pos.x + " " + _pos.y);
        }
        else
        {
            _gameController.GetGameTile((int) oldPos.x, (int) oldPos.y).Player = this;
            _pos = oldPos;
            Debug.Log("Staying at:" + _pos.x + " " + _pos.y);
        }

        transform.position = _pos;

    }

    #endregion Public Methods

    #region Private Methods

    private void Start()
    {
        _pos = transform.position;
        GameObject GameBoard = GameObject.Find("GameBoard");
        _gameController = GameBoard.GetComponent<GameController>();
        Tile tile = _gameController.GetGameTile((int) _pos.x, (int) _pos.y);
        if (tile.CanLandOn())
        {
            tile.Player = this;
        }
        else
        {
            throw new Exception("Player's Starting Position is Invalid!");
        }
    }


    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private Methods
}