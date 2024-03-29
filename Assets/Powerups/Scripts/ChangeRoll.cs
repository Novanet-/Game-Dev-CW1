﻿using System.Collections;
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

    public override void Activate()
    {
        UIController uiController = UIController.GetUIController();
        List<int> dice = Holder.Dice;
        if (dice.Count < 2) return;
        for (int i = 0; i < dice.Count; i++)
        {
            dice[i]++;
        }
        uiController.SetDice(dice);

        Holder.GetAvailibleMoves(dice);
        base.Activate();
    }

}
