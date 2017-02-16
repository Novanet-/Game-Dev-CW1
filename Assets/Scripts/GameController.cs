using UnityEngine;

public class GameController : MonoBehaviour
{
    public int Width
    {
        get { return _width; }
        private set { _width = value; }
    }

    public int Height
    {
        get { return _height; }
        private set { _height = value; }
    }

    public Tile[,] GameGrid;

    public GameObject TilePrefab;
    public Transform GameBoard;

    [SerializeField] private int _width;
    [SerializeField] private int _height;


    // Use this for initialization
    void Start()
    {
        GameGrid = new Tile[Width, Height];
        for (var x = 0; x <= GameGrid.GetUpperBound(0); x++)
        {
            for (var y = 0; y <= GameGrid.GetUpperBound(1); y++)
            {
                GameObject tileInstance = Instantiate(TilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                GameGrid[x, y] = tileInstance.GetComponent<Tile>();
                //TODO: Assign data to each tile when created, to have different tile types
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Tile GetGameTile(int x, int y)
    {
        return GameGrid[x, y];
    }
}