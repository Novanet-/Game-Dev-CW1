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
	    _removeMovementListener = false;
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
        if (player != null)
        {
            player.Powerups.Add(this);
            Holder = player;
            _removeMovementListener = true;
            UIController.GetUIController().UpdatePowerupBar(player);
        }

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

    public void Hide()
    {
       GetComponent<SpriteRenderer>().enabled = false;
    }

    public void Show()
    {
       GetComponent<SpriteRenderer>().enabled = true;
    }

    public virtual void OnMouseEnter()
    {
        UIController.GetUIController().TooltipText = ToolTip;
    }

    public virtual void OnMouseExit()
    {
        UIController.GetUIController().TooltipText = "";
    }
}
