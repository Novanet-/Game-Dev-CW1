using System.Collections.Generic;
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

    public bool CanLandOn()
    {
        return Player == null;
    }

    public PlayerController Player
    {
        get { return _player; }
        set { setPlayer(value); }
    }


    private void setPlayer(PlayerController player)
    {
        if (this.Player == null && player != null)
        {
            foreach (PlayerMovementListener listener in _playerMovementListeners)
            {
                listener.PlayerLandsOn(player);
            }
        }
        else if (this.Player == player)
        {

            foreach (PlayerMovementListener listener in _playerMovementListeners)
            {
                listener.PlayerRemainsOn(player);
            }
        }
        else if (this.Player != null && player == null)
        {

            foreach (PlayerMovementListener listener in _playerMovementListeners)
            {
                listener.PlayerLeaves(this.Player);
            }
        }

        _player = player;
    }


    private List<PlayerMovementListener> _playerMovementListeners;
    private PlayerController _player;

    public void AddPlayerMovementListener(PlayerMovementListener listener)
    {
        _playerMovementListeners.Add(listener);
    }

    public void RemovePlayerMovementListener(PlayerMovementListener listener)
    {
        _playerMovementListeners.Remove(listener);
    }
}