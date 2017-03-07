using Assets.Tiles.Scripts;
using UnityEngine;

namespace Assets.Scripts
{
    public class GoldController : MonoBehaviour, IPlayerMovementListener
    {
        #region Private Fields

        private int _spriteNum;
        [SerializeField] private Sprite[] _sprites;
        private Tile _tile;

        #endregion Private Fields


        #region Public Properties

        public int Gold { get; private set; }

        /// <summary>
        /// Gets or sets the tile.
        /// </summary>
        /// <value>
        /// The tile.
        /// </value>
        public Tile Tile
        {
            get { return _tile; }
            set
            {
                if (_tile != null) _tile.RemovePlayerMovementListener(this);
                _tile = value;
                _tile.AddPlayerMovementListener(this);
            }
        }

        #endregion Public Properties


        #region Public Methods

        /// <summary>
        /// Gets the gold.
        /// </summary>
        public void GetGold()
        {
            Gold = Gold + 10;
            var spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteNum = Mathf.Clamp(_spriteNum + 1, 0, _sprites.Length - 1);
            spriteRenderer.sprite = _sprites[_spriteNum];
        }

        /// <summary>
        /// Players the lands on.
        /// </summary>
        /// <param name="player">The player.</param>
        public void PlayerLandsOn(PlayerController player) { GivePlayerGold(player); }

        /// <summary>
        /// Players the leaves.
        /// </summary>
        /// <param name="player">The player.</param>
        public void PlayerLeaves(PlayerController player) { }

        /// <summary>
        /// Players the remains on.
        /// </summary>
        /// <param name="player">The player.</param>
        public void PlayerRemainsOn(PlayerController player) { GivePlayerGold(player); }

        #endregion Public Methods


        #region Private Methods

        // Use this for initialization
        /// <summary>
        /// Awakes this instance.
        /// </summary>
        private void Awake()
        {
            Gold = 10;
            var spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteNum = 1;
            spriteRenderer.sprite = _sprites[_spriteNum];
        }

        /// <summary>
        /// Gives the player gold.
        /// </summary>
        /// <param name="player">The player.</param>
        private void GivePlayerGold(PlayerController player)
        {
            if (Gold > 0)
            {
                player.Money = player.Money + 10;
                Gold = Gold - 10;
                Debug.Log("Log Given Gold to Player " + player.Id + ", remaining gold: " + Gold);
            }
            else
            {
                Debug.Log("No Here! Player " + player.Id + ", has gone without!");
            }

            var spriteRenderer = GetComponent<SpriteRenderer>();
            _spriteNum = Mathf.Clamp(_spriteNum - 1, 0, _sprites.Length - 1);
            spriteRenderer.sprite = _sprites[_spriteNum];
        }

        // Update is called once per frame
        private void Update() { }

        #endregion Private Methods
    }
}