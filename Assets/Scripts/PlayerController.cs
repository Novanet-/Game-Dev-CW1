using UnityEngine;

public class PlayerController : MonoBehaviour

{
    #region Private Fields

    private GameController _gameController;

    [SerializeField]
    private GameObject _gameBoard;

    private bool _moving;
    private Vector2 _pos;


    public int _money;

    #endregion Private Fields

    #region Public Methods

    public void Move(Vector2 direction)
    {
        
        Debug.Log("" + _pos + " /" + direction + " /" +  _gameController);
        _pos.x = Mathf.Clamp(_pos.x, 0, _gameController.Width - 1);
        _pos.y = Mathf.Clamp(_pos.y, 0, _gameController.Height - 1);

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
        _gameController = _gameBoard.GetComponent<GameController>();
    }


    // Update is called once per frame
    private void Update()
    {

        if (!_moving) return;

        // pos is changed when there's input from the player
        transform.position = _pos;
        _moving = false;

        //TODO: We also need to update the gamestate at this point, which contains a 2d array of tiles, for looking up tile info etc.
        Tile newTile = _gameController.GetGameTile((int)_pos.x, (int)_pos.y);

        newTile.LandedOn(this);
    }

    #endregion Private Methods
}