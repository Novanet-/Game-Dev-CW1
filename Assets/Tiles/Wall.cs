using UnityEngine;

public class NewBehaviourScript : Tile
{

    [SerializeField] private Sprite[] _sprites;

    public new bool CanLandOn()
    {
        return false;
    }
}
