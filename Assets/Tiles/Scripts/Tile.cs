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
        public float GetGoldHeat(PlayerController player)
        {
            GameController gameController = GameController.GetGameController();
            float heat = 0;
            ICollection<Tile> coinSpawners = gameController.TilebyType[TileType.CoinSpawner];

            foreach (Tile tile in coinSpawners)
            {
                CoinSpawnerController coinSpawnerController = tile as CoinSpawnerController;
                float gold = coinSpawnerController.GetGoldAmount();
                float distanceToTile = Mathf.Floor(Distance(coinSpawnerController) + 3 / 4);
                float distanceFromClosestPlayer = 100;
                foreach (PlayerController playerController in gameController.PlayerControllers)
                {
                    if (player != playerController)
                    {
                        distanceFromClosestPlayer = Mathf.Min(distanceFromClosestPlayer,
                            Distance(playerController.GetCurrentTile()));
                    }
                }

                float tileHeat = gold / distanceToTile;
                if (distanceFromClosestPlayer >= distanceToTile)
                    tileHeat *= 2f;
                else if (distanceFromClosestPlayer + 6 <= distanceToTile)
                    tileHeat *= 1f;
                else
                    tileHeat *= 0.5f;
                
                if (distanceFromClosestPlayer < 0.5f)
                    tileHeat = 0f;

                heat = heat + tileHeat;
            }

            return heat;
        }

        /// <summary>
        /// Gets the neighbours.
        /// </summary>
        /// <returns></returns>
        public List<Tile> GetNeighbours(bool includeWalls = false)
        {
            GameController gameController = GameController.GetGameController();
            List<Tile> list = new List<Tile>();
            foreach (var direction in _directions)
            {
                var pos = transform.position + direction;
                if (gameController.IsInBounds(pos))
                {
                    var tile = gameController.GetGameTile((int) pos.x, (int) pos.y);
                    if (includeWalls || tile.CanPassThrough()) list.Add(tile);
                }
            }
            return
                    list;
        }

        /// <summary>
        /// Glows this instance.
        /// </summary>
        public virtual void SetValidMove()
        {
            IsValidMove = true;
            this.Glow();
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

        public virtual void Start()
        {
            SetSprite(GetComponent<SpriteRenderer>());
            
        }

        /// <summary>
        /// Stops the glowing.
        /// </summary>
        public void Glow()
        {
            GetComponent<Outline>().enabled = true;
        }

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

        public virtual void CallPlayerMovedOver(PlayerController player, bool doneMoving)
        {
            foreach (IPlayerMovementListener playerMovementListener in _playerMovementListeners)
            {
                playerMovementListener.PlayerMovedOver(player, doneMoving);
            }
        }

        public virtual void OnMouseEnter()
        {
            if (IsValidMove)
            {
                UIController.GetUIController().TooltipText = "Valid Move";
            }

        }


        public virtual void OnMouseExit()
        {
            UIController.GetUIController().TooltipText = "";
        }

        #endregion Private Methods



    }
}