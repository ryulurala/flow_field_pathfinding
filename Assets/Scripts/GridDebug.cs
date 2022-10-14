using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridDebug : MonoBehaviour
{
    public GridController GridControl;

    public Vector2Int GridSize;
    public float CellRadius;

    public bool DisplayGrid;

    FlowField _currFlowField;
    Sprite[] _ffIcons;

    void OnDrawGizmos()
    {
        if (DisplayGrid)
        {
            if (_currFlowField == null)
            {
                DrawGrid(GridControl.GridSize, Color.black, GridControl.CellRadius);
            }
            else
            {
                DrawGrid(GridSize, Color.green, CellRadius);
            }
        }
    }

    void DrawGrid(Vector2Int drawGridSize, Color drawColor, float drawCellRadius)
    {
        Gizmos.color = drawColor;
        for (int x = 0; x < drawGridSize.x; x++)
        {
            for (int y = 0; y < drawGridSize.y; y++)
            {
                Vector3 center = new Vector3(drawCellRadius * 2 * x + drawCellRadius, 0, drawCellRadius * 2 * y + drawCellRadius);
                Vector3 size = Vector3.one * drawCellRadius * 2;
                Gizmos.DrawWireCube(center, size);
            }
        }
    }
}
