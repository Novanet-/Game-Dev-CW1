using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameController : MonoBehaviour
{

    #region Public Fields

    public GameObject PlayerPrefab, CoinPrefab;

    #endregion Public Fields

    #region Internal Fields

    [SerializeField]
    internal int DieNumber = 6;

    #endregion Internal Fields

    #region Private Fields

    private const int Gold = 3;
    private static GameController _gameController;

    public static GameController GetGameController()
    {
        return _gameController;
    }


    private const int Path = 0;

    private const int River = 2;

    private const int Wall = 1;

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

    [SerializeField]
    private int _activePlayerIndex;
    [SerializeField]
    private int _height;
    [SerializeField]
    private PlayerController[] _playerControllers;
    private List<RoundEndListener> _roundEndListeners;
    [SerializeField]
    private GameObject[] _tilePrefabs;
    [SerializeField]
    private Canvas _ui;
    [SerializeField] private int _dieNumber = 6;
    [SerializeField]
    private int _activePlayerIndex;
    private UIController _uiController;
    [SerializeField] private int _width;

    //    public Transform GameBoard;
    private Tile[,] GameGrid;

    #endregion Private Fields

    #region Public Properties

    public int ActivePlayerIndex
    {
        get { return _activePlayerIndex; }
        private set { _activePlayerIndex = value % PlayerControllers.Length; }
    }

    public PlayerController CurrentPlayer
    {
        get { return PlayerControllers[ActivePlayerIndex]; }
        private set
        {
            for (int i = 0; i < PlayerControllers.Length; i++)
            {
                if (value == PlayerControllers[i])
                {
                    _activePlayerIndex = i;
                    return;
                }
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

    public PlayerController[] PlayerControllers { get; private set; }
    public int PlayerMovesLeft { get; private set; }
    public int TurnNumber { get; private set; }
    public int Width
    {
        get { return _width; }
        private set { _width = value; }
    }

    #endregion Public Properties

    #region Public Methods

    public void AddRoundEndListener(RoundEndListener listener)
    {
        _roundEndListeners.Add(listener);
    }

    public Tile GetGameTile(int x, int y)
    {
        return GameGrid[x, y];
    }

    public void RemoveRoundEndListener(RoundEndListener listener)
    {
        _roundEndListeners.Remove(listener);
    }

    public int RollDice(int d)
    {
        int rollDice = UnityEngine.Random.Range(1, d + 1);
        Debug.Log(string.Format("Rolled a {0}", rollDice));
        return rollDice;
    }

    #endregion Public Methods

    #region Private Methods

    private void CheckInput()
    {
        // WASD control
        // We add the direction to our position,
        // this moves the character 1 unit (32 pixels)
        PlayerMovesLeft = CurrentPlayer.PlayerMoves;

        if (Input.anyKey)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                CurrentPlayer.Move(Vector2.zero);
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                CurrentPlayer.Move(Vector2.right);
            else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                CurrentPlayer.Move(Vector2.left);
            else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                CurrentPlayer.Move(Vector2.up);
            else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                CurrentPlayer.Move(Vector2.down);


            if (CurrentPlayer.PlayerMoves == 0)
            {
                NextTurn();
                CurrentPlayer = GetActivePlayer();
                _uiController.ToggleRollDice(true);
                _uiController.ToggleSelectDie(false);
                CurrentPlayer.PlayerMoves = -1;
//                _uiController.OnClickRollDice();
            }
        }
    }

    private PlayerController GetActivePlayer()
    {
        return PlayerControllers[ActivePlayerIndex];
    }

    private void NextTurn()
    {
        CurrentPlayer.OnTurnEnd(this);
        ActivePlayerIndex++; 
        if (ActivePlayerIndex == 0)
        {
            TurnNumber++;
            foreach (RoundEndListener roundEndListener in _roundEndListeners)
            {
                roundEndListener.OnRoundEnd(TurnNumber);
            }
        }
        CurrentPlayer.PlayerMoves = RollDice(_dieNumber);
        CurrentPlayer.OnTurnStart(this);

    }


    // Use this for initialization
    private void Start()
    {
        GameController._gameController = this;
        _roundEndListeners = new List<RoundEndListener>();
        GameGrid = new Tile[Width, Height];
        List<KeyValuePair<int, int>> playerSpawnLocations = new List<KeyValuePair<int, int>>();


        for (var x = 0; x <= GameGrid.GetUpperBound(0); x++)
        for (var y = 0; y <= GameGrid.GetUpperBound(1); y++)
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
            Tile tile = tileInstance.GetComponent<Tile>();
            tile.Direction = facing;
            GameGrid[x, y] = tile;

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