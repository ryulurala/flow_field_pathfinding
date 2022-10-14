using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    public Cell[,] Grid { get; private set; }
    public Vector2Int GridSize { get; private set; }

    public float CellRadius { get; private set; }
    public Cell DestinationCell;

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

    public void CreateCostField()
    {
        Vector3 cellHalfExtents = Vector3.one * CellRadius;
        int terrainMask = LayerMask.GetMask("Impassible", "RoughTerrain");

        foreach (Cell currCell in Grid)
        {
            Collider[] obstacles = Physics.OverlapBox(currCell.WorldPos, cellHalfExtents, Quaternion.identity, terrainMask);
            bool hasIncreasedCost = false;
            foreach (Collider collider in obstacles)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Impassible"))
                {
                    currCell.IncreaseCost(255);

                    continue;
                }
                else if (!hasIncreasedCost && collider.gameObject.layer == LayerMask.NameToLayer("RoughTerrain"))
                {
                    currCell.IncreaseCost(3);
                    hasIncreasedCost = true;
                }
            }
        }
    }

    public void CreateIntegrationField(Cell destinationCell)
    {
        DestinationCell = destinationCell;

        DestinationCell.Cost = 0;
        DestinationCell.BestCost = 0;

        Queue<Cell> cellToCheck = new Queue<Cell>();

        cellToCheck.Enqueue(DestinationCell);

        while (cellToCheck.Count > 0)
        {
            Cell currCell = cellToCheck.Dequeue();
            List<Cell> currNeighbors = GetNeighborCells(currCell.GridIndex, GridDirection.CardinalDirections);
            foreach (Cell currNeighbor in currNeighbors)
            {
                if (currNeighbor.Cost == byte.MaxValue)
                    continue;
                else if (currNeighbor.Cost + currCell.BestCost < currNeighbor.BestCost)
                {
                    currNeighbor.BestCost = (ushort)(currNeighbor.Cost + currCell.BestCost);
                    cellToCheck.Enqueue(currNeighbor);
                }
            }
        }
    }

    public Cell GetCellFromWorldPos(Vector3 worldPos)
    {
        float percentX = worldPos.x / (GridSize.x * _cellDiameter);
        float percentY = worldPos.z / (GridSize.y * _cellDiameter);

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.Clamp(Mathf.FloorToInt(GridSize.x * percentX), 0, GridSize.x - 1);
        int y = Mathf.Clamp(Mathf.FloorToInt(GridSize.y * percentY), 0, GridSize.y - 1);

        return Grid[x, y];
    }

    public void CreateFlowField()
    {
        foreach (Cell cell in Grid)
        {
            List<Cell> neighbors = GetNeighborCells(cell.GridIndex, GridDirection.AllDirections);

            int bestCost = cell.BestCost;

            foreach (Cell neighborCell in neighbors)
            {
                if (neighborCell.BestCost < bestCost)
                {
                    bestCost = neighborCell.BestCost;
                    cell.BestDirection = GridDirection.GetDirectionFromV2I(neighborCell.GridIndex - cell.GridIndex);
                }
            }
        }
    }

    List<Cell> GetNeighborCells(Vector2Int nodeIndex, List<GridDirection> directions)
    {
        List<Cell> neightborCells = new List<Cell>();

        foreach (Vector2Int currDir in directions)
        {
            Cell newNeighbor = GetCellAtRelativePos(nodeIndex, currDir);
            if (newNeighbor != null)
                neightborCells.Add(newNeighbor);
        }

        return neightborCells;
    }

    Cell GetCellAtRelativePos(Vector2Int originPos, Vector2Int relativePos)
    {
        Vector2Int finalPos = originPos + relativePos;

        if (finalPos.x < 0 || finalPos.x >= GridSize.x || finalPos.y < 0 || finalPos.y >= GridSize.y)
            return null;
        else
            return Grid[finalPos.x, finalPos.y];
    }
}
