using JetBrains.Annotations;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    #region Private Fields

    [SerializeField] private GameObject _gameBoard;

    private GameController _gameController;

    [SerializeField] private int _money;

    private bool _moving;

    private Vector2 _pos;

    #endregion Private Fields

    #region Public Properties

    [NotNull] public GameObject GameBoard
    {
        get { return _gameBoard; }
        private set { _gameBoard = value; }
    }
    public int Money
    {
        get { return _money; }
        set { _money = value; }
    }

    #endregion Public Properties

    #region Public Methods

    public void Move(Vector2 direction)
    {
        Debug.Log("" + _pos + " /" + direction + " /" + _gameController);
        _pos.x = Mathf.Clamp(_pos.x, 0, _gameController.Width - 1);
        _pos.y = Mathf.Clamp(_pos.y, 0, _gameController.Height - 1);

        _moving = true;
    }

    #endregion Public Methods

    #region Private Methods

    // Use this for initialization
    private void Start()
    {
        // First store our current position when the
        // script is initialized.
        _pos = transform.position;
//        _gameController = _gameBoard.GetComponent<GameController>();
//        _gameController = GameBoard.GetComponent<GameController>();
        GameBoard = GameObject.Find("GameBoard");
        _gameController = GameBoard.GetComponent<GameController>();
    }


    // Update is called once per frame
    private void Update()
    {
        if (!_moving) return;

        // pos is changed when there's input from the player
        transform.position = _pos;
        _moving = false;

        //TODO: We also need to update the gamestate at this point, which contains a 2d array of tiles, for looking up tile info etc.
        Tile newTile = _gameController.GetGameTile((int) _pos.x, (int) _pos.y);

        newTile.LandedOn(this);
    }

    #endregion Private Methods
}