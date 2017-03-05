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
        public void Awake()
        {
            base.Awake();
            GameObject goldObject = Instantiate(GoldPrefab, transform);
            goldObject.transform.position = transform.position;
            gold = goldObject.GetComponent<GoldController>();
            gold.Tile = this;
        }

        public int GetGoldAmount() { return gold.Gold; }

        public void OnRoundEnd(int roundNumber)
        {
            if (--_timeForMoreGold == 0)
            {
                gold.GetGold();
                _timeForMoreGold = Random.Range(4, 7);
                _goldSpawnTime = Time.time;
            }
        }

        public override void SetSprite(SpriteRenderer renderer) { renderer.sprite = _sprites[Random.Range(0, _sprites.Length - 1)]; }

        public override void Start()
        {
            base.Start();
            GameObject tile = Instantiate(TilePrefab, transform);
            tile.transform.position = transform.position;

            _timeForMoreGold = Random.Range(4, 7);

            GameController controller = GameController.GetGameController();
            controller.AddRoundEndListener(this);
        }

        #endregion Public Methods


        #region Private Methods

        private void Glow(int i)
        {
            var outline = GetComponent<Outline>();
            outline.color = i;
            outline.enabled = true;
        }

        private void StopGlowing(int i)
        {
            var outline = GetComponent<Outline>();
            outline.color = 1;
            if (IsValidMove) Glow();
            else StopGlowing();
        }

        // Update is called once per frame
        private void Update()
        {
            StopGlowing(2);
            if (Time.time < _goldSpawnTime + 2) { Glow(2); }
        }

        #endregion Private Methods
    }
}