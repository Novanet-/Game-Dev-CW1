using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : Tile, PlayerMovementListener{
    private readonly int MOVESTOPUSH = 1;


    // Use this for initialization
	public override void  Start ()
	{
	    base.Start();
        this.AddPlayerMovementListener(this);
		
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
        if (player.PlayerMoves > -MOVESTOPUSH)
        {
            player.PlayerMoves--;
            player.Move(Direction);
        }
    }

    public void PlayerLeaves(PlayerController player)
    {
    }
}
