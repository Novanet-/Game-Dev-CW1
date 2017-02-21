﻿using System.Collections;
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
        if (player.PlayerMoves > -(2 * MOVESTOPUSH) && player.PlayerMoves <= 0)
        {
            player.PlayerMoves--;
            player.Move(Direction);
        }
    }

    public void PlayerLeaves(PlayerController player)
    {
    }
}
