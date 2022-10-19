using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    public Cell[,] Grid { get; private set; }
    public Vector2Int GridSize { get; private set; }

    public float CellRadius { get; private set; }   // Grid 한 칸의 반지름
    public float CellDiameter { get; private set; } // Grid 한 칸의 지름

    public Cell DestinationCell { get; set; }

    public FlowField(float cellRadius, Vector2Int gridSize)
    {
        CellRadius = cellRadius;
        CellDiameter = cellRadius * 2f;
        GridSize = gridSize;
    }

    public void CreateGrid()
    {
        Grid = new Cell[GridSize.x, GridSize.y];

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                Vector3 worldPos = new Vector3(CellDiameter * x + CellRadius, 0, CellDiameter * y + CellRadius);
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
            foreach (Collider collider in obstacles)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Impassible"))
                    currCell.Cost = 255;
                else if (collider.gameObject.layer == LayerMask.NameToLayer("RoughTerrain"))
                    currCell.Cost = 3;
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
        float percentX = worldPos.x / (GridSize.x * CellDiameter);
        float percentY = worldPos.z / (GridSize.y * CellDiameter);

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

    List<Cell> GetNeighborCells(Vector2Int gridIndex, List<GridDirection> directions)
    {
        List<Cell> neightborCells = new List<Cell>();

        for (int i = 0; i < directions.Count; i++)
        {
            Vector2Int currDir = directions[i].Vector;

            Cell newNeighbor = FindCellByDirection(gridIndex, currDir);
            if (newNeighbor != null)
                neightborCells.Add(newNeighbor);
        }

        return neightborCells;
    }

    Cell FindCellByDirection(Vector2Int gridIndex, Vector2Int direction)
    {
        Vector2Int finalPos = gridIndex + direction;

        if (finalPos.x < 0 || finalPos.x >= GridSize.x || finalPos.y < 0 || finalPos.y >= GridSize.y)
            return null;
        else
            return Grid[finalPos.x, finalPos.y];
    }
}
