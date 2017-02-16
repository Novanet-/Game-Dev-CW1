using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Tile : MonoBehaviour
{
    private int foo;
    private int bar;

    public void landedOn(PlayerController player)
    {
        Debug.Log("Landed on Tile at: " + transform.position.x + ", " + transform.position.y);
    }
}