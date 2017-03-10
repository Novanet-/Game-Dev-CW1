using Assets.Scripts;
using Assets.Tiles.Scripts;
using UnityEngine;

public abstract class Powerup : MonoBehaviour, IPlayerMovementListener
{

    protected PlayerController Holder;
    private Tile _tile;
    // Use this for initialization
	public virtual void Start ()
	{
	    ToolTip = "";
	}

	// Update is called once per frame
	void Update () {
	    if (Holder != null && Tile != null)
	    {
            Tile.RemovePlayerMovementListener(this);
	        Tile.HasPowerUp = false;
	        Tile = null;
	    }
	}

    public string ToolTip { get; protected set; }

    public Tile Tile
    {
        get { return _tile; }
        set
        {
            _tile = value;
            if (Tile != null)
            {
                transform.position = Tile.transform.position;
                Tile.AddPlayerMovementListener(this);
            }
        }
    }

    public virtual void Activate()
    {
        Debug.Log("Before List Removeal");
        foreach (Powerup powerup in Holder.Powerups)
        {
            Debug.Log(powerup);
        }
        Holder.Powerups.Remove(this);
        Debug.Log("AFter list Removeal");
        foreach (Powerup powerup in Holder.Powerups)
        {
            Debug.Log(powerup);
        }
        UIController.GetUIController().UpdatePowerupBar(Holder);
        Destroy(gameObject);
    }

    public void PickUp(PlayerController player)
    {
        if (player != null)
        {
            if (player.Powerups.Contains(this) || player.Powerups.Count > 5) return;
            player.Powerups.Add(this);
            Holder = player;
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
       transform.position = new Vector2(-100, -100);

    }

    public void Show()
    {
       GetComponent<SpriteRenderer>().enabled = true;
    }

    public virtual void OnMouseEnter()
    {
        UIController.GetUIController().TooltipText = ToolTip;
    }

    public virtual void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
            if (Holder != null && GameController.GetGameController().ActivePlayer == Holder)
        {
            Activate();
        }
    }
    public virtual void OnMouseExit()
    {
        UIController.GetUIController().TooltipText = "";
    }
}
