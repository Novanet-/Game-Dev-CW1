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

        public float speed;

        #endregion Public Fields

        #region Private Fields

        private Queue<Vector3> _animationPath;
        private GameController _gameController;
        private HashSet<Tile> _glowingTiles;

        [SerializeField] private int _money;
        [SerializeField] private Sprite[] _spriteTextures;

        private Vector2 _tilePos;
        private AudioController _audioController = AudioController.GetAudioController();

        #endregion Private Fields

        #region Public Properties

        public bool CanBePushed { get; set; }
        public int Id { get; set; }
        public bool IsAI { get; set; }
        public int Money
        {
            get { return _money; }
            set { _money = Math.Max(value, 0); }
        }

        public List<Powerup> Powerups;
        private bool _disabled;
        //public int PlayerMoves { get; set; }

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
            foreach (Stack<Tile> path in paths)
            {
                GlowPathEndpoints(path);
            }
            Stack<Tile> doNothing = new Stack<Tile>();
            doNothing.Push(GetCurrentTile());
            GlowPathEndpoints(doNothing);
        }

        /// <summary>
        /// Moves the along path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void MoveAlongPath(IEnumerable<Tile> path)
        {
            HaveMoved = true;
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

            if (newTile.CanLandOn())
            {
                MovePlayer(oldTile, newTile);
            }
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

            foreach (Tile tile in _glowingTiles)
            {
                tile.StopGlowing();
            }
        }

        /// <summary>
        /// Called when [turn start].
        /// </summary>
        /// <param name="gameController">The game controller.</param>
        public void OnTurnStart(GameController gameController)
        {
            GetComponent<Outline>().enabled = true;
            CanBePushed = true;
            HaveMoved = false;
            IsMyTurn = true;

            if (Disabled)
            {
                Disabled = false;
                EndTurn();
            }

            CoinSpawnerController coinSpawner = GetCurrentTile() as CoinSpawnerController;
            if (coinSpawner != null && coinSpawner.GetGoldAmount() > 0)
            {
                StayStill();
                Debug.Log("Player " + Id + " is still stealing gold");  
            }
        }

        private void EndTurn()
        {
            _gameController.NextTurn();
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
                Tile tile = _gameController.GetGameTile((int) currentPos.x, (int) currentPos.y);
                tile.PlayerMovedOver(this, _animationPath.Count == 0);
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
            Powerups = new List<Powerup>();
        }

        public Tile GetCurrentTile()
        {
            return _gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y);
        }

        /// <summary>
        /// Gets the best move.
        /// </summary>
        /// <returns></returns>
        private Tile GetBestMove()
        {
            Tile moveTo = null;
            float highestHeat = 0;
            foreach (Tile tile in _glowingTiles)
            {
                Tile destinationTile = tile;
                if (tile.GetType() == typeof(River))
                {
                    River river = (River) tile;
                    Tile finalTile = river.DestinationTile;
                    if (finalTile.IsValidMove) destinationTile = finalTile;
                }
                float heat = destinationTile.GetGoldHeat(this);
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
            endPoint.Path = path.Reverse();
            if (!IsAI)
                endPoint.SetValidMove();
        }

        /// <summary>
        /// Moves the ai.
        /// </summary>
        private void MoveAI()
        {
            UIController.GetUIController().OnClickRollDice();
            Tile bestMove = GetBestMove();

            bestMove.Glow();
            MoveAlongPath(bestMove.Path);
        }

        /// <summary>
        /// Moves the player.
        /// </summary>
        /// <param name="oldTile">The old tile.</param>
        /// <param name="newTile">The new tile.</param>
        private void MovePlayer(Tile oldTile, Tile newTile)
        {
            HaveMoved = true;
            oldTile.CurrentPlayer = null;
            _animationPath.Enqueue(_tilePos);
            newTile.CurrentPlayer = this;
        }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        /// <exception cref="System.Exception">ActivePlayer's Starting Position is Invalid!</exception>
        private void Start()
        {
        }

        // Update is called once per frame
        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            if (_animationPath.Count > 0)
                AnimatePlayer();

            if (IsMyTurn)
            {
                if (HaveMoved)
                {
                    if (_animationPath.Count == 0)
                    {
                        _audioController.PlaySoundOnce(_audioController.MoveSound, 0.2f);
                        EndTurn();
                    }
                }
                else
                {
                    if (IsAI)
                        MoveAI();
                }
            }
        }

        public bool HaveMoved { get; set; }

        #endregion Private Methods

        public void OnGameStart(GameController gameController)
        {
            Disabled = false;
            IsMyTurn = false;
            HaveMoved = false;
            GetComponent<Outline>().enabled = false;
            if (Id > 1)
            {
                IsAI = true;
            }
            else IsAI = false;
            _gameController = GameController.GetGameController();
            Tile tile = _gameController.GetGameTile((int) _tilePos.x, (int) _tilePos.y);

            if (tile.CanLandOn()) tile.CurrentPlayer = this;
            else throw new Exception("ActivePlayer's Starting Position is Invalid!");
        }

        public void StayStill()
        {
            MovePlayer(GetCurrentTile(), GetCurrentTile());
        }

        public Sprite CalculateAssignedSprite()
        {
            return _spriteTextures[this.Id - 1];
        }


        public bool Disabled
        {
            get { return _disabled; }
            set
            {
                SpriteRenderer renderer = GetComponent<SpriteRenderer>();
                if (value)
                {

                    Debug.Log("Player " + Id + " is disabled!");
                    renderer.color = Color.black;
                    CoinSpawnerController coinSpawner = GetCurrentTile() as CoinSpawnerController;
                    if (coinSpawner != null) 
                        coinSpawner.EmptyOfGold();
                    Money = Money - 20;
                }
                else
                {
                    Debug.Log("Player " + Id + " is not disabled!");
                    renderer.color = Color.white;

                }
                _disabled = value;
            }
        }
    }
}