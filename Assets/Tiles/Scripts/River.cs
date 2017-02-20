using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class River : Tile, PlayerMovementListener{
    private readonly int MOVESTOPUSH = 1;


    // Use this for initialization
	void Start ()
	{
	    base.Start();
        Direction = Vector2.right;
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
        if (player.PlayerMoves > -MOVESTOPUSH && player.PlayerMoves <= 0)
        {
            player.PlayerMoves--;
            player.Move(Direction);
        }
    }

    public Vector2 Direction
    { get; set; }

    public void PlayerLeaves(PlayerController player)
    {
    }
}
