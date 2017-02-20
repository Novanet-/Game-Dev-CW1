using UnityEngine;
using UnityEngine.UI;

public class ScoreboardController : MonoBehaviour
{
    [SerializeField] private Text[] _txtPlayerScoresArray;

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
}