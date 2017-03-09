using Assets.Scripts;
using UnityEngine;

public abstract class Powerup : MonoBehaviour
{

    private PlayerController holder;
	// Use this for initialization
	public virtual void Start ()
	{
	    ToolTip = "";
	}
	
	// Update is called once per frame
	void Update () {
	}

    public string ToolTip { get; protected set; }

    public abstract void Activate(GameController gameController, PlayerController currentPlayer);

    public void Activate()
    {
        Activate(GameController.GetGameController(), holder);
    }

    public void PickUp(PlayerController player)
    {
        player.Powerups.Add(this);
        holder = player;
    }
}
