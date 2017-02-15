using System;
using UnityEngine;

public class PlayerController : MonoBehaviour

{
    private Vector2 pos;
    private bool moving;

//    #region Public Enums
//
//    public enum MoveDirection
//    {
//        Up,
//        Down,
//        Left,
//        Right
//    }

//    #endregion Public Enums

    #region Public Methods

//    /// <exception cref="ArgumentOutOfRangeException">Condition.</exception>
//    public Vector3 MoveTile(Transform player, MoveDirection moveDirection)
//    {
//        Vector3 currentPos = player.position;
//
//        Vector3 newPos = currentPos;
//
//        switch (moveDirection)
//        {
//            case MoveDirection.Up:
//                newPos.y = currentPos.y + 1;
//                break;
//
//            case MoveDirection.Down:
//                newPos.y = currentPos.y - 1;
//                break;
//
//            case MoveDirection.Left:
//                newPos.y = currentPos.x - 1;
//                break;
//
//            case MoveDirection.Right:
//                newPos.y = currentPos.x + 1;
//                break;
//
//            default:
//                throw new ArgumentOutOfRangeException("moveDirection", moveDirection, null);
//        }
//
//        return newPos;
//    }

    public void CheckInput()
    {
        // WASD control
        // We add the direction to our position,
        // this moves the character 1 unit (32 pixels)
        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            pos += Vector2.right;
            moving = true;
        }

        // For left, we have to subtract the direction
        else if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            pos += Vector2.left;
            moving = true;
        }
        else if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
        {
            pos += Vector2.up;
            moving = true;
        }

        // Same as for the left, subtraction for down
        else if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
        {
            pos += Vector2.down;
            moving = true;
        }
    }

    #endregion Public Methods

    #region Private Methods

    // Use this for initialization
    private void Start()
    {
        // First store our current position when the
        // script is initialized.
        pos = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        CheckInput();

        if (!moving) return;

        // pos is changed when there's input from the player
        transform.position = pos;
        moving = false;

        //TODO: We also need to update the gamestate at this point, which contains a 2d array of tiles, for looking up tile info etc.
    }

    #endregion Private Methods
}