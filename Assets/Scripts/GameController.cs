using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    #region Public Fields

    //    public Transform GameBoard;
    private Tile[,] GameGrid;

    public GameObject PlayerPrefab, CoinPrefab;

    #endregion Public Fields

    #region Private Fields

    private const int Path = 0;

    private const int River = 2;

    private const int Wall = 1;
    private const int Gold = 3;

    // P is a Path
    // W is Wall
    // R, S, T, U are River Tiles (Up, Down, Left, Right, respectively)
    private readonly char[,] _map =
    {
        {'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'G', 'W', 'W', 'W', 'P', 'P', 'P', 'P', 'P', 'G', 'W'},
        {'W', 'P', 'W', 'W', 'P', 'W', 'W', 'P', 'P', 'P', 'W', 'P', 'W', 'P', 'W', 'W'},
        {'W', 'P', 'W', 'W', 'P', 'W', 'W', 'P', 'W', 'W', 'W', 'P', 'P', 'P', 'W', 'W'},
        {'U', 'U', 'U', 'U', 'U', 'U', 'U', 'U', 'S', 'P', 'P', 'P', 'W', 'W', 'W', 'W'},
        {'U', 'U', 'U', 'U', 'U', 'U', 'S', 'U', 'S', 'W', 'W', 'W', 'W', 'W', 'W', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'S', 'W', 'S', 'W', 'W', 'W', 'W', 'W', 'G', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'P', 'W', 'S', 'P', 'P', 'P', 'P', 'W', 'P', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'P', 'W', 'S', 'W', 'W', 'P', 'W', 'W', 'P', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'P', 'W', 'U', 'P', 'P', 'P', 'P', 'W', 'P', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'P', 'W', 'P', 'W', 'P', 'W', 'P', 'P', 'P', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'W', 'P', 'W', 'W', 'P', 'W', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'W', 'P', 'P', 'P', 'P', 'W', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'W', 'W', 'P', 'W', 'W', 'W', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'W'},
        {'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W'}
    };

    [SerializeField] private int _dieNumber = 6;
    [SerializeField]
    private int _activePlayerIndex;
    [SerializeField]

    private PlayerController[] _playerControllers;

    [SerializeField] private GameObject[] _tilePrefabs;

    [SerializeField] private Text _txtCurrentPlayer;
    [SerializeField] private Text _txtMovesLeft;
    [SerializeField] private Text _txtTurnNumber;

    [SerializeField] private int _height;
    [SerializeField] private int _width;

    #endregion Private Fields

    #region Public Properties

    public int ActivePlayerIndex { get; private set; }
    public PlayerController CurrentPlayer { get; private set; }

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

    private void CheckInput()
    {
        // WASD control
        // We add the direction to our position,
        // this moves the character 1 unit (32 pixels)
        CurrentPlayer = GetActivePlayer();

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


            if (CurrentPlayer.PlayerMoves <= 0)
            {
                NextTurn();
                CurrentPlayer = GetActivePlayer();
                CurrentPlayer.PlayerMoves = RollDice(_dieNumber);
            }
        }
    }

    public Tile GetGameTile(int x, int y)
    {
        return GameGrid[x, y];
    }

    #endregion Public Methods

    #region Private Methods

    private static int RollDice(int d)
    {
        int rollDice = new Random().Next(1, d);
        Debug.Log(string.Format("Rolled a {0}", rollDice));
        return rollDice;
    }

    private PlayerController GetActivePlayer()
    {
        return PlayerControllers[ActivePlayerIndex];
    }

    private void NextTurn()
    {
        ActivePlayerIndex = (ActivePlayerIndex + 1) % PlayerControllers.Length;
        if (ActivePlayerIndex == 0)
        {
            TurnNumber++;
            Vector3 pos;
            Tile tile;
            do
            {
                pos = new Vector3(UnityEngine.Random.Range(0, Width - 1), UnityEngine.Random.Range(0, Height - 1));
                tile = GetGameTile((int)pos.x, (int)pos.y);
            } while (!(tile.CurrentPlayer == null && tile.CanLandOn()));

            Instantiate(CoinPrefab, pos, Quaternion.identity);
        }
    }


    // Use this for initialization
    private void Start()
    {
        GameGrid = new Tile[Width, Height];
        for (var x = 0; x <= GameGrid.GetUpperBound(0); x++)
            for (var y = 0; y <= GameGrid.GetUpperBound(1); y++)
            {
                GameObject tileToMake = _tilePrefabs[0];
                Vector2 facing = Vector2.zero;
                switch (_map[15 - y, x])
                {
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

                //TODO: Assign data to each tile when created, to have different tile types
            }

        PlayerControllers = new PlayerController[4];
        ActivePlayerIndex = 0;
        for (var i = 0; i < PlayerControllers.Length; i++)
        {
            GameObject playerInstance = Instantiate(PlayerPrefab, new Vector3(i + 1, i + 1, 0), Quaternion.identity);
            var playerController = playerInstance.GetComponent<PlayerController>();
            PlayerControllers[i] = playerController;
            playerController.Id = i + 1;
        }

        CurrentPlayer = GetActivePlayer();
        CurrentPlayer.PlayerMoves = RollDice(_dieNumber);

        TurnNumber = 1;
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateUI();
        CheckInput();
    }

    private void UpdateUI()
    {
        _txtCurrentPlayer.text = CurrentPlayer.Id.ToString();
        _txtMovesLeft.text = PlayerMovesLeft.ToString();
        _txtTurnNumber.text = TurnNumber.ToString();
    }

    #endregion Private Methods
}