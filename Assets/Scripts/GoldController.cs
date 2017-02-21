using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoldController : MonoBehaviour, PlayerMovementListener
{
    private int _gold;
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
        if (_gold > 0)
        {
            player.Money = player.Money + 10;
            _gold = _gold - 10;
        }

        var spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteNum = Mathf.Clamp(--_spriteNum, 0, _sprites.Length - 1);
        spriteRenderer.sprite = _sprites[_spriteNum];
    }

    // Use this for initialization
	void Start()
	{
	    _gold = 10;
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
        _gold = _gold + 10;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteNum = Mathf.Clamp(_spriteNum++, 0, _sprites.Length - 1);
        spriteRenderer.sprite = _sprites[_spriteNum];
    }
}