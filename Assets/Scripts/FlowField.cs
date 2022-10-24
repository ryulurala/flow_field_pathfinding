using System.Collections.Generic;
using UnityEngine;

public class FlowField
{
    public Cell[,] Grid { get; private set; }
    public Cell DestinationCell { get; private set; }

    readonly float _cellRadius;       // Grid 한 칸의 반지름
    readonly float _cellDiameter;     // Grid 한 칸의 지름

    readonly Vector2Int _gridSize;
    readonly float _biasX;      // for. 음수 위치 표현
    readonly float _biasY;      // for. 음수 위치 표현


    public FlowField(float cellRadius, Vector2Int gridSize)
    {
        _cellRadius = cellRadius;
        _cellDiameter = cellRadius + cellRadius;

        _gridSize = gridSize;

        _biasX = (float)gridSize.x / 2;
        _biasY = (float)gridSize.y / 2;
    }

    public void CreateGrid()
    {
        // 2차원 Cell 배열(= Grid)를 생성한다.
        Grid = new Cell[_gridSize.x, _gridSize.y];

        for (int x = 0; x < _gridSize.x; x++)
        {
            for (int y = 0; y < _gridSize.y; y++)
            {
                Vector3 worldPos = new Vector3(_cellDiameter * x + _cellRadius - _biasX, 0, _cellDiameter * y + _cellRadius - _biasY);
                Grid[x, y] = new Cell(worldPos, new Vector2Int(x, y));
            }
        }
    }

    public void CreateCostField()
    {
        Vector3 cellHalfExtents = Vector3.one * _cellRadius;
        int terrainMask = LayerMask.GetMask("Impassable", "RoughTerrain");

        foreach (Cell currCell in Grid)
        {
            Collider[] obstacles = Physics.OverlapBox(currCell.WorldPos, cellHalfExtents, Quaternion.identity, terrainMask);
            foreach (Collider collider in obstacles)
            {
                if (collider.gameObject.layer == LayerMask.NameToLayer("Impassable"))
                    currCell.Cost = byte.MaxValue;
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
            Cell currentCell = cellToCheck.Dequeue();

            // 주변 셀 탐색: 4방향
            List<Cell> neighborCells = FindNeighborCells(currentCell.GridIndex, CellDirection.CardinalDirections);
            foreach (Cell neighborCell in neighborCells)
            {
                // 갈 수 없는 길
                if (neighborCell.Cost == byte.MaxValue)
                    continue;
                else if (neighborCell.Cost + currentCell.BestCost < neighborCell.BestCost)
                {
                    // Cost 갱신
                    neighborCell.BestCost = (ushort)(neighborCell.Cost + currentCell.BestCost);
                    cellToCheck.Enqueue(neighborCell);
                }
            }
        }
    }

    public void CreateFlowField()
    {
        foreach (Cell cell in Grid)
        {
            int bestCost = cell.BestCost;

            // 8방향 이동
            List<Cell> neighbors = FindNeighborCells(cell.GridIndex, CellDirection.CardinalAndIntercardinalDirections);
            foreach (Cell neighborCell in neighbors)
            {
                if (neighborCell.BestCost < bestCost)
                {
                    bestCost = neighborCell.BestCost;
                    cell.BestDirection = (CellDirection)(neighborCell.GridIndex - cell.GridIndex);
                }
            }
        }
    }

    public Cell FindCellFromWorldPos(Vector3 worldPos)
    {
        float posX = worldPos.x;
        float posY = worldPos.z;        // z axis

        int indexX = Mathf.RoundToInt((posX - _cellRadius + _biasX) / _cellDiameter);
        int indexY = Mathf.RoundToInt((posY - _cellRadius + _biasY) / _cellDiameter);
        indexX = Mathf.Clamp(indexX, 0, _gridSize.x - 1);
        indexY = Mathf.Clamp(indexY, 0, _gridSize.y - 1);

        return Grid[indexX, indexY];
    }

    List<Cell> FindNeighborCells(Vector2Int gridIndex, List<CellDirection> directions)
    {
        List<Cell> neightborCells = new List<Cell>();

        foreach (CellDirection currentDir in directions)
        {
            Cell newNeighbor = FindCellFromDirection(gridIndex, currentDir);
            if (newNeighbor != null)
                neightborCells.Add(newNeighbor);
        }

        return neightborCells;
    }

    Cell FindCellFromDirection(Vector2Int gridIndex, CellDirection direction)
    {
        Vector2Int finalPos = gridIndex + direction.Vector;

        if (finalPos.x < 0 || finalPos.x >= _gridSize.x || finalPos.y < 0 || finalPos.y >= _gridSize.y)
            return null;
        else
            return Grid[finalPos.x, finalPos.y];
    }
}
