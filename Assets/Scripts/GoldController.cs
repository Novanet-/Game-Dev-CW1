using UnityEngine;

public class GoldController : MonoBehaviour, PlayerMovementListener
{
    #region Private Fields

    private int _spriteNum;

    [SerializeField] private Sprite[] _sprites;

    private Tile _tile;

    #endregion Private Fields


    #region Public Properties

    public int Gold { get; private set; }

    public Tile Tile
    {
        get { return _tile; }
        set
        {
            if (_tile != null) _tile.RemovePlayerMovementListener(this);
            _tile = value;
            _tile.AddPlayerMovementListener(this);
        }
    }

    #endregion Public Properties


    #region Public Methods

    public void getGold()
    {
        Gold = Gold + 10;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteNum = Mathf.Clamp(_spriteNum + 1, 0, _sprites.Length - 1);
        spriteRenderer.sprite = _sprites[_spriteNum];
    }

    public void PlayerLandsOn(PlayerController player) { GivePlayerGold(player); }

    public void PlayerLeaves(PlayerController player) { }

    public void PlayerRemainsOn(PlayerController player) { GivePlayerGold(player); }

    #endregion Public Methods


    #region Private Methods

    // Use this for initialization
    private void Awake()
    {
        Gold = 10;
        var spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteNum = 1;
        spriteRenderer.sprite = _sprites[_spriteNum];
    }

    private void GivePlayerGold(PlayerController player)
    {
        if (Gold > 0)
        {
            player.Money = player.Money + 10;
            Gold = Gold - 10;
            Debug.Log("Log Given Gold to Player" + player.Id + ", remaining gold: " + Gold);
        }

        var spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteNum = Mathf.Clamp(_spriteNum - 1, 0, _sprites.Length - 1);
        spriteRenderer.sprite = _sprites[_spriteNum];
    }

    // Update is called once per frame
    private void Update() { }

    #endregion Private Methods
}