using System;
using System.Security.Cryptography.X509Certificates;
using JetBrains.Annotations;
using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Public Fields

//    public Transform GameBoard;
    public Tile[,] GameGrid;
    public GameObject TilePrefab, PlayerPrefab;

    #endregion Public Fields

    #region Private Fields

    [SerializeField] private int _height;
    [SerializeField] private int _width;
    [SerializeField] private PlayerController[] _playerControllers;
    [SerializeField] private int _activePlayer;

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

    public Tile GetGameTile(int x, int y)
    {
        return GameGrid[x, y];
    }

    #endregion Public Methods

    #region Private Methods

    // Use this for initialization
    void Start()
    {
        GameGrid = new Tile[Width, Height];
        for (var x = 0; x <= GameGrid.GetUpperBound(0); x++)
        for (var y = 0; y <= GameGrid.GetUpperBound(1); y++)
        {
            GameObject tileInstance = Instantiate(TilePrefab, new Vector3(x, y, 0), Quaternion.identity);
            GameGrid[x, y] = tileInstance.GetComponent<Tile>();
            //TODO: Assign data to each tile when created, to have different tile types
        }

        _playerControllers = new PlayerController[4];
        _activePlayer = 0;
        for (var i = 0; i < _playerControllers.Length; i++)
        {
             GameObject playerInstance = Instantiate(PlayerPrefab, new Vector3(i, i, 0), Quaternion.identity);
            _playerControllers[i] = playerInstance.GetComponent<PlayerController>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
    }


    public void CheckInput()
    {
        // WASD control
        // We add the direction to our position,
        // this moves the character 1 unit (32 pixels)
        Vector2 direction = Vector2.zero;
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Vector2.right;
        }

        // For left, we have to subtract the direction
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Vector2.up;
        }

        // Same as for the left, subtraction for down
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Vector2.down;
        }

        if (direction != Vector2.zero)
        {
            GetActivePlayer().Move(direction);
        }


    }

    private PlayerController GetActivePlayer()
    {
        PlayerController player = _playerControllers[_activePlayer];
        _activePlayer = (_activePlayer + 1) % _playerControllers.Length;
        return player;

    }

    #endregion Private Methods
}