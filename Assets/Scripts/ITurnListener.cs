namespace Assets.Scripts
{
    public interface ITurnListener
    {
        void OnTurnStart(PlayerController player);
        void OnTurnEnd(PlayerController player);
    }
}