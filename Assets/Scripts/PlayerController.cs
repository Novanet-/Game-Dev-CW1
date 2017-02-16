using UnityEngine;

public class PlayerController : MonoBehaviour

{
    #region Private Fields

    [SerializeField]
    private GameController _gameBoard;

    private bool _moving;
    private Vector2 _pos;


    public int _money;

    #endregion Private Fields

    #region Public Methods

    public void CheckInput()
    {
        // WASD control
        // We add the direction to our position,
        // this moves the character 1 unit (32 pixels)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            _pos += Vector2.right;
            _moving = true;
        }

        // For left, we have to subtract the direction
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _pos += Vector2.left;
            _moving = true;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            _pos += Vector2.up;
            _moving = true;
        }

        // Same as for the left, subtraction for down
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            _pos += Vector2.down;
            _moving = true;
        }

        _pos.x = Mathf.Clamp(_pos.x, 0, _gameBoard.Width - 1);
        _pos.y = Mathf.Clamp(_pos.y, 0, _gameBoard.Height - 1);
    }

    #endregion Public Methods

    #region Private Methods

    public int money
    {
        get { return _money; }
        set { _money = value; }
    }


    // Use this for initialization
    private void Start()
    {
        // First store our current position when the
        // script is initialized.
        _pos = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();

        if (!_moving) return;

        // pos is changed when there's input from the player
        transform.position = _pos;
        _moving = false;

        //TODO: We also need to update the gamestate at this point, which contains a 2d array of tiles, for looking up tile info etc.
        Tile newTile = _gameBoard.GetGameTile((int)_pos.x, (int)_pos.y);

        newTile.LandedOn(this);
    }

    #endregion Private Methods
}