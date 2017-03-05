using UnityEngine;

public class River : Tile, PlayerMovementListener
{
    #region Private Fields

    private readonly int MOVESTOPUSH = 1;
    private Tile toPushTo;

    #endregion Private Fields


    #region Public Methods

    public void PlayerLandsOn(PlayerController player) { MovePlayer(player); }

    public void PlayerLeaves(PlayerController player) { }

    public void PlayerRemainsOn(PlayerController player) { MovePlayer(player); }

    public override void SetSprite(SpriteRenderer renderer) { renderer.sprite = _sprites[Random.Range(0, _sprites.Length)]; }

    // Use this for initialization
    public override void Start()
    {
        base.Start();
        AddPlayerMovementListener(this);
        toPushTo = GameController.GetGameController()
                                 .GetGameTile((int) (transform.position.x + Direction.x), (int) (transform.position.y + Direction.y));
    }

    #endregion Public Methods


    #region Private Methods

    private void MovePlayer(PlayerController player)
    {
        //hack to avoid PLayerMoves never decrementing when being pushed by a river that can't push a player
        if (player.PlayerMoves > 2 * -MOVESTOPUSH && player.PlayerMoves <= 0)
        {
            player.PlayerMoves--;
            player.Move(toPushTo);
        }
    }

    // Update is called once per frame
    private void Update() { }

    #endregion Private Methods
}