using UnityEngine;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour
{
    [SerializeField] private Text[] _txtPlayerScoresArray;
    [SerializeField] private Text[] _txtCurrentPlayerIndicatorArray;

    // Use this for initialization
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public void UpdateScoreboard(PlayerController[] players)
    {
        for (var i = 0; i < players.Length; i++)
            _txtPlayerScoresArray[i].text = players[i].Money.ToString();
    }

    public void UpdateCurrentTurn(PlayerController currentPlayer)
    {
        for (var i = 0; i < _txtCurrentPlayerIndicatorArray.Length; i++)
            _txtCurrentPlayerIndicatorArray[i].enabled = Equals(i + 1, currentPlayer.Id);
    }
}