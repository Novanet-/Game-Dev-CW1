using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinSpawnerController : Tile, RoundEndListener
{

    [SerializeField] private GameObject TilePrefab, GoldPrefab;
    private GoldController gold;
	// Use this for initialization
	public override void Start ()
	{
        base.Start();
	    GameObject tile = Instantiate(TilePrefab, transform);
        tile.transform.position = transform.position;

        GameObject goldObject = Instantiate(GoldPrefab, transform);
	    goldObject.transform.position = transform.position;
	    gold = goldObject.GetComponent<GoldController>();
	    gold.Tile  = this;

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnRoundEnd(int roundNumber)
    {
        gold.getGold();
    }
}
