using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Tiles.Scripts;
using JetBrains.Annotations;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Scripts
{
    public class GameController : MonoBehaviour
    {
        #region Public Fields

        public const int WinningMoneyDiffThreshold = 50;
        public GameObject PlayerPrefab, CoinPrefab;

        #endregion Public Fields

        #region Internal Fields

        [SerializeField]
        internal int DieSize = 6;
        internal int NumberOfDice = 2;

        #endregion Internal Fields

        #region Private Fields

        private static GameController _gameController;

        // P is a Path
        // W is Wall
        // R, S, T, U are River Tiles (Up, Down, Left, Right, respectively)
        // C is a Player Spawn Point
        //J is the Police station
        private readonly char[,] _map =
        {
            {'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W'},
            {'W', 'P', 'P', 'P', 'P', 'G', 'W', 'G', 'W', 'P', 'P', 'P', 'P', 'P', 'G', 'W'},
            {'G', 'P', 'W', 'W', 'P', 'W', 'W', 'P', 'P', 'P', 'W', 'C', 'W', 'P', 'W', 'W'},
            {'W', 'P', 'W', 'W', 'P', 'W', 'W', 'P', 'W', 'W', 'W', 'P', 'P', 'P', 'W', 'W'},
            {'U', 'U', 'U', 'U', 'U', 'U', 'U', 'U', 'S', 'P', 'P', 'P', 'W', 'W', 'W', 'W'},
            {'U', 'U', 'U', 'U', 'U', 'U', 'S', 'U', 'S', 'W', 'G', 'W', 'W', 'W', 'W', 'W'},
            {'G', 'W', 'W', 'R', 'W', 'W', 'S', 'W', 'S', 'W', 'W', 'W', 'W', 'W', 'G', 'W'},
            {'W', 'G', 'W', 'R', 'W', 'J', 'S', 'W', 'S', 'P', 'P', 'P', 'G', 'W', 'P', 'W'},
            {'W', 'P', 'W', 'R', 'W', 'W', 'S', 'W', 'S', 'W', 'W', 'P', 'W', 'W', 'P', 'W'},
            {'W', 'P', 'W', 'R', 'T', 'T', 'T', 'W', 'U', 'P', 'P', 'C', 'P', 'W', 'P', 'W'},
            {'W', 'P', 'W', 'P', 'W', 'P', 'W', 'W', 'P', 'W', 'P', 'W', 'P', 'P', 'P', 'W'},
            {'W', 'P', 'P', 'P', 'W', 'P', 'P', 'P', 'P', 'W', 'P', 'W', 'W', 'P', 'W', 'W'},
            {'W', 'W', 'C', 'W', 'P', 'P', 'W', 'W', 'P', 'W', 'P', 'P', 'P', 'P', 'W', 'W'},
            {'W', 'W', 'P', 'P', 'P', 'W', 'W', 'G', 'P', 'W', 'W', 'P', 'W', 'W', 'W', 'W'},
            {'W', 'G', 'P', 'W', 'P', 'P', 'G', 'W', 'C', 'P', 'P', 'P', 'P', 'P', 'G', 'W'},
            {'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W'}
        };

        [SerializeField] private int _activePlayerIndex;
        [SerializeField] private Canvas _cnvUi;
        private Tile[,] _gameGrid;
        [SerializeField] private int _height;
        private List<IRoundEndListener> _roundEndListeners;
        private List<ITurnListener> _turnListeners;
        [SerializeField] private GameObject[] _tilePrefabs;
        private UIController _uiController;

        [SerializeField]
        private int _width;

        #endregion Private Fields

        #region Public Properties

        public int ActivePlayerIndex { get { return _activePlayerIndex; } private set { _activePlayerIndex = value % PlayerControllers.Count; } }

        public Dictionary<TileType, List<Tile>> TilebyType { get; private set; }

        [NotNull]
        public PlayerController ActivePlayer
        {
            get { return PlayerControllers[ActivePlayerIndex]; }
            private set { _activePlayerIndex = PlayerControllers.IndexOf(value); }
        }

        public bool GameInProgress { get; set; }

        public int Height
        {
            get { return _height; }
            private set { _height = value; }
        }

        public List<PlayerController> PlayerControllers { get; private set; }
        public int PlayerMovesLeft { get; private set; }
        public int TurnNumber { get; private set; }

        public int Width
        {
            get { return _width; }
            private set { _width = value; }
        }

        #endregion Public Properties

        #region Public Methods

        public static GameController GetGameController()
        {
            return _gameController;
        }

        /// <summary>
        /// Adds the round end listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void AddRoundEndListener(IRoundEndListener listener)
        {
            _roundEndListeners.Add(listener);
        }

        /// <summary>
        /// Removes the round end listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void RemoveRoundEndListener(IRoundEndListener listener)
        {
            _roundEndListeners.Remove(listener);
        }

        public void AddTurnListener(ITurnListener listener)
        {
            _turnListeners.Add(listener);
        }

        public void RemoveTurnListener(ITurnListener listener)
        {
            _turnListeners.Remove(listener);
        }


        public Tile GetGameTile(int x, int y)
        {
            return _gameGrid[x, y];
        }

        /// <summary>
        /// Determines whether [is in bounds] [the specified position].
        /// </summary>
        /// <param name="pos">The position.</param>
        /// <returns>
        ///   <c>true</c> if [is in bounds] [the specified position]; otherwise, <c>false</c>.
        /// </returns>
        public bool IsInBounds(Vector3 pos)
        {
            return pos.x >= -0.5 &&
                   pos.y >= -0.5 &&
                   pos.x < Width - 0.5 &&
                   pos.y < Height - 0.5;
        }

        /// <summary>
        /// Nexts the turn.
        /// </summary>
        public void NextTurn()
        {
            foreach (ITurnListener listener in _turnListeners)
            {
                listener.OnTurnEnd(ActivePlayer);
            }
            ActivePlayer.OnTurnEnd(this);

            CheckIfWin();
            if (!GameInProgress) return;

            ActivePlayerIndex++;

            if (ActivePlayerIndex == 0)
            {
                TurnNumber++;
                foreach (IRoundEndListener roundEndListener in _roundEndListeners)
                {
                    roundEndListener.OnRoundEnd(TurnNumber);
                }
            }

            foreach (ITurnListener listener in _turnListeners)
            {
                listener.OnTurnStart(ActivePlayer);
            }
            ActivePlayer.OnTurnStart(this);
            _uiController.ToggleRollDice(true);
        }


        /// <summary>
        /// Rolls the dice.
        /// </summary>
        /// <param name="d">The d.</param>
        /// <returns></returns>
        public int RollDice(int d)
        {
            int rollDice = Random.Range(1, d + 1);
            return rollDice;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            GameInProgress = true;
            _gameController = this;
            _roundEndListeners = new List<IRoundEndListener>();
            _turnListeners = new List<ITurnListener>();
            _gameGrid = new Tile[Width, Height];


            TilebyType = new Dictionary<TileType, List<Tile>>();
            foreach (TileType type in Enum.GetValues(typeof(TileType)))
            {
                TilebyType.Add(type, new List<Tile>());
            }

            for (var x = 0; x <= _gameGrid.GetUpperBound(0); x++)
                for (var y = 0; y <= _gameGrid.GetUpperBound(1); y++)
                {
                    GameObject tileToMake = _tilePrefabs[0];
                    Vector2 facing = Vector2.zero;
                    TileType type;
                    switch (_map[15 - y, x])
                    {
                        case 'C':
                            type = TileType.PlayerSpawner;
                            break;

                        case 'G':
                            tileToMake = _tilePrefabs[(int)TileType.CoinSpawner];
                            type = TileType.CoinSpawner;
                            break;

                        case 'J':
                            tileToMake = _tilePrefabs[(int) TileType.Jail];
                            type = TileType.Jail;
                            break;

                        case 'W':
                            tileToMake = _tilePrefabs[(int)TileType.Wall];
                            type = TileType.Wall;
                            break;

                        case 'R':
                            tileToMake = _tilePrefabs[(int)TileType.River];
                            type = TileType.River;
                            facing = Vector2.up;
                            break;

                        case 'S':
                            tileToMake = _tilePrefabs[(int)TileType.River];
                            type = TileType.River;
                            facing = Vector2.down;
                            break;

                        case 'T':
                            tileToMake = _tilePrefabs[(int)TileType.River];
                            type = TileType.River;
                            facing = Vector2.left;
                            break;

                        case 'U':
                            tileToMake = _tilePrefabs[(int)TileType.River];
                            type = TileType.River;
                            facing = Vector2.right;
                            break;

                        default:
                            type = TileType.Path;
                            tileToMake = _tilePrefabs[(int)TileType.Path];
                            break;
                    }

                    GameObject tileInstance = Instantiate(tileToMake, new Vector3(x, y, 0), Quaternion.identity);
                    var tile = tileInstance.GetComponent<Tile>();
                    tile.Direction = facing;
                    _gameGrid[x, y] = tile;
                    TilebyType[type].Add(tile);

                    //TODO: Assign data to each tile when created, to have different tile types
                }

            PlayerControllers = new List<PlayerController>(TilebyType[TileType.PlayerSpawner].Count);
        }

        /// <summary>
        /// Checks if win.
        /// </summary>
        private void CheckIfWin()
        {
            IOrderedEnumerable<PlayerController> sortedPlayers = PlayerControllers.OrderByDescending(x => x.Money);
            PlayerController firstPlace = sortedPlayers.FirstOrDefault();
            PlayerController lastPlace = sortedPlayers.LastOrDefault();

            if (firstPlace == null || lastPlace == null) return;

            int moneyDiff = firstPlace.Money - lastPlace.Money;
            if (moneyDiff >= WinningMoneyDiffThreshold)
            {
                //TODO: What happens on win
                _uiController.ShowWinSplash(firstPlace);
                _uiController.ToggleInteraction(false);
            }
        }

        /// <summary>
        /// Checks the input.
        /// </summary>
        private void CheckInput()
        {
            // WASD control
            // We add the direction to our position,
            // this moves the character 1 unit (32 pixels)
            if (!Input.GetMouseButtonDown(0)) return;

            Vector3 mouseClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (!IsInBounds(mouseClick)) return;

            Tile tile = GetGameTile(Mathf.RoundToInt(mouseClick.x), Mathf.RoundToInt(mouseClick.y));
            if (tile.IsValidMove)
            {
                ActivePlayer.MoveAlongPath(tile.Path);
            }

            //                _uiController.OnClickRollDice();
        }

        // Use this for initialization
        /// <summary>
        /// Starts this instance.
        /// </summary>
        private void Start()
        {
            _uiController = UIController.GetUIController();
            ICollection<Tile> playerSpawnLocations = TilebyType[TileType.PlayerSpawner];
            foreach (Tile location in playerSpawnLocations)
            {
                GameObject playerInstance = Instantiate(PlayerPrefab, location.transform.position, Quaternion.identity);
                var playerController = playerInstance.GetComponent<PlayerController>();
                PlayerControllers.Add(playerController);
                playerController.Id = PlayerControllers.IndexOf(playerController) + 1;
                playerInstance.GetComponent<SpriteRenderer>().sprite = playerController.CalculateAssignedSprite();
                playerController.OnGameStart(this);
            }

            ActivePlayer.OnTurnStart(this);

            TurnNumber = 1;

            _uiController.HideWinSplash();
        }

        // Update is called once per frame
        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            CheckInput();
        }

        #endregion Private Methods
    }

}