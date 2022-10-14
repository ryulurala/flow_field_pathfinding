using UnityEngine;

public class Cell
{
    public Vector3 WorldPos;
    public Vector2Int GridIndex;

    public Cell(Vector3 worldPos, Vector2Int gridIndex)
    {
        WorldPos = worldPos;
        GridIndex = gridIndex;
    }
}
