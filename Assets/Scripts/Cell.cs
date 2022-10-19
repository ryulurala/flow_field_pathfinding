using UnityEngine;

public class Cell
{
    public Vector3 WorldPos;        // Unity 좌표
    public Vector2Int GridIndex;

    byte _cost;
    public byte Cost { get => _cost; set => _cost = value >= byte.MaxValue ? byte.MaxValue : (byte)value; }
    public ushort BestCost;
    public GridDirection BestDirection;

    public Cell(Vector3 worldPos, Vector2Int gridIndex)
    {
        WorldPos = worldPos;
        GridIndex = gridIndex;

        Cost = 1;
        BestCost = ushort.MaxValue;
        BestDirection = GridDirection.None;
    }
}
