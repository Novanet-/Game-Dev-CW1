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
        GetComponent<SpriteRenderer>().sprite = this._sprites[Random.Range(0, this._sprites.Length - 1)];
    }

    #endregion Private Methods
}