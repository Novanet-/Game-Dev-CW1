using UnityEngine;

namespace Assets.Tiles.Scripts
{
    public class Wall : Tile
    {
        #region Public Methods

        public override bool CanLandOn()
        {
            Debug.Log(("Calling Wall CanLandOn"));
            return false;
        }

        public override bool CanPassThrough() { return false; }

        #endregion Public Methods
    }
}