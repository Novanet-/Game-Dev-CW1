using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameController : MonoBehaviour
{

    public Tile[,] GameGrid;

    public GameObject TilePrefab;
    public Transform GameBoard;


    // Use this for initialization
    void Start()
    {
        GameGrid = new Tile[16,16];
        for (var x = 0; x < GameGrid.GetUpperBound(0); x++)
        {
            for (var y = 0; y < GameGrid.GetUpperBound(1); y++)
            {
                GameObject tileInstance = Instantiate(TilePrefab, new Vector3(x,y,0), Quaternion.identity);
                //TODO: Assign data to each tile when created, to have different tile types
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
    }
}