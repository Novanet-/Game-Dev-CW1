using System.Collections.Generic;
using System.Linq;
using Assets.Scripts;
using cakeslice;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Tiles.Scripts
{
    public class Tile : MonoBehaviour
    {
        #region Protected Fields

        [SerializeField] protected Sprite[] _sprites;

        #endregion Protected Fields


        #region Private Fields

        private readonly HashSet<Vector3> _directions = new HashSet<Vector3>(new[] {Vector3.left, Vector3.right, Vector3.up, Vector3.down});
        private PlayerController _currentPlayer;
        private List<IPlayerMovementListener> _playerMovementListeners;

        #endregion Private Fields


        #region Public Properties

        [CanBeNull] public PlayerController CurrentPlayer { get { return _currentPlayer; } set { SetPlayer(value); } }
        public Vector2 Direction { get; set; }
        public bool IsValidMove { get; set; }
        public IEnumerable<Tile> Path { get; set; }

        #endregion Public Properties


        #region Public Methods

        /// <summary>
        /// Adds the player movement listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void AddPlayerMovementListener(IPlayerMovementListener listener)
        {
            _playerMovementListeners.Add(listener);
        }

        /// <summary>
        /// Awakes this instance.
        /// </summary>
        public virtual void Awake()
        {
            SetSprite(GetComponent<SpriteRenderer>());
            _playerMovementListeners = new List<IPlayerMovementListener>();
            StopGlowing();
        }

        /// <summary>
        /// Determines whether this instance [can land on].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can land on]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanLandOn()
        {
            return CurrentPlayer == null;
        }

        /// <summary>
        /// Determines whether this instance [can pass through].
        /// </summary>
        /// <returns>
        ///   <c>true</c> if this instance [can pass through]; otherwise, <c>false</c>.
        /// </returns>
        public virtual bool CanPassThrough()
        {
            return true;
        }

        /// <summary>
        /// Distances the specified destination.
        /// </summary>
        /// <param name="destination">The destination.</param>
        /// <returns></returns>
        public int Distance(Tile destination)
        {
            var tiles = new Queue<KeyValuePair<Tile, int>>();
            var visited = new HashSet<Tile>();
            tiles.Enqueue(new KeyValuePair<Tile, int>(this, 0));
            while (tiles.Count > 0)
            {
                KeyValuePair<Tile, int> pair = tiles.Dequeue();
                Tile tile = pair.Key;
                if (tile == destination) return pair.Value;

                foreach (Tile neighbour in tile.GetNeighbours())
                {
                    if (neighbour.CanPassThrough())
                        if (!visited.Contains(neighbour))
                        {
                            visited.Add(neighbour);
                            tiles.Enqueue(new KeyValuePair<Tile, int>(neighbour, pair.Value + 1));
                        }
                }
            }

            Debug.Log("Cannot Reach " + destination.transform.position + " from " + transform.position);
            return -1;
        }

        /// <summary>
        /// Gets the gold heat.
        /// </summary>
        /// <returns></returns>
        public float GetGoldHeat()
        {
            GameController gameController = GameController.GetGameController();
            float heat = 0;
            List<CoinSpawnerController> coinSpawners =
                    gameController.CoinSpawners.Select(
                                                       coinSpawnerLocation =>
                                                           gameController.GetGameTile(coinSpawnerLocation.Key, coinSpawnerLocation.Value) as
                                                                   CoinSpawnerController).ToList();

            foreach (CoinSpawnerController coinSpawnerController in coinSpawners)
            {
                float gold = coinSpawnerController.GetGoldAmount();
                float distance = Mathf.Floor(Distance(coinSpawnerController) + 3 / 4f);

                heat = heat + gold / distance;
            }

            return heat;
        }

        /// <summary>
        /// Gets the neighbours.
        /// </summary>
        /// <returns></returns>
        public List<Tile> GetNeighbours()
        {
            GameController gameController = GameController.GetGameController();
            return
                    _directions.Select(direction => transform.position + direction)
                               .Where(pos => gameController.IsInBounds(pos))
                               .Select(pos => gameController.GetGameTile((int) pos.x, (int) pos.y))
                               .Where(tile => tile.CanPassThrough())
                               .ToList();
        }

        /// <summary>
        /// Glows this instance.
        /// </summary>
        public virtual void Glow()
        {
            GetComponent<Outline>().enabled = true;
            IsValidMove = true;
        }

        /// <summary>
        /// Landeds the on.
        /// </summary>
        /// <param name="player">The player.</param>
        public virtual void LandedOn(PlayerController player)
        {
            Debug.Log("Landed on Tile at: " + transform.position.x + ", " + transform.position.y);
        }

        /// <summary>
        /// Removes the player movement listener.
        /// </summary>
        /// <param name="listener">The listener.</param>
        public void RemovePlayerMovementListener(IPlayerMovementListener listener)
        {
            _playerMovementListeners.Remove(listener);
        }

        /// <summary>
        /// Sets the sprite.
        /// </summary>
        /// <param name="renderer">The renderer.</param>
        public virtual void SetSprite(SpriteRenderer renderer)
        {
            var pos = 0;
            if (transform.position.x % 2 < 0.6) pos += 1;
            if (transform.position.y % 2 < 0.6) pos += 2;
            renderer.sprite = _sprites[pos];
        }

        public virtual void Start() { }

        /// <summary>
        /// Stops the glowing.
        /// </summary>
        public void StopGlowing()
        {
            GetComponent<Outline>().enabled = false;
            IsValidMove = false;
        }

        #endregion Public Methods


        #region Private Methods

        /// <summary>
        /// Sets the player.
        /// </summary>
        /// <param name="newPlayer">The new player.</param>
        private void SetPlayer([CanBeNull] PlayerController newPlayer)
        {
            PlayerController oldPlayer = CurrentPlayer;
            _currentPlayer = newPlayer;
            if (oldPlayer == null && newPlayer != null) foreach (IPlayerMovementListener listener in _playerMovementListeners) { listener.PlayerLandsOn(newPlayer); }
            else if (oldPlayer == newPlayer && oldPlayer != null) foreach (IPlayerMovementListener listener in _playerMovementListeners) { listener.PlayerRemainsOn(newPlayer); }
            else if (oldPlayer != null && newPlayer == null) foreach (IPlayerMovementListener listener in _playerMovementListeners) { listener.PlayerLeaves(CurrentPlayer); }
        }

        #endregion Private Methods
    }
}