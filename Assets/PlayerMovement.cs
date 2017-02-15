using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public enum MoveDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    // Use this for initialization
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void PlayerMoveInput()
    {

    }

    /// <exception cref="ArgumentOutOfRangeException">Condition.</exception>
    public Vector3 MoveTile(Transform player, MoveDirection moveDirection)
    {
        Vector3 currentPos = player.position;
        Vector3 newPos = currentPos;

        switch (moveDirection)
        {
            case MoveDirection.Up:
                newPos.y = currentPos.y + 1;
                break;
            case MoveDirection.Down:
                newPos.y = currentPos.y - 1;
                break;
            case MoveDirection.Left:
                newPos.y = currentPos.x - 1;
                break;
            case MoveDirection.Right:
                newPos.y = currentPos.x + 1;
                break;
            default:
                throw new ArgumentOutOfRangeException("moveDirection", moveDirection, null);
        }

        return newPos;
    }
}