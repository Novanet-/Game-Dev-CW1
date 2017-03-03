using System;
using cakeslice;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerController : MonoBehaviour

{
    #region Private Fields

    private GameController _gameController;
    private HashSet<Tile> _glowignTiles;
    

    [SerializeField] private int _money;

    private Vector2 _tilePos;
    private Queue<Vector3> _animationPath;

    #endregion Private Fields

    #region Public Properties

    public bool CanBePushed { get; set; }
    public int Id { get; set; }

    public float speed = 3;

    public int Money
    {
        get { return _money; }
        set { _money = value; }
    }

    public int PlayerMoves { get; set; }

    #endregion Public Properties

    #region Public Methods

    public void Move(IEnumerable<Tile> path)
    {
        PlayerMoves = 1;
        Tile lastTile = null; 
        foreach (Tile tile in path)
        {
            _animationPath.Enqueue(tile.transform.position);
            lastTile = tile;
        }
        Move(lastTile);
    }

    public void Move(Tile newTile) 
    {
        Vector2 oldPos = _tilePos;
        Tile oldTile = _gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y);
        _tilePos = newTile.transform.position;
        if (newTile.CanLandOn())
        {
            PlayerMoves--;
            oldTile.CurrentPlayer = null;
            _animationPath.Enqueue(_tilePos);
            newTile.CurrentPlayer = this;
            
        }
        else
        {
            _tilePos = oldPos;
            oldTile.CurrentPlayer = this;
        }
    }

    public void OnTurnEnd(GameController gameController)
    {
        GetComponent<Outline>().enabled = false;
        if (_glowignTiles != null)
        foreach (Tile tile in _glowignTiles)
            tile.StopGlowing();
    }

    public void OnTurnStart(GameController gameController)
    {
        GetComponent<Outline>().enabled = true;
        if (IsAI)
        {
            UIController.GetUIController().OnClickRollDice();
            int move = Random.Range(0, _glowignTiles.Count);
            Tile tile = _glowignTiles.ElementAt(move);
            Move(tile.Path);
           _gameController.NextTurn();
        }
    }

    public bool IsAI { get; set; }

    public void GetAvailibleMoves(int dice1, int dice2, GameController gameController)
    {
        
    HashSet<Stack<Tile>> paths = GetPath(gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y), new Stack<Tile>(), dice1);
    paths.UnionWith(GetPath(gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y), new Stack<Tile>(), dice2));
        _glowignTiles = new HashSet<Tile>();
        foreach (Stack<Tile> path in paths)
        {
            Tile endPoint = path.Peek();
            _glowignTiles.Add(endPoint);
            endPoint.Glow();
            endPoint.Path = path.Reverse();
        }
    }

    #endregion Public Methods

    #region Private Methods

    private HashSet<Stack<Tile>> GetPath(Tile tile, Stack<Tile> path, int remainingMoves)
    {
        HashSet<Stack<Tile>> routes = new HashSet<Stack<Tile>>();
        if (remainingMoves == 0)
        {
            if (tile.CanLandOn())
            {
                path.Push(tile);
                routes.Add(path);
            }
            return routes;
        }

        remainingMoves--;
        foreach (Tile neighbour in tile.GetNeighbours())
            if (!path.Contains(neighbour))
            {
                Stack<Tile> route = new Stack<Tile>(path.Reverse());

                route.Push(tile);
                routes.UnionWith(GetPath(neighbour, route, remainingMoves));
            }
        return routes;
    }

    private void Start()
    {
        _animationPath = new Queue<Vector3>();
        _tilePos = transform.position;
        CanBePushed = true;
        _gameController = GameController.GetGameController();
        Tile tile = _gameController.GetGameTile((int)_tilePos.x, (int)_tilePos.y);

        if (tile.CanLandOn())
            tile.CurrentPlayer = this;
        else
            throw new Exception("CurrentPlayer's Starting Position is Invalid!");

        //        Id = UnityEngine.Random.Range(0, 1000000);
    }

    // Update is called once per frame
    private void Update()
    {
        if (_animationPath.Count > 0)
        {
            Vector3 currentPos = transform.position;
            Vector3 target = _animationPath.Peek();

            if (Vector3.Distance(currentPos, target) < 0.01)
            {
                _animationPath.Dequeue();
                if (_animationPath.Count > 0)
                    target = _animationPath.Peek();
                else
                    return;
            }

            float frac = speed/Vector3.Distance(currentPos, target) * Time.deltaTime;
            transform.position = Vector3.Lerp(currentPos, target, frac);


        }

    }

    #endregion Private Methods
}