using UnityEngine;

namespace Assets.Scripts
{
    public static class UnityExtensions
    {
        #region Public Methods

        public static Vector3 ScreenToWorldLength(this Camera camera, Vector3 position)
        {
            return camera.ScreenToWorldPoint(position) - camera.ScreenToWorldPoint(Vector3.zero);
        }

        #endregion Public Methods
    }
}