using System.Collections.Generic;
using Assets.Scripts;
using Assets.Tiles.Scripts;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour, ITurnListener
{
    public static PowerupSpawner Spawner { get; private set; }
    [SerializeField] private GameObject[] _powerupsPreFabs;


    void Awake()
    {
        Spawner = this;
    }


    // Use this for initialization
    void Start()
    {
        GameController gameController = GameController.GetGameController();
        gameController.AddTurnListener(this);
        TimeToPowerup = Random.Range(5, 15);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void OnTurnStart(PlayerController player)
    {
        if (--TimeToPowerup <= 0)
        {
            TimeToPowerup = Random.Range(5, 15);
            GameController gameController = GameController.GetGameController();
            List<Tile> paths = gameController.TilebyType[TileType.Path];
            Tile randomTile;
            do
            {
                randomTile = paths[Random.Range(0, paths.Count)];
            } while (randomTile.CurrentPlayer != null || randomTile.HasPowerUp);

            GameObject randomPowerupPrefab = _powerupsPreFabs[Random.Range(0, _powerupsPreFabs.Length)];

            GameObject powerupObject = Instantiate(randomPowerupPrefab);
            Powerup powerup = powerupObject.GetComponent<Powerup>();
            powerup.Tile = randomTile;
        }
    }

    public int TimeToPowerup { get; set; }


    public void OnTurnEnd(PlayerController player)
    {
    }
}