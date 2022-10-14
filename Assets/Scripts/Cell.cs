using UnityEngine;

public class Cell
{
    public Vector3 WorldPos;
    public Vector2Int GridIndex;

    public byte Cost;
    public ushort BestCost;

    public Cell(Vector3 worldPos, Vector2Int gridIndex)
    {
        WorldPos = worldPos;
        GridIndex = gridIndex;

        Cost = 1;
        BestCost = ushort.MaxValue;
    }

    public void IncreaseCost(int amount)
    {
        if (Cost == byte.MaxValue)
            return;
        else if (amount + Cost >= 255)
            Cost = byte.MaxValue;
        else
            Cost += (byte)amount;
    }
}
