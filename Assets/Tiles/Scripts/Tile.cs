using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Private Fields

    [SerializeField] protected Sprite[] _sprites;

    #endregion Private Fields

    #region Public Methods

    public virtual void LandedOn(PlayerController player)
    {
        Debug.Log("Landed on Tile at: " + transform.position.x + ", " + transform.position.y);
    }

    #endregion Public Methods

    #region Private Methods

    public virtual void Start()
    {
        SetSprite(GetComponent<SpriteRenderer>());
        _playerMovementListeners = new List<PlayerMovementListener>();
    }

    public virtual void SetSprite(SpriteRenderer renderer)
    {
        int pos = 0;
        if (transform.position.x % 2 < 0.6)
            pos += 1;
        if (transform.position.y % 2 < 0.6)
            pos += 2;
        renderer.sprite = _sprites[pos];
    }

    #endregion Private Methods

    public virtual bool CanLandOn()
    {
        return CurrentPlayer == null;
    }

    public virtual bool CanPassThrough()
    {
        return true;
    }

    [CanBeNull] public PlayerController CurrentPlayer
    {
        get { return _currentPlayer; }
        set { SetPlayer(value); }
    }

    public Vector2 Direction { get; set; }


    private void SetPlayer([CanBeNull] PlayerController newPlayer)
    {
        PlayerController oldPlayer = CurrentPlayer;
        _currentPlayer = newPlayer;
        if (oldPlayer == null && newPlayer != null)
        {
            foreach (PlayerMovementListener listener in _playerMovementListeners)
            {
                listener.PlayerLandsOn(newPlayer);
            }
        }
        else if (oldPlayer == newPlayer && oldPlayer != null)
        {

            foreach (PlayerMovementListener listener in _playerMovementListeners)
            {
                listener.PlayerRemainsOn(newPlayer);
            }
        }
        else if (oldPlayer != null && newPlayer == null)
        {

            foreach (PlayerMovementListener listener in _playerMovementListeners)
            {
                listener.PlayerLeaves(this.CurrentPlayer);
            }
        }

    }

    private readonly HashSet<Vector3> _directions = new HashSet<Vector3>(new Vector3[] {Vector3.left, Vector3.right, Vector3.up, Vector3.down});
    public List<KeyValuePair<Tile, Vector3>> GetNeighbours()
    {
        List<KeyValuePair<Tile, Vector3>> nieghbours = new List<KeyValuePair<Tile, Vector3>>();
        GameController gameController = GameController.GetGameController();
        foreach (Vector3 direction in _directions)
        {
            Vector3 pos = transform.position + direction;
            if (gameController.IsInBounds(pos))
            {
                Tile tile = gameController.GetGameTile((int)pos.x, (int)pos.y);
                if (tile.CanPassThrough())
                {
                    nieghbours.Add(new KeyValuePair<Tile, Vector3>(tile, direction));
                }
            }
        }
        return nieghbours;

    }


    private PlayerController _currentPlayer;

    private List<PlayerMovementListener> _playerMovementListeners;
    public void AddPlayerMovementListener(PlayerMovementListener listener)
    {
        _playerMovementListeners.Add(listener);
    }

    public void RemovePlayerMovementListener(PlayerMovementListener listener)
    {
        _playerMovementListeners.Remove(listener);
    }

    public void Glow()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.yellow;
    }

    public void StopGlowing()
    {
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.white;
    }
}