using Assets.Scripts;
using UnityEngine;

public abstract class Powerup : MonoBehaviour {

	// Use this for initialization
	public virtual void Start ()
	{
	    ToolTip = "";
	}
	
	// Update is called once per frame
	void Update () {
	}

    public string ToolTip { get; private set; }

    public abstract void Activate(GameController gameController, PlayerController currentPlayer);

    public void Activate()
    {
        Activate(GameController.GetGameController(), GameController.GetGameController().ActivePlayer);
    }
}
