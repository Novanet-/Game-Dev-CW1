using UnityEngine;
using Random = System.Random;

public class GameController : MonoBehaviour
{
    #region Public Fields

    //    public Transform GameBoard;
    public Tile[,] GameGrid;

    public GameObject PlayerPrefab, CoinPrefab;

    #endregion Public Fields

    #region Private Fields

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

    private readonly int PATH = 0;
    private readonly int WALL = 1;
    private readonly int RIVER = 2;
    [SerializeField]
    private int _activePlayerIndex;
    [SerializeField]
    private int _height;

    private PlayerController[] _playerControllers;

    private int _playerMovesLeft;

    [SerializeField]
    private int _width;

    private PlayerController CurrentPlayer;

    [SerializeField]
    private int dieNumber = 6;

    [SerializeField]
    private GameObject[] TilePrefabs;

    #endregion Private Fields

    #region Public Properties

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
                CurrentPlayer.PlayerMoves = RollDice(dieNumber);
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
        return _playerControllers[_activePlayerIndex];
    }

    private void NextTurn()
    {
        _activePlayerIndex = (_activePlayerIndex + 1) % _playerControllers.Length;
        if (_activePlayerIndex == 0)
        {
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
                GameObject tileToMake = TilePrefabs[0];
                switch (_map[15 - y, x])
                {
                    case 'W':
                        tileToMake = TilePrefabs[WALL];
                        break;

                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                        tileToMake = TilePrefabs[RIVER];
                        break;

                    default:
                        tileToMake = TilePrefabs[PATH];
                        break;
                }
                GameObject tileInstance = Instantiate(tileToMake, new Vector3(x, y, 0), Quaternion.identity);
                GameGrid[x, y] = tileInstance.GetComponent<Tile>();

                //TODO: Assign data to each tile when created, to have different tile types
            }

        _playerControllers = new PlayerController[4];
        _activePlayerIndex = 0;
        for (var i = 0; i < _playerControllers.Length; i++)
        {
            GameObject playerInstance = Instantiate(PlayerPrefab, new Vector3(i + 1, i + 1, 0), Quaternion.identity);
            _playerControllers[i] = playerInstance.GetComponent<PlayerController>();
        }

        CurrentPlayer = GetActivePlayer();
        CurrentPlayer.PlayerMoves = RollDice(dieNumber);
    }


    // Update is called once per frame
    private void Update()
    {
        CheckInput();
    }

    #endregion Private Methods
}