using UnityEngine;

public class Tile : MonoBehaviour
{
    #region Private Fields

    [SerializeField] private Sprite[] sprites;

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
        var renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = sprites[Random.Range(0, sprites.Length - 1)];
    }

    #endregion Private Methods
}