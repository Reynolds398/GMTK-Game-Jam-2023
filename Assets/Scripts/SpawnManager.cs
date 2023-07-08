using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class SpawnManager : MonoBehaviour
{
    public Tilemap tilemap { get; private set; }
    public Pieces activePiece { get; private set; }
    public TetrominoData[] tetrominoes;
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);

    public bool falling = false;

    public RectInt Bounds
    {
        get
        {
            Vector2Int position = new Vector2Int(-boardSize.x / 2, -boardSize.y / 2);
            return new RectInt(position, boardSize);
        }
    }

    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Pieces>();

        for (int i = 0; i < tetrominoes.Length; i++)
        {
            tetrominoes[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    // Update is called once per frame
    void Update()
    {
        if (!falling)
        {

            falling = true;
        }
        
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominoes.Length);
        TetrominoData data = tetrominoes[random];

        if (random == 0) //I tetromino
        {
            int randomSpawn = Random.Range(-3, 3);
            spawnPosition = new Vector3Int(randomSpawn, 8);
        }
        else
        {
            int randomSpawn = Random.Range(-4, 4);
            spawnPosition = new Vector3Int(randomSpawn, 8);
        }
        activePiece.Initialize(this, spawnPosition, data);
        Set(activePiece);
    }

    public void Set(Pieces piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Pieces piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + piece.position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    public bool IsValidPosition(Pieces piece, Vector3Int position)
    {
        RectInt bounds = Bounds;

        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector3Int tilePosition = piece.cells[i] + position;

            //Check if tile will go out of bounds
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }

            //Check if new position will overlap with a tile
            if (tilemap.HasTile(tilePosition))
            {
                return false;
            }
        }

        return true;
    }
}
