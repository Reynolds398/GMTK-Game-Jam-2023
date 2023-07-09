using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    public SpawnManager board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    public GameObject player;

    public float fallInterval = 1.0f;
    public float lockDelay = 0.5f;

    //Difficulty variables
    private int counter = 0;
    private int difficultyThreshold = 2;
    public int difficulty = 1;

    private float timer;
    private float lockTime;

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

        //Move left and right here

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
            this.lockTime = 0f;
        }

        return valid;
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

    public void MoveLeft()
    {
        Move(Vector2Int.left);
    }

    public void MoveRight()
    {
        Move(Vector2Int.right);
    }
}
