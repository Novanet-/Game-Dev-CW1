using UnityEngine;

public class GameController : MonoBehaviour
{
    #region Public Fields

    public Transform GameBoard;

    public Tile[,] GameGrid;

    public GameObject TilePrefab;

    #endregion Public Fields

    #region Private Fields

    [SerializeField] private int _height;

    [SerializeField] private int _width;

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
    private void Start()
    {
        GameGrid = new Tile[Width, Height];
        for (var x = 0; x <= GameGrid.GetUpperBound(0); x++)
        for (var y = 0; y <= GameGrid.GetUpperBound(1); y++)
        {
            GameObject tileInstance = Instantiate(TilePrefab, new Vector3(x, y, 0), Quaternion.identity);
            GameGrid[x, y] = tileInstance.GetComponent<Tile>();
            //TODO: Assign data to each tile when created, to have different tile types
        }
    }

    // Update is called once per frame
    private void Update()
    {
    }

    #endregion Private Methods
}