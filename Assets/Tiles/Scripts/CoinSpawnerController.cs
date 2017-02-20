using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawnerController : Tile
{

    [SerializeField] private GameObject TilePrefab;
	// Use this for initialization
	protected override void Start ()
	{
        base.Start();
	    GameObject tile = Instantiate(TilePrefab, transform);
        tile.transform.position = transform.position;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
