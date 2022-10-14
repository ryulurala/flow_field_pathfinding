using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public enum FlowFieldDisplayType
    {
        CostField,
        IntegrationField,
    }

    public Vector2Int GridSize;
    public float CellRadius = 0.5f;

    public FlowField CurrFlowField;

    public FlowFieldDisplayType DisplayType;
    public bool DisplayGrid;

    void Reset()
    {
        CellRadius = 0.5f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InitializeFlowField();

            CurrFlowField.CreateCostField();
        }
    }

    void OnDrawGizmos()
    {
        if (DisplayGrid)
        {
            DrawGrid(GridSize, Color.black, CellRadius);
        }

        if (CurrFlowField == null)
            return;

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        switch (DisplayType)
        {
            case FlowFieldDisplayType.CostField:
                foreach (Cell cell in CurrFlowField.Grid)
                    Handles.Label(cell.WorldPos, cell.Cost.ToString(), style);
                break;
        }
    }

    void InitializeFlowField()
    {
        CurrFlowField = new FlowField(CellRadius, GridSize);
        CurrFlowField.CreateGrid();
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
