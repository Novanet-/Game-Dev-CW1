using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Private Fields

    [SerializeField] private Sprite[] _sprites;

    #endregion Private Fields

    #region Public Methods

    public void LandedOn(PlayerController player)
    {
        Debug.Log("Landed on Tile at: " + transform.position.x + ", " + transform.position.y);
    }

    #endregion Public Methods

    #region Private Methods

    private void Start()
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

    [NotNull] public PlayerController CurrentPlayer
    {
        get { return _currentPlayer; }
        set { SetPlayer(value); }
    }


    private void SetPlayer([CanBeNull] PlayerController newPlayer)
    {
        if (this.CurrentPlayer == null && newPlayer != null)
        {
            foreach (PlayerMovementListener listener in _playerMovementListeners)
            {
                listener.PlayerLandsOn(newPlayer);
            }
        }
        else if (this.CurrentPlayer == newPlayer)
        {

            foreach (PlayerMovementListener listener in _playerMovementListeners)
            {
                listener.PlayerRemainsOn(newPlayer);
            }
        }
        else if (this.CurrentPlayer != null && newPlayer == null)
        {

            foreach (PlayerMovementListener listener in _playerMovementListeners)
            {
                listener.PlayerLeaves(this.CurrentPlayer);
            }
        }

        _currentPlayer = newPlayer;
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