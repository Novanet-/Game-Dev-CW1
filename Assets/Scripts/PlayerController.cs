using UnityEngine;

public class PlayerController : MonoBehaviour

{
    #region Private Fields

    [SerializeField]
    private GameController GameBoard;

    private bool moving;
    private Vector2 pos;


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
            pos += Vector2.right;
            moving = true;
        }

        // For left, we have to subtract the direction
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            pos += Vector2.left;
            moving = true;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            pos += Vector2.up;
            moving = true;
        }

        // Same as for the left, subtraction for down
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            pos += Vector2.down;
            moving = true;
        }

        pos.x = Mathf.Clamp(pos.x, 0, GameBoard.Width - 1);
        pos.y = Mathf.Clamp(pos.y, 0, GameBoard.Height - 1);
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
        pos = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();

        if (!moving) return;

        // pos is changed when there's input from the player
        transform.position = pos;
        moving = false;

        //TODO: We also need to update the gamestate at this point, which contains a 2d array of tiles, for looking up tile info etc.
        Tile newTile = GameBoard.GetGameTile((int)pos.x, (int)pos.y);

        newTile.LandedOn(this);
    }

    #endregion Private Methods
}