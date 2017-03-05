using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Tiles.Scripts;
using cakeslice;
using UnityEngine;

namespace Assets.Scripts
{
    public class PlayerController : MonoBehaviour

    {
        #region Public Fields

        public float speed = 3;

        #endregion Public Fields


        #region Private Fields

        private Queue<Vector3> _animationPath;
        private GameController _gameController;
        private HashSet<Tile> _glowingTiles;

        [SerializeField] private int _money;

        private Vector2 _tilePos;

        #endregion Private Fields


        #region Public Properties

        public bool CanBePushed { get; set; }
        public int Id { get; set; }
        public bool IsAI { get; set; }
        public int Money { get { return _money; } set { _money = value; } }
        public int PlayerMoves { get; set; }

        #endregion Public Properties


        #region Private Properties

        private bool IsMyTurn { get; set; }

        #endregion Private Properties


        #region Public Methods

        /// <summary>
        /// Gets the availible moves.
        /// </summary>
        /// <param name="dice1">The dice1.</param>
        /// <param name="dice2">The dice2.</param>
        public void GetAvailibleMoves(int dice1, int dice2)
        {
            GameController gameController = GameController.GetGameController();

            HashSet<Stack<Tile>> paths = GetPath(gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y), new Stack<Tile>(), dice1);
            paths.UnionWith(GetPath(gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y), new Stack<Tile>(), dice2));
            _glowingTiles = new HashSet<Tile>();

            foreach (Stack<Tile> path in paths) { GlowPathEndpoints(path); }
        }

        /// <summary>
        /// Moves the along path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void MoveAlongPath(IEnumerable<Tile> path)
        {
            PlayerMoves = 1;
            Tile lastTile = null;
            foreach (Tile tile in path)
            {
                _animationPath.Enqueue(tile.transform.position);
                lastTile = tile;
            }

            MoveToTile(lastTile);
        }

        /// <summary>
        /// Moves to tile.
        /// </summary>
        /// <param name="newTile">The new tile.</param>
        public void MoveToTile(Tile newTile)
        {
            GameController gameController = GameController.GetGameController();

            Vector2 oldPos = _tilePos;
            Tile oldTile = gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y);
            _tilePos = newTile.transform.position;

            if (newTile.CanLandOn()) { MovePlayer(oldTile, newTile); }
            else
            {
                _tilePos = oldPos;
                oldTile.CurrentPlayer = this;
            }
        }

        /// <summary>
        /// Called when [turn end].
        /// </summary>
        /// <param name="gameController">The game controller.</param>
        public void OnTurnEnd(GameController gameController)
        {
            IsMyTurn = false;
            GetComponent<Outline>().enabled = false;
            if (_glowingTiles == null) return;

            foreach (Tile tile in _glowingTiles) { tile.StopGlowing(); }
        }

        /// <summary>
        /// Called when [turn start].
        /// </summary>
        /// <param name="gameController">The game controller.</param>
        public void OnTurnStart(GameController gameController)
        {
            GetComponent<Outline>().enabled = true;
            IsMyTurn = true;
        }

        #endregion Public Methods


        #region Private Methods

        /// <summary>
        /// Gets the path.
        /// </summary>
        /// <param name="tile">The tile.</param>
        /// <param name="path">The path.</param>
        /// <param name="remainingMoves">The remaining moves.</param>
        /// <returns></returns>
        private static HashSet<Stack<Tile>> GetPath(Tile tile, Stack<Tile> path, int remainingMoves)
        {
            var routes = new HashSet<Stack<Tile>>();
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
            {
                if (!path.Contains(neighbour))
                {
                    var route = new Stack<Tile>(path.Reverse());

                    route.Push(tile);
                    routes.UnionWith(GetPath(neighbour, route, remainingMoves));
                }
            }

            return routes;
        }

        /// <summary>
        /// Animates the player.
        /// </summary>
        private void AnimatePlayer()
        {
            Vector3 currentPos = transform.position;
            Vector3 target = _animationPath.Peek();

            if (Vector3.Distance(currentPos, target) < 0.01)
            {
                _animationPath.Dequeue();
                if (_animationPath.Count > 0) target = _animationPath.Peek();
                else return;
            }

            float frac = speed / Vector3.Distance(currentPos, target) * Time.deltaTime;
            transform.position = Vector3.Lerp(currentPos, target, frac);
        }

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            _animationPath = new Queue<Vector3>();
            _tilePos = transform.position;
            CanBePushed = true;
        }

        /// <summary>
        /// Gets the best move.
        /// </summary>
        /// <returns></returns>
        private Tile GetBestMove()
        {
            float highestHeat = 0;
            Tile moveTo = null;
            foreach (Tile tile in _glowingTiles)
            {
                float heat = tile.GetGoldHeat();
                if (heat > highestHeat)
                {
                    highestHeat = heat;
                    moveTo = tile;
                }
            }

            return moveTo;
        }

        /// <summary>
        /// Glows the path endpoints.
        /// </summary>
        /// <param name="path">The path.</param>
        private void GlowPathEndpoints(Stack<Tile> path)
        {
            Tile endPoint = path.Peek();
            _glowingTiles.Add(endPoint);
            endPoint.Glow();
            endPoint.Path = path.Reverse();
        }

        /// <summary>
        /// Moves the ai.
        /// </summary>
        private void MoveAI()
        {
            UIController.GetUIController().OnClickRollDice();
            Tile bestMove = GetBestMove();

            MoveAlongPath(bestMove.Path);
            GameController.GetGameController().NextTurn();
        }

        /// <summary>
        /// Moves the player.
        /// </summary>
        /// <param name="oldTile">The old tile.</param>
        /// <param name="newTile">The new tile.</param>
        private void MovePlayer(Tile oldTile, Tile newTile)
        {
            PlayerMoves--;
            oldTile.CurrentPlayer = null;
            _animationPath.Enqueue(_tilePos);
            newTile.CurrentPlayer = this;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.Exception">CurrentPlayer's Starting Position is Invalid!</exception>
        private void Start()
        {
            _gameController = GameController.GetGameController();
            Tile tile = _gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y);

            if (tile.CanLandOn()) tile.CurrentPlayer = this;
            else throw new Exception("CurrentPlayer's Starting Position is Invalid!");
        }

        // Update is called once per frame
        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            if (IsAI && IsMyTurn && _animationPath.Count == 0) MoveAI();

            if (_animationPath.Count <= 0) return;

            AnimatePlayer();
        }

        #endregion Private Methods
    }
}