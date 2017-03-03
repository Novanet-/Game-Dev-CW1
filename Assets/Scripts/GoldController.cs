﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldController : MonoBehaviour, PlayerMovementListener
{
    public int Gold { get; private set; }
    [SerializeField] private Sprite[] _sprites;
    private int _spriteNum;
    private Tile _tile;

    public Tile Tile
    {
        get { return _tile; }
        set
        {
            if (_tile != null)
            _tile.RemovePlayerMovementListener(this);
            _tile = value;
            _tile.AddPlayerMovementListener(this);
        }
    }

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
        if (Gold > 0)
        {
            player.Money = player.Money + 10;
            Gold = Gold - 10;
            Debug.Log("Log Given Gold to Player" + player.Id + ", remaining gold: " + Gold);
        }

        var spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteNum = Mathf.Clamp(_spriteNum -1, 0, _sprites.Length - 1);
        spriteRenderer.sprite = _sprites[_spriteNum];
    }

    // Use this for initialization
	void Awake()
	{
	    Gold = 10;
        var spriteRenderer = GetComponent<SpriteRenderer>();
	    _spriteNum = 1;
        spriteRenderer.sprite = _sprites[_spriteNum];
	}

	// Update is called once per frame
	void Update()
	{
	}


    public void getGold()
    {
        Gold = Gold + 10;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteNum = Mathf.Clamp(_spriteNum + 1, 0, _sprites.Length - 1);
        spriteRenderer.sprite = _sprites[_spriteNum];
    }
}