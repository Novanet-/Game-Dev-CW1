using UnityEngine;

public class Wall : Tile
{

    public override bool CanLandOn()
    {
        Debug.Log(("Calling Wall CanLandOn"));
        return false;
    }
}
