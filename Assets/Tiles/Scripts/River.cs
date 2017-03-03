using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class River : Tile, PlayerMovementListener{
    private readonly int MOVESTOPUSH = 1;
    private Tile toPushTo;


    // Use this for initialization
	public override void  Start ()
	{
	    base.Start();
        this.AddPlayerMovementListener(this);
	    toPushTo = GameController.GetGameController()
	        .GetGameTile((int)(transform.position.x + Direction.x), (int)(transform.position.y + Direction.y));

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void SetSprite(SpriteRenderer renderer)
    {
        renderer.sprite = _sprites[Random.Range(0, _sprites.Length)];
    }

    public void PlayerLandsOn(PlayerController player)
    {
        MovePlayer(player);
    }

    public void PlayerRemainsOn(PlayerController player)
    {
        MovePlayer(player);
    }

    private void MovePlayer(PlayerController player)
    {
        //hack to avoid PLayerMoves never decrementing when being pushed by a river that can't push a player
        if (player.PlayerMoves > 2* -MOVESTOPUSH && player.PlayerMoves <= 0)
        {
            player.PlayerMoves--;
            player.Move(toPushTo);
        }
    }

    public void PlayerLeaves(PlayerController player)
    {
    }
}
