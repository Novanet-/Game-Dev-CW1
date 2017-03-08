using Assets.Scripts;
using cakeslice;
using UnityEngine;

namespace Assets.Tiles.Scripts
{
    public class CoinSpawnerController : Tile, IRoundEndListener
    {
        #region Private Fields

        private float _goldSpawnTime;
        private int _timeForMoreGold;
        private GoldController gold;
        [SerializeField] private GameObject TilePrefab, GoldPrefab;

        #endregion Private Fields


        #region Public Methods

        // Use this for initialization
        /// <summary>
        /// Awakes this instance.
        /// </summary>
        public void Awake()
        {
            base.Awake();

            GameObject tile = Instantiate(TilePrefab, transform);
            tile.transform.position = transform.position;

            _timeForMoreGold = Random.Range(4, 7);
            GameObject goldObject = Instantiate(GoldPrefab, transform);
            goldObject.transform.position = transform.position;
            gold = goldObject.GetComponent<GoldController>();
            gold.Tile = this;
        }

        /// <summary>
        /// Gets the gold amount.
        /// </summary>
        /// <returns></returns>
        public int GetGoldAmount() { return gold.Gold; }

        /// <summary>
        /// Called when [round end].
        /// </summary>
        /// <param name="roundNumber">The round number.</param>
        public void OnRoundEnd(int roundNumber)
        {
            if (--_timeForMoreGold != 0) return;

            gold.GetGold();
            _timeForMoreGold = Random.Range(4, 7);
            _goldSpawnTime = Time.time;
        }

        public override void SetSprite(SpriteRenderer renderer) { renderer.sprite = _sprites[Random.Range(0, _sprites.Length - 1)]; }

        /// <summary>
        /// Starts this instance.
        /// </summary>
        public override void Start()
        {
            base.Start();
            GameController controller = GameController.GetGameController();
            controller.AddRoundEndListener(this);
        }

        public override void PlayerMovedOver(PlayerController player, bool doneMoving)
        {
            base.PlayerMovedOver(player, doneMoving);
            if (doneMoving)
                gold.PlayerTouched(player);
        }

        public void OnMouseEnter()
        {
            UIController.GetUIController().TooltipText = gold.Gold.ToString();
        }


        public void OnMouseExit()
        {
            UIController.GetUIController().TooltipText = "";
        }


        #endregion Public Methods


        #region Private Methods

        /// <summary>
        /// Glows the specified i.
        /// </summary>
        /// <param name="i">The i.</param>
        private void Glow(int i)
        {
            var outline = GetComponent<Outline>();
            outline.color = i;
            outline.enabled = true;
        }

        /// <summary>
        /// Stops the glowing.
        /// </summary>
        /// <param name="i">The i.</param>
        private void StopGlowing(int i)
        {
            var outline = GetComponent<Outline>();
            outline.color = 1;
            if (IsValidMove) Glow();
            else StopGlowing();
        }

        // Update is called once per frame
        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            StopGlowing(2);
            if (Time.time < _goldSpawnTime + 2) { Glow(2); }
        }

        #endregion Private Methods
    }
}