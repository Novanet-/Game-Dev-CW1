using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    #region Public Fields

    //    public Transform GameBoard;
    public Tile[,] GameGrid;

    public GameObject PlayerPrefab, CoinPrefab;

    #endregion Public Fields

    #region Private Fields

    private const int Path = 0;

    private const int River = 2;

    private const int Wall = 1;

    // P is a Path
    // W is Wall
    // R, S, T, U are River Tiles (Up, Down, Left, Right, respectively)
    private readonly char[,] _map =
    {
        {'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'W', 'W', 'W', 'P', 'P', 'P', 'P', 'P', 'P', 'W'},
        {'W', 'P', 'W', 'W', 'P', 'W', 'W', 'P', 'P', 'P', 'W', 'P', 'W', 'P', 'W', 'W'},
        {'W', 'P', 'W', 'W', 'P', 'W', 'W', 'P', 'W', 'W', 'W', 'P', 'P', 'P', 'W', 'W'},
        {'U', 'U', 'U', 'U', 'U', 'U', 'U', 'U', 'S', 'P', 'P', 'P', 'W', 'W', 'W', 'W'},
        {'U', 'U', 'U', 'U', 'U', 'U', 'S', 'U', 'S', 'W', 'W', 'W', 'W', 'W', 'W', 'W'},
        {'W', 'P', 'P', 'P', 'P', 'P', 'S', 'W', 'S', 'W', 'W', 'W', 'W', 'W', 'P', 'W'},
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

    [SerializeField] private GameObject[] _tilePrefabs;

    [SerializeField] private Text _txtCurrentPlayer;
    [SerializeField] private Text _txtMovesLeft;

    [SerializeField] private int _height;
    [SerializeField] private int _width;

    #endregion Private Fields

    #region Public Properties

    public int ActivePlayerIndex { get; private set; }
    public PlayerController CurrentPlayer { get; private set; }

    public PlayerController[] PlayerControllers { get; private set; }
    public int PlayerMovesLeft { get; private set; }

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

    public void CheckInput()
    {
        // WASD control
        // We add the direction to our position,
        // this moves the character 1 unit (32 pixels)
        Vector2 direction = Vector2.zero;
        var moved = false;

        CurrentPlayer = GetActivePlayer();

        PlayerMovesLeft = CurrentPlayer.PlayerMoves;

        if (Input.anyKey)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                moved = true;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                    direction = SetupMove(Vector2.right, ref moved);

                // For left, we have to subtract the direction
                else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                    direction = SetupMove(Vector2.left, ref moved);
                else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                    direction = SetupMove(Vector2.up, ref moved);

                // Same as for the left, subtraction for down
                else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                    direction = SetupMove(Vector2.down, ref moved);
            }


            if (moved)
                CurrentPlayer.Move(direction);

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
            Vector3 pos;
            Tile tile;
            do
            {
                pos = new Vector3(UnityEngine.Random.Range(0, Width - 1), UnityEngine.Random.Range(0, Height - 1));
                tile = GetGameTile((int) pos.x, (int) pos.y);
            } while (!(tile.CurrentPlayer == null && tile.CanLandOn()));

            Instantiate(CoinPrefab, pos, Quaternion.identity);
        }
    }

    private Vector2 SetupMove(Vector2 direction, ref bool moved)
    {
        moved = true;
        CurrentPlayer.PlayerMoves--;
        Debug.Log(string.Format("{0} moves left", CurrentPlayer.PlayerMoves));

        return direction;
    }

    // Use this for initialization
    private void Start()
    {
        GameGrid = new Tile[Width, Height];
        for (var x = 0; x <= GameGrid.GetUpperBound(0); x++)
        for (var y = 0; y <= GameGrid.GetUpperBound(1); y++)
        {
            GameObject tileToMake = _tilePrefabs[0];
            switch (_map[15 - y, x])
            {
                case 'W':
                    tileToMake = _tilePrefabs[Wall];
                    break;

                case 'R':
                case 'S':
                case 'T':
                case 'U':
                    tileToMake = _tilePrefabs[River];
                    break;

                default:
                    tileToMake = _tilePrefabs[Path];
                    break;
            }
            GameObject tileInstance = Instantiate(tileToMake, new Vector3(x, y, 0), Quaternion.identity);
            GameGrid[x, y] = tileInstance.GetComponent<Tile>();

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
    }

    #endregion Private Methods
}