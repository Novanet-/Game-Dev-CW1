using System.Collections.Generic;
using Assets.Scripts;
using Assets.Tiles.Scripts;
using UnityEngine;

public class PowerupSpawner : MonoBehaviour, IRoundEndListener
{
    public static PowerupSpawner Spawner { get; private set; }
    [SerializeField] private GameObject[] _powerupsPreFabs;


    void Awake()
    {
        Spawner = this;
    }


	// Use this for initialization
	void Start () {
        GameController gameController = GameController.GetGameController();
        gameController.AddRoundEndListener(this);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnRoundEnd(int roundNumber)
    {
       GameController gameController = GameController.GetGameController();
        List<Tile> paths =  gameController.TilebyType[TileType.Path];
        Tile randomTile;
        Debug.Log("Generating a Powerup!");
        do
        {
            randomTile = paths[Random.Range(0, paths.Count)];
        } while (randomTile.CurrentPlayer != null);

        GameObject randomPowerupPrefab = _powerupsPreFabs[Random.Range(0, _powerupsPreFabs.Length)];

        GameObject powerupObject = Instantiate(randomPowerupPrefab);
        Powerup powerup = powerupObject.GetComponent<Powerup>();
        powerup.Tile = randomTile;
    }
}
