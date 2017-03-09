using Assets.Scripts;
using UnityEngine;

namespace Assets.Tiles.Scripts
{
    public class River : Tile, IPlayerMovementListener
    {
        #region Private Fields

        private readonly int MOVESTOPUSH = 1;
        public Tile DestinationTile { get; private set; }

        #endregion Private Fields


        #region Public Methods

        /// <summary>
        /// Players the lands on.
        /// </summary>
        /// <param name="player">The player.</param>
        public void PlayerLandsOn(PlayerController player) { MovePlayer(player); }

        /// <summary>
        /// Players the leaves.
        /// </summary>
        /// <param name="player">The player.</param>
        public void PlayerLeaves(PlayerController player) { }

        /// <summary>
        /// Players the remains on.
        /// </summary>
        /// <param name="player">The player.</param>
        public void PlayerRemainsOn(PlayerController player) { MovePlayer(player); }

        /// <summary>
        /// Sets the sprite.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public override void SetSprite(SpriteRenderer renderer)
        {
            renderer.sprite = _sprites[Random.Range(0, 0)]; // _sprites.Length)];
             renderer.transform.rotation = Quaternion.FromToRotation(Vector2.right, Direction);
        }

        public void PlayerMovedOver(PlayerController player, bool doneMoving)
        {
        }

        // Use this for initialization
        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Start()
        {
            base.Start();
            AddPlayerMovementListener(this);
            DestinationTile = GameController.GetGameController()
                                     .GetGameTile((int) (transform.position.x + Direction.x), (int) (transform.position.y + Direction.y));
        }

        #endregion Public Methods


        #region Private Methods

        /// <summary>
        /// Moves the player.
        /// </summary>
        /// <param name="player">The player.</param>
        private void MovePlayer(PlayerController player)
        {
            if (player.CanBePushed)
            {
                player.CanBePushed = false;
                player.MoveToTile(DestinationTile);
            }
        }

        public override void OnMouseEnter()
        {
            base.OnMouseEnter();
            if (Direction == Vector2.down)
                UIController.GetUIController().TooltipText = "Down";
            else if (Direction == Vector2.up)
                UIController.GetUIController().TooltipText = "Up";
            else if (Direction == Vector2.right)
                UIController.GetUIController().TooltipText = "Right";
            else if (Direction == Vector2.left)
                UIController.GetUIController().TooltipText = "Left";
        }


        public override void OnMouseExit()
        {
            base.OnMouseExit();
            UIController.GetUIController().TooltipText = "";
        }

        // Update is called once per frame
        private void Update() { }

        #endregion Private Methods
    }
}