using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Public Fields

    public const int WinningMoneyDiffThreshold = 20;
    public GameObject PlayerPrefab, CoinPrefab;

    #endregion Public Fields

    #region Internal Fields

    [SerializeField] internal int DieNumber = 6;

    #endregion Internal Fields

    #region Private Fields

    private const int Gold = 3;
    private const int Path = 0;
    private const int River = 2;
    private const int Wall = 1;
    private static GameController _gameController;

    // P is a Path
    // W is Wall
    // R, S, T, U are River Tiles (Up, Down, Left, Right, respectively)
    // C is a Player Spawn Point
    private readonly char[,] _map =
    {
        {'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'G', 'W', 'G', 'W', 'P', 'P', 'P', 'P', 'P', 'G', 'W'},
        {'G', 'P', 'W', 'W', 'P', 'W', 'W', 'P', 'P', 'P', 'W', 'C', 'W', 'P', 'W', 'W'},
        {'W', 'P', 'W', 'W', 'P', 'W', 'W', 'P', 'W', 'W', 'W', 'P', 'P', 'P', 'W', 'W'},
        {'U', 'U', 'U', 'U', 'U', 'U', 'U', 'U', 'S', 'P', 'P', 'P', 'W', 'W', 'W', 'W'},
        {'U', 'U', 'U', 'U', 'U', 'U', 'S', 'U', 'S', 'W', 'G', 'W', 'W', 'W', 'W', 'W'},
        {'W', 'W', 'G', 'R', 'W', 'W', 'S', 'W', 'S', 'W', 'W', 'W', 'W', 'W', 'G', 'W'},
        {'W', 'G', 'W', 'R', 'W', 'G', 'S', 'W', 'S', 'P', 'P', 'P', 'G', 'W', 'P', 'W'},
        {'W', 'P', 'W', 'R', 'W', 'W', 'S', 'W', 'S', 'W', 'W', 'P', 'W', 'W', 'P', 'W'},
        {'W', 'P', 'W', 'R', 'S', 'T', 'T', 'W', 'U', 'P', 'P', 'C', 'P', 'W', 'P', 'W'},
        {'W', 'P', 'W', 'P', 'W', 'P', 'W', 'W', 'P', 'W', 'P', 'W', 'P', 'P', 'P', 'W'},
        {'W', 'P', 'P', 'P', 'W', 'P', 'P', 'P', 'P', 'W', 'P', 'W', 'W', 'P', 'W', 'W'},
        {'W', 'W', 'C', 'W', 'P', 'P', 'W', 'W', 'P', 'W', 'P', 'P', 'P', 'P', 'W', 'W'},
        {'W', 'W', 'P', 'P', 'P', 'W', 'W', 'G', 'P', 'W', 'W', 'P', 'W', 'W', 'W', 'W'},
        {'W', 'G', 'P', 'W', 'P', 'P', 'G', 'W', 'C', 'P', 'P', 'P', 'P', 'P', 'G', 'W'},
        {'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W'}
    };

    [SerializeField] private int _activePlayerIndex;
    [SerializeField] private int _dieNumber = 6;

    private Tile[,] _gameGrid;
    [SerializeField] private int _height;
    [SerializeField] private int _width;

    private List<RoundEndListener> _roundEndListeners;
    [SerializeField] private GameObject[] _tilePrefabs;

    [SerializeField] private Canvas _ui;
    private UIController _uiController;

    #endregion Private Fields

    #region Public Properties

    public int ActivePlayerIndex
    {
        get { return _activePlayerIndex; }
        private set { _activePlayerIndex = value % PlayerControllers.Length; }
    }

    [NotNull] public PlayerController CurrentPlayer
    {
        get { return PlayerControllers[ActivePlayerIndex]; }
        private set
        {
            for (var i = 0; i < PlayerControllers.Length; i++)
                if (value == PlayerControllers[i])
                {
                    _activePlayerIndex = i;
                    return;
                }
        }
    }

    public PlayerController[] PlayerControllers { get; private set; }
    public int PlayerMovesLeft { get; private set; }
    public int TurnNumber { get; private set; }

    public int Height
    {
        get { return _height; }
        private set { _height = value; }
    }

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

    public void AddRoundEndListener(RoundEndListener listener)
    {
        _roundEndListeners.Add(listener);
    }

    public Tile GetGameTile(int x, int y)
    {
        return _gameGrid[x, y];
    }

    public bool IsInBounds(Vector3 pos)
    {
        return pos.x >= 0 &&
               pos.y >= 0 &&
               pos.x < Width &&
               pos.y < Height;
    }

    public void RemoveRoundEndListener(RoundEndListener listener)
    {
        _roundEndListeners.Remove(listener);
    }

    public int RollDice(int d)
    {
        int rollDice = Random.Range(1, d + 1);
        Debug.Log(string.Format("Rolled a {0}", rollDice));
        return rollDice;
    }

    #endregion Public Methods

    #region Private Methods

    private void CheckIfWin()
    {
        IOrderedEnumerable<PlayerController> sortedPlayers = PlayerControllers.OrderByDescending(x => x.Money);
        PlayerController firstPlace = sortedPlayers.FirstOrDefault();
        PlayerController lastPlace = sortedPlayers.LastOrDefault();

        if (firstPlace == null || lastPlace == null) return;

        int moneyDiff = firstPlace.Money - lastPlace.Money;
        if (moneyDiff > WinningMoneyDiffThreshold)
        {
            //TODO: What happens on win
        }
    }

    private void CheckInput()
    {
        // WASD control
        // We add the direction to our position,
        // this moves the character 1 unit (32 pixels)
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mouseClick = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (IsInBounds(mouseClick))
            {
                Tile tile = GetGameTile(Mathf.RoundToInt(mouseClick.x), Mathf.RoundToInt(mouseClick.y));
                if (tile.IsValidMove)
                {
                    CurrentPlayer.Move(tile);
                    NextTurn();
                    _uiController.ToggleRollDice(true);
                    _uiController.ToggleSelectDie(false);
                    CheckIfWin();
                }
            }
        }

        //                _uiController.OnClickRollDice();
    }

    private void NextTurn()
    {
        CurrentPlayer.OnTurnEnd(this);
        ActivePlayerIndex++;
        if (ActivePlayerIndex == 0)
        {
            TurnNumber++;
            foreach (RoundEndListener roundEndListener in _roundEndListeners)
                roundEndListener.OnRoundEnd(TurnNumber);
        }

        CurrentPlayer.OnTurnStart(this);
    }

    // Use this for initialization
    private void Start()
    {
        _gameController = this;
        _roundEndListeners = new List<RoundEndListener>();
        _gameGrid = new Tile[Width, Height];
        var playerSpawnLocations = new List<KeyValuePair<int, int>>();

        for (var x = 0; x <= _gameGrid.GetUpperBound(0); x++)
        for (var y = 0; y <= _gameGrid.GetUpperBound(1); y++)
        {
            GameObject tileToMake = _tilePrefabs[0];
            Vector2 facing = Vector2.zero;
            switch (_map[15 - y, x])
            {
                case 'C':
                    playerSpawnLocations.Add(new KeyValuePair<int, int>(x, y));
                    break;

                case 'G':
                    tileToMake = _tilePrefabs[Gold];
                    break;

                case 'W':
                    tileToMake = _tilePrefabs[Wall];
                    break;

                case 'R':
                    tileToMake = _tilePrefabs[River];
                    facing = Vector2.up;
                    break;

                case 'S':
                    tileToMake = _tilePrefabs[River];
                    facing = Vector2.down;
                    break;

                case 'T':
                    tileToMake = _tilePrefabs[River];
                    facing = Vector2.left;
                    break;

                case 'U':
                    tileToMake = _tilePrefabs[River];
                    facing = Vector2.right;
                    break;

                default:
                    tileToMake = _tilePrefabs[Path];
                    break;
            }

            GameObject tileInstance = Instantiate(tileToMake, new Vector3(x, y, 0), Quaternion.identity);
            var tile = tileInstance.GetComponent<Tile>();
            tile.Direction = facing;
            _gameGrid[x, y] = tile;

            _uiController = _ui.GetComponent<UIController>();
            _uiController.GameController = this;

            //TODO: Assign data to each tile when created, to have different tile types
        }

        PlayerControllers = new PlayerController[4];
        ActivePlayerIndex = 0;
        for (var i = 0; i < PlayerControllers.Length; i++)
        {
            int x = playerSpawnLocations[i].Key;
            int y = playerSpawnLocations[i].Value;
            GameObject playerInstance = Instantiate(PlayerPrefab, new Vector3(x, y, 0), Quaternion.identity);
            var playerController = playerInstance.GetComponent<PlayerController>();
            PlayerControllers[i] = playerController;
            playerController.Id = i + 1;
        }

        CurrentPlayer.PlayerMoves = RollDice(_dieNumber);
        CurrentPlayer.OnTurnStart(this);

        TurnNumber = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        _uiController.UpdateUI(this);
        CheckInput();
    }

    #endregion Private Methods
}