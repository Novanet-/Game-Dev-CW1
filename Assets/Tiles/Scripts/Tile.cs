using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Private Fields

    [SerializeField] private Sprite[] _sprites;

    #endregion Private Fields

    #region Public Methods

    public virtual void LandedOn(PlayerController player)
    {
        Debug.Log("Landed on Tile at: " + transform.position.x + ", " + transform.position.y);
    }

    #endregion Public Methods

    #region Private Methods

    protected void Start()
    {
        var spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = _sprites[Random.Range(0, _sprites.Length - 1)];
        _playerMovementListeners = new List<PlayerMovementListener>();
    }

    #endregion Private Methods

    public virtual bool CanLandOn()
    {
        return CurrentPlayer == null;
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
        else if (oldPlayer == newPlayer && CurrentPlayer != null)
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


    private List<PlayerMovementListener> _playerMovementListeners;
    private PlayerController _currentPlayer;

    public void AddPlayerMovementListener(PlayerMovementListener listener)
    {
        _playerMovementListeners.Add(listener);
    }

    public void RemovePlayerMovementListener(PlayerMovementListener listener)
    {
        _playerMovementListeners.Remove(listener);
    }
}