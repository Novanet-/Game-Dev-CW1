using UnityEngine;

namespace Assets.Tiles.Scripts
{
    public class Wall : Tile
    {
        #region Public Methods

        /// <summary>
        /// Determines whether this instance [can land on].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can land on]; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanLandOn()
        {
            Debug.Log(("Calling Wall CanLandOn"));
            return false;
        }

        /// <summary>
        /// Determines whether this instance [can pass through].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can pass through]; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanPassThrough() { return false; }

        #endregion Public Methods
    }
}