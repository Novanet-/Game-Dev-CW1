using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldController : MonoBehaviour, PlayerMovementListener
{
    public void PlayerLandsOn(PlayerController player)
    {
        GivePlayerGold(player);
    }


    public void PlayerLeaves(PlayerController player)
    {
    }

    public void PlayerRemainsOn(PlayerController player)
    {
        GivePlayerGold(player);
    }

    private void GivePlayerGold(PlayerController player)
    {
        player.Money = player.Money + 10;
        Debug.Log("Given Player Gold! Gold is now " + player.Money);
    }


    // Use this for initialization
    void Start () {
        GameObject GameBoard = GameObject.Find("GameBoard");
        GameController _gameController = GameBoard.GetComponent<GameController>();
        Tile tile =_gameController.GetGameTile((int) transform.position.x, (int) transform.position.y);
        tile.AddPlayerMovementListener(this);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
