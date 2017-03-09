using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Tiles.Scripts;
using UnityEngine;

public class Jail : Tile, IPlayerMovementListener
{

	// Use this for initialization
	 public override void Start () {
        base.Start();
        AddPlayerMovementListener(this);
	}
	
	// Update is called once per frame
	void Update () {
	}

    public override void SetSprite(SpriteRenderer renderer)
    {
        renderer.sprite = _sprites[0];
    }

    public void PlayerLandsOn(PlayerController player)
    {
        foreach (PlayerController playerController in GameController.GetGameController().PlayerControllers)
        {
            if (playerController != player)
            {
                if (playerController.GetCurrentTile() is CoinSpawnerController)
                {
                    playerController.Disabled = true;
                }
            }
        }
    }

    public void PlayerLeaves(PlayerController player)
    {
    }

    public void PlayerRemainsOn(PlayerController player)
    {
    }

    public void PlayerMovedOver(PlayerController player, bool doneMoving)
    {
    }
}
