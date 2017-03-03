using System.Collections;
using System.Collections.Generic;
using cakeslice;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Protected Fields

    [SerializeField] protected Sprite[] _sprites;

    #endregion Protected Fields

    #region Private Fields

    private readonly HashSet<Vector3> _directions = new HashSet<Vector3>(new[] {Vector3.left, Vector3.right, Vector3.up, Vector3.down});

    private PlayerController _currentPlayer;

    private List<PlayerMovementListener> _playerMovementListeners;

    #endregion Private Fields

    #region Public Properties

    [CanBeNull] public PlayerController CurrentPlayer
    {
        get { return _currentPlayer; }
        set { SetPlayer(value); }
    }

    public Vector2 Direction { get; set; }

    #endregion Public Properties

    #region Public Methods

    public void AddPlayerMovementListener(PlayerMovementListener listener)
    {
        _playerMovementListeners.Add(listener);
    }

    public virtual bool CanLandOn()
    {
        return CurrentPlayer == null;
    }

    public virtual bool CanPassThrough()
    {
        return true;
    }

    public List<Tile> GetNeighbours()
    {
        var nieghbours = new List<Tile>();
        GameController gameController = GameController.GetGameController();
        foreach (Vector3 direction in _directions)
        {
            Vector3 pos = transform.position + direction;
            if (gameController.IsInBounds(pos))
            {
                Tile tile = gameController.GetGameTile((int) pos.x, (int) pos.y);
                if (tile.CanPassThrough())
                    nieghbours.Add(tile);
            }
        }
        return nieghbours;
    }

    public virtual void Glow()
    {

        GetComponent<Outline>().enabled = true;
        IsValidMove = true;
    }

    public bool IsValidMove { get; set; }

    public IEnumerable<Tile> Path { get; set; }

    public virtual void LandedOn(PlayerController player)
    {
        Debug.Log("Landed on Tile at: " + transform.position.x + ", " + transform.position.y);
    }

    public void RemovePlayerMovementListener(PlayerMovementListener listener)
    {
        _playerMovementListeners.Remove(listener);
    }

    public virtual void SetSprite(SpriteRenderer renderer)
    {
        var pos = 0;
        if (transform.position.x % 2 < 0.6)
            pos += 1;
        if (transform.position.y % 2 < 0.6)
            pos += 2;
        renderer.sprite = _sprites[pos];
    }

    public virtual void Awake()
    {
        
        SetSprite(GetComponent<SpriteRenderer>());
        _playerMovementListeners = new List<PlayerMovementListener>();
        StopGlowing();
    }
    public virtual void Start()
    {
    }

    public void StopGlowing()
    {
        GetComponent<Outline>().enabled = false;
        IsValidMove = false;
    }

    #endregion Public Methods

    #region Private Methods

    private void SetPlayer([CanBeNull] PlayerController newPlayer)
    {
        PlayerController oldPlayer = CurrentPlayer;
        _currentPlayer = newPlayer;
        if (oldPlayer == null && newPlayer != null)
            foreach (PlayerMovementListener listener in _playerMovementListeners)
                listener.PlayerLandsOn(newPlayer);
        else if (oldPlayer == newPlayer && oldPlayer != null)
            foreach (PlayerMovementListener listener in _playerMovementListeners)
                listener.PlayerRemainsOn(newPlayer);
        else if (oldPlayer != null && newPlayer == null)
            foreach (PlayerMovementListener listener in _playerMovementListeners)
                listener.PlayerLeaves(CurrentPlayer);
    }

    public int Distance(Tile destination)
    {
        Queue<KeyValuePair<Tile, int>> tiles = new Queue<KeyValuePair<Tile, int>>();
        HashSet<Tile> visited = new HashSet<Tile>();
        tiles.Enqueue(new KeyValuePair<Tile, int>(this, 0));
        while (tiles.Count > 0)
        {
            KeyValuePair<Tile, int> pair =  tiles.Dequeue();
            Tile tile = pair.Key;
            if (tile == destination)
            {
                return pair.Value;
            }
            else
            {
                foreach (Tile neighbour in tile.GetNeighbours())
                {
                    if (neighbour.CanPassThrough() && !visited.Contains(neighbour))
                    {
                        visited.Add(neighbour);
                        tiles.Enqueue(new KeyValuePair<Tile, int>(neighbour, pair.Value + 1));
                    }
                }
            }
        }
        Debug.Log("Cannot Reach " + destination.transform.position + " from " + this.transform.position);
        return -1;
    }

    public float GetGoldHeat()
    {
        GameController gameController = GameController.GetGameController();
        List<CoinSpawnerController> coinSpawners = new List<CoinSpawnerController>();
        float heat = 0;
        foreach (KeyValuePair<int, int> coinSpawnerLocation in gameController.CoinSpawners)
        {
            coinSpawners.Add((CoinSpawnerController)gameController.GetGameTile(coinSpawnerLocation.Key, coinSpawnerLocation.Value));
        }

        foreach (CoinSpawnerController coinSpawnerController in coinSpawners)
        {
            float gold = coinSpawnerController.GetGoldAmount();
            float distance =  Mathf.Floor(Distance(coinSpawnerController) + 3 / 4f);

            heat = heat + gold / distance;
        }


        return heat;
    }

    #endregion Private Methods
}