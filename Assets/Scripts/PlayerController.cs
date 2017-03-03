using System;
using cakeslice;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    #region Private Fields

    private GameController _gameController;
    private HashSet<Stack<Tile>> _glowignTiles;
    

    [SerializeField] private int _money;

    private Vector2 _tilePos;
    private Queue<Vector3> _animationPath;

    #endregion Private Fields

    #region Public Properties

    public bool CanBePushed { get; set; }
    public int Id { get; set; }

    public float speed = 1;

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
        foreach (Tile tile in path)
        {
            Move((Vector2)tile.transform.position - _tilePos);
        }
    }

    public void Move(Vector2 direction)
    {
        Vector2 oldPos = _tilePos;
        Tile oldTile = _gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y);
        _tilePos = _tilePos + direction;
        _tilePos.x = Mathf.Clamp(_tilePos.x, 0, _gameController.Width - 1);
        _tilePos.y = Mathf.Clamp(_tilePos.y, 0, _gameController.Height - 1);
        Tile newTile = _gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y);
        if (newTile.CanLandOn())
        {
            oldTile.CurrentPlayer = null;
            _animationPath.Enqueue(_tilePos);
            newTile.CurrentPlayer = this;
            Debug.Log("Moving to:" + _tilePos.x + " " + _tilePos.y);
            
        }
        else
        {
            _tilePos = oldPos;
            oldTile.CurrentPlayer = this;
            Debug.Log("Staying at:" + _tilePos.x + " " + _tilePos.y);
        }
    }

    public void OnTurnEnd(GameController gameController)
    {
        GetComponent<Outline>().enabled = false;
        if (_glowignTiles != null)
        foreach (Stack<Tile> paths in _glowignTiles)
            paths.Peek().StopGlowing();
    }

    public void OnTurnStart(GameController gameController)
    {
        PlayerMoves = 0;
        GetComponent<Outline>().enabled = true;
    }

    public void GetAvailibleMoves(int dice1, int dice2, GameController gameController)
    {
    _glowignTiles = GetPath(gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y), new Stack<Tile>(), dice1);
    _glowignTiles.UnionWith(GetPath(gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y), new Stack<Tile>(), dice2));
        foreach (Stack<Tile> path in _glowignTiles)
        {
            Tile endPoint = path.Peek();
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
        Debug.Log(remainingMoves);
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
                    target = _animationPath.Dequeue();
                else
                    return;
            }

            float frac = Mathf.Min(Vector3.Distance(currentPos, target), speed);
            transform.position = Vector3.Lerp(currentPos, target, frac);


        }

    }

    #endregion Private Methods
}