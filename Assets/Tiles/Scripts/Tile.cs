using System.Collections.Generic;
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

    public List<KeyValuePair<Tile, Vector3>> GetNeighbours()
    {
        var nieghbours = new List<KeyValuePair<Tile, Vector3>>();
        GameController gameController = GameController.GetGameController();
        foreach (Vector3 direction in _directions)
        {
            Vector3 pos = transform.position + direction;
            if (gameController.IsInBounds(pos))
            {
                Tile tile = gameController.GetGameTile((int) pos.x, (int) pos.y);
                if (tile.CanPassThrough())
                    nieghbours.Add(new KeyValuePair<Tile, Vector3>(tile, direction));
            }
        }
        return nieghbours;
    }

    public void Glow()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.yellow;
    }

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

    public virtual void Start()
    {
        SetSprite(GetComponent<SpriteRenderer>());
        _playerMovementListeners = new List<PlayerMovementListener>();
    }

    public void StopGlowing()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
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

    #endregion Private Methods
}