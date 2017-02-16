using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

public class Tile : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    private void Start()
    {
        SpriteRenderer renderer = GetComponent<SpriteRenderer>();
        renderer.sprite = sprites[Random.Range(0, sprites.Length - 1)];
    }

    public void LandedOn(PlayerController player)
    {
        Debug.Log("Landed on Tile at: " + transform.position.x + ", " + transform.position.y);
    }
}