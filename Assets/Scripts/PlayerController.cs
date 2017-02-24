﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    #region Private Fields

    public int Id { get;  set; }


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

    public bool CanBePushed{ get; set; }
    public int PlayerMoves { get; set; }

    #endregion Public Properties

    #region Public Methods

    public void Move(Vector2 direction)
    {
        Vector2 oldPos = _pos;
        Tile oldTile = _gameController.GetGameTile((int) _pos.x, (int) _pos.y);
        _pos = _pos + direction;
        _pos.x = Mathf.Clamp(_pos.x, 0, _gameController.Width - 1);
        _pos.y = Mathf.Clamp(_pos.y, 0, _gameController.Height - 1);
        var newTile = _gameController.GetGameTile((int)_pos.x, (int)_pos.y);
        if (newTile.CanLandOn())
        {
            PlayerMoves--;
            oldTile.CurrentPlayer = null;
            transform.position = _pos;
            newTile.CurrentPlayer = this;
            Debug.Log("Moving to:" + _pos.x + " " + _pos.y);
        }
        else
        {
            if (direction == Vector2.zero)
                PlayerMoves--;
            _pos = oldPos;
            transform.position = _pos;
            oldTile.CurrentPlayer = this;
            Debug.Log("Staying at:" + _pos.x + " " + _pos.y);
        }


    }

    #endregion Public Methods

    #region Private Methods

    private void Start()
    {
        _pos = transform.position;
        CanBePushed = true;
        _gameController = GameController.GetGameController();
        Tile tile = _gameController.GetGameTile((int) _pos.x, (int) _pos.y);

        if (tile.CanLandOn())
        {
            tile.CurrentPlayer = this;
        }
        else
        {
            throw new Exception("CurrentPlayer's Starting Position is Invalid!");
        }

//        Id = UnityEngine.Random.Range(0, 1000000);

    }


    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private Methods

    private HashSet<Tile> _glowignTiles;
    public void OnTurnStart(GameController gameController)
    {

        _glowignTiles = GetPath(gameController.GetGameTile((int) transform.position.x, (int)transform.position.y), Vector3.zero, PlayerMoves);
        foreach (Tile tile in _glowignTiles)
        {
            tile.Glow();
        }
    }

    public void OnTurnEnd(GameController gameController)
    {
        foreach (Tile tile in _glowignTiles)
        {
            tile.StopGlowing();
        }
    }

    private HashSet<Tile> GetPath(Tile tile, Vector3 lastDirection, int remainingMoves)
    {
        HashSet<Tile> tiles = new HashSet<Tile>();
        if (remainingMoves == 0)
        {
            if (tile.CanLandOn())
            {
                tiles.Add(tile);
                return tiles;
            }
            else
            {
                return tiles;
            }
        }

        foreach (KeyValuePair<Tile, Vector3> neighbour in tile.GetNeighbours())
        {
            if (!neighbour.Value.Equals(-lastDirection))
            {
                tiles.UnionWith(GetPath(neighbour.Key, neighbour.Value, remainingMoves - 1));
            }
        }
        return tiles;
    }
}