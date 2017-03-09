using Assets.Scripts;
using Assets.Tiles.Scripts;
using UnityEngine;

public abstract class Powerup : MonoBehaviour, IPlayerMovementListener
{

    protected PlayerController Holder;
    private Tile _tile;
    private bool _removeMovementListener;
    // Use this for initialization
	public virtual void Start ()
	{
	    ToolTip = "";
	}
	
	// Update is called once per frame
	void Update () {
	    if (_removeMovementListener)
	    {
	        _removeMovementListener = false;
            Tile.RemovePlayerMovementListener(this);
	    }
	}

    public string ToolTip { get; protected set; }

    public Tile Tile
    {
        get { return _tile; }
        set
        {
            _tile = value;
            transform.position = Tile.transform.position;
            Tile.AddPlayerMovementListener(this);
        }
    }

    public virtual void Activate()
    {
        Holder.Powerups.Remove(this);
        Destroy(this);
    }

    public void PickUp(PlayerController player)
    {
        player.Powerups.Add(this);
        Holder = player;
        _removeMovementListener = true;
        
        GetComponent<SpriteRenderer>().enabled = false;
    }

    public void PlayerLandsOn(PlayerController player)
    {
    }

    public void PlayerLeaves(PlayerController player)
    {
    }

    public void PlayerRemainsOn(PlayerController player)
    {
    }

    public void PlayerMovedOver(PlayerController player, bool doneMoving)
    {
        PickUp(player);
    }
}
