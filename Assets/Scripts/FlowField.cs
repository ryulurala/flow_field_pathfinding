using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    public Cell[,] Grid { get; private set; }
    public Vector2Int GridSize { get; private set; }

    public float CellRadius { get; private set; }

    float _cellDiameter;

    public FlowField(float cellRadius, Vector2Int gridSize)
    {
        CellRadius = cellRadius;
        _cellDiameter = cellRadius * 2f;
        GridSize = gridSize;
    }

    public void CreateGrid()
    {
        Grid = new Cell[GridSize.x, GridSize.y];

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                Vector3 worldPos = new Vector3(_cellDiameter * x + CellRadius, 0, _cellDiameter * y + CellRadius);
                Grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
            }
        }
    }

}
