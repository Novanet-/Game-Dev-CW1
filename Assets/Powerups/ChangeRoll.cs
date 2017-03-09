using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts;

public class ChangeRoll : Powerup {

	// Use this for initialization
	void Start ()
	{
	    ToolTip = "Adds 1 to your dice rolls";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Activate(GameController gameController, PlayerController currentPlayer)
    {
    }
    
}
