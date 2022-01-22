using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    public Tile GridPrefab;

    public int GridHeight = 11;
    public int GridWidth = 11;

    float _hexWidth = .69282f;
    float _hexHeight = .84f;
    public float _gap = 0.0f;

    Vector2 _startPos;

    private Dictionary<Vector2, Tile> _tiles = new Dictionary<Vector2, Tile>();

    private void Awake()
    {
        Instance = this;
    }

    public void Init()
    {
        AddHexGap();
        CalculateStartPosition();
        GenerateHexGrid();
    }

    private void AddHexGap()
    {
        _hexWidth += _hexWidth * _gap;
        _hexHeight += _hexHeight * _gap;
    }

    private void CalculateStartPosition()
    {
        float offset = 0;

        if (GridHeight / 2 % 2 != 0)
            offset = _hexWidth / 2;

        float x = -_hexWidth * (GridWidth / 2) - offset;
        float y = _hexHeight * 0.75f * (GridHeight) / 2;

        _startPos = new Vector2(x, y);
    }

    private void GenerateHexGrid()
    {
        for (int x = 0; x < GridWidth; x++)
        {
            for (int y = 0; y < GridHeight; y++)
            {
                var hex = Instantiate(GridPrefab);
                Vector2 hexPos = CalculateHexPosition(x, y);
                hex.transform.position = hexPos;
                hex.name = $"Hex {x},{y}";

                _tiles[new Vector2(x, y)] = hex;
            }           
        }
    }

    //Calculate Hex World Positions with respect to hex shape offset
    private Vector2 CalculateHexPosition(int x, int y)
    {
        float offset = 0;

        if (y % 2 != 0)
            offset = _hexWidth / 2;

        float xPos = _startPos.x + x * _hexWidth + offset;
        float yPos = _startPos.y - y * _hexHeight * 0.75f;

        return new Vector2(xPos, yPos);
    }

    //Calculate Hex Grid Positions with respect to hex shape offset
    public Vector2 CalculateHexCoord(Vector2 WorldPos)
    {
        float offset = 0;

        if (WorldPos.y % 2 != 0)
            offset = _hexWidth / 2;

        int xCoord = (int)Mathf.Ceil(((WorldPos.x - offset - _startPos.x) /_hexWidth));
        int yCoord = (int)Mathf.Ceil(-((WorldPos.y - _startPos.y) / (_hexHeight * 0.75f)));

        return new Vector2(xCoord, yCoord);
    }

    public Tile GetTileAtCoord(int x, int y)
    {
        return _tiles.Where(t => t.Key.x == x && t.Key.y == y).FirstOrDefault().Value;
    }
    public bool IsInBounds(Vector2 tilePos)
    {
        return ((int)tilePos.x >= 0 && (int)tilePos.x < GridWidth) && ((int)tilePos.y >= 0 && (int)tilePos.y < GridHeight);
    }

    public Tile GetPlayerUnitSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < GridWidth / 4 && t.Key.y < GridHeight - 2 && t.Value.Walkable).OrderBy(o => Random.value).First().Value;
    }
    public Tile GetEnemyUnitSpawnTile()
    {
        return _tiles.Where(t => t.Key.x > GridWidth - (GridWidth / 4) && t.Key.y < GridHeight - 2 && t.Value.Walkable).OrderBy(o => Random.value).First().Value;
    }

    public void TurnAllGridsOff()
    {
        foreach (var tile in _tiles)
        {
            tile.Value.SetHighlight(false);
        }
    }
}
