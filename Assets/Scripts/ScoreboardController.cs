using UnityEngine;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour
{
    #region Private Fields

    [SerializeField] private Text[] _txtCurrentPlayerIndicatorArray;
    [SerializeField] private Text[] _txtPlayerScoresArray;

    #endregion Private Fields


    #region Public Methods

    public void UpdateCurrentTurn(PlayerController currentPlayer)
    {
        for (var i = 0; i < _txtCurrentPlayerIndicatorArray.Length; i++) _txtCurrentPlayerIndicatorArray[i].enabled = Equals(i + 1, currentPlayer.Id);
    }

    public void UpdateScoreboard(PlayerController[] players)
    {
        for (var i = 0; i < players.Length; i++) _txtPlayerScoresArray[i].text = players[i].Money.ToString();
    }

    #endregion Public Methods


    #region Private Methods

    // Use this for initialization
    private void Start() { }

    // Update is called once per frame
    private void Update() { }

    #endregion Private Methods
}