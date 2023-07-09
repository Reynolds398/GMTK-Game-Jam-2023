using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pieces : MonoBehaviour
{
    public SpawnManager board { get; private set; }
    public TetrominoData data { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }

    public float fallInterval = 1.0f;
    public float lockDelay = 0.5f;

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

        if (lockTime >= lockDelay)
        {
            Lock();
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
}
