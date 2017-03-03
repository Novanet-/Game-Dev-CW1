using System;
using System.Collections;
using System.Collections.Generic;
using cakeslice;
using UnityEngine;
using Random = UnityEngine.Random;

public class CoinSpawnerController : Tile, RoundEndListener
{

    [SerializeField] private GameObject TilePrefab, GoldPrefab;
    private GoldController gold;
    private int _timeForMoreGold;
    private float _goldSpawnTime;
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
	    _timeForMoreGold = Random.Range(4, 7);


        GameController controller = GameController.GetGameController();
        controller.AddRoundEndListener(this);
	}
	
	// Update is called once per frame
	void Update () {
            StopGlowing(2);
	    if (Time.time < _goldSpawnTime + 2)
	    {
	        Glow(2);
	    }
	}

    private void StopGlowing(int i)
    {
        Outline outline = GetComponent<Outline>();
        outline.color = 1;
        if (IsValidMove)
            Glow();
        else 
            StopGlowing();
    }   

    private void Glow(int i)
    {
        Outline outline = GetComponent<Outline>();
        outline.color = i;
        outline.enabled = true;
    }

    public override void SetSprite(SpriteRenderer renderer)
    {
        renderer.sprite = _sprites[Random.Range(0, _sprites.Length - 1)];
    }

    public void OnRoundEnd(int roundNumber)
    {
        if (--_timeForMoreGold == 0)
        {
            gold.getGold();
            _timeForMoreGold = Random.Range(4, 7);
            _goldSpawnTime = Time.time;
        }
    }

    public int GetGoldAmount()
    {
        return gold.Gold;
    }
}
