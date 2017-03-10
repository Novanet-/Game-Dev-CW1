using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : Powerup{

	// Use this for initialization
	void Start () {
		
	    ToolTip = "Allows you to fly over walls!";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Activate()
    {
        if (Holder.Dice.Count < 2) return;
        Holder.GetAvailibleMoves(Holder.Dice, true);
        base.Activate();
    }
}
