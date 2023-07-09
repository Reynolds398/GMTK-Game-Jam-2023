using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    public SpawnManager board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public int rotationIndex { get; private set; }
    public GameObject player;
    public GameController ai;

    public float fallInterval = 1.0f;
    public float lockDelay = 0.5f;

    //Difficulty variables
    private int counter = 0;
    private int difficultyThreshold = 2;
    public int difficulty = 1;

    private float timer;
    private float lockTime;

    //AI Variables
    private int leftCount = 0;
    private int rightCount = 0;
    private float moveTimer;
    private bool softDropOn = false;
    private bool softDropping = false;
    private int rightRotateCount = 0;
    private int leftRotateCount = 0;

    public void Initialize(SpawnManager board, Vector3Int position, TetrominoData data)
    {
        this.board = board;
        this.position = position;
        this.data = data;

        if (this.cells == null)
        {
            this.cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            this.cells[i] = (Vector3Int)data.cells[i];
        }
    }

    private void Start()
    {
        timer = Time.time + fallInterval;
        moveTimer = Time.time + (fallInterval/5);
        lockTime = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        //Increase difficulty
        if (counter >= difficultyThreshold && difficulty <= 5)
        {
            difficultyThreshold += difficulty * 2;
            counter = 0;
            RaiseDifficulty();
            
        }

        lockTime += Time.deltaTime;

        board.Clear(this);

        //Rotate right
        if (rightRotateCount > 0)
        {
            //1 is to the right
            Rotate(1);
            rightRotateCount--;
        }

        //Rotate left
        if (leftRotateCount > 0)
        {
            //-1 is to the left
            Rotate(-1);
            leftRotateCount--;
        }

        //Move left
        if (leftCount > 0)
        {
            if (Time.time >= moveTimer)
            {
                Move(Vector2Int.left);
                leftCount--;
                moveTimer = Time.time + (fallInterval / 5);
            }
        }

        //Move Right
        if (rightCount > 0)
        {
            if (Time.time >= moveTimer)
            {
                Move(Vector2Int.right);
                rightCount--;
                moveTimer = Time.time + (fallInterval / 5);
            }
        }

        //Soft drop when position is halfway down the map
        if (softDropOn)
        {
            //If halfway down, soft drop
            if (position.y <= 0)
            {
                softDropping = true;

                //Lock in place before drop
                leftCount = 0;
                rightCount = 0;
                rightRotateCount = 0;
                leftRotateCount = 0;

                if (Time.time >= moveTimer)
                {
                    Move(Vector2Int.down);
                    moveTimer = Time.time + (fallInterval / 5);
                }
            }
        }

        if (Time.time >= timer)
        {
            Step();
        }

        board.Set(this);
    }

    private void Step()
    {
        timer = Time.time + fallInterval;

        Move(Vector2Int.down);

        if (lockTime >= lockDelay && !player.GetComponent<PlayerControl>().death)
        {
            Lock();
            counter++;
        }
    }

    private void Lock()
    {
        board.Set(this);
        board.SpawnPiece();
        board.ClearLine();
        softDropping = false;
        ai.vertical = false;
    }

    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;

        bool valid = board.IsValidPosition(this, newPosition);

        if (valid)
        {
            position = newPosition;
            lockTime = 0f;
        }

        return valid;
    }

    private void Rotate(int direction)
    {
        int originalRotation = rotationIndex;

        rotationIndex = Wrap(rotationIndex + direction, 0, 4);
        ApplyRotationMatrix(direction);

        if (!TestWallKicks(rotationIndex, direction))
        {
            rotationIndex = originalRotation;
            ApplyRotationMatrix(-direction);
        }
    }

    private void ApplyRotationMatrix(int direction)
    {
        float[] matrix = Data.RotationMatrix;

        for (int i = 0; i < cells.Length; i++)
        {
            Vector3 cell = cells[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            cells[i] = new Vector3Int(x, y, 0);
        }
    }

    private bool TestWallKicks(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = GetWallKickIndex(rotationIndex, rotationDirection);

        for (int i = 0; i < data.wallKicks.GetLength(1); i++)
        {
            Vector2Int translation = data.wallKicks[wallKickIndex, i];

            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }

    private int GetWallKickIndex(int rotationIndex, int rotationDirection)
    {
        int wallKickIndex = rotationIndex * 2;

        if (rotationDirection < 0)
        {
            wallKickIndex--;
        }

        return Wrap(wallKickIndex, 0, data.wallKicks.GetLength(0));
    }

    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }

    private void RaiseDifficulty()
    {
        if (difficulty == 1) //Super Easy -> Easy
        {
            fallInterval = 0.8f;
        }
        else if (difficulty == 2) //Easy -> Normal
        {
            fallInterval = 0.6f;
        }
        else if (difficulty == 3) //Normal -> Hard
        {
            fallInterval = 0.2f;
        }
        else if (difficulty == 4) //Hard -> Very Hard
        {
            fallInterval = 0.05f;
        }

        difficulty++;
    }

    // ****************************************************************
    // -------------------Public Function for AI-----------------------
    // ****************************************************************

    public void MoveLeft(int count)
    {
        if (!softDropping)
        {
            leftCount = count;
        }
    }

    public void MoveRight(int count)
    {
        if (!softDropping)
        {
            rightCount = count;
        }
    }

    public void TurnOnSoftDrop()
    {
        softDropOn = true;
    }

    public void RotateRight()
    {
        if (!softDropping)
        {
            rightRotateCount++;
        }
    }

    public void RotateLeft()
    {
        if (!softDropping)
        {
            leftRotateCount++;
        }
    }
}
