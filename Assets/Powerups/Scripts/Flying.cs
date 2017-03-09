using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying : Powerup{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public override void Activate()
    {
        Holder.GetAvailibleMoves(Holder.Dice, true);
        base.Activate();
    }
}
