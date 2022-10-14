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
        AllIcons,
        DestinationIcon,
    }

    public Vector2Int GridSize;
    public float CellRadius = 0.5f;

    public FlowField CurrFlowField;

    public FlowFieldDisplayType DisplayType;
    public bool DisplayGrid;

    public Sprite PickSprite;
    public Sprite CantGoSprite;
    public Sprite NorthSprite;

    void Reset()
    {
        CellRadius = 0.5f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            InitializeFlowField();

            CurrFlowField.CreateCostField();

            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            Cell destinationCell = CurrFlowField.GetCellFromWorldPos(worldMousePos);
            CurrFlowField.CreateIntegrationField(destinationCell);

            CurrFlowField.CreateFlowField();
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

        DrawCostField();
        DrawFlowField();
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

    void DrawCostField()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        switch (DisplayType)
        {
            case FlowFieldDisplayType.CostField:
                foreach (Cell cell in CurrFlowField.Grid)
                    Handles.Label(cell.WorldPos, cell.Cost.ToString(), style);

                break;
            case FlowFieldDisplayType.IntegrationField:
                foreach (Cell cell in CurrFlowField.Grid)
                    Handles.Label(cell.WorldPos, cell.BestCost.ToString(), style);

                break;
        }
    }

    void DrawFlowField()
    {
        ClearCellDisplay();

        switch (DisplayType)
        {
            case FlowFieldDisplayType.AllIcons:
                DisplayAllCells();

                break;
            case FlowFieldDisplayType.DestinationIcon:
                DisplayDestinationCells();

                break;
        }
    }

    void ClearCellDisplay()
    {
        foreach (Transform tf in transform)
        {
            GameObject.Destroy(tf.gameObject);
        }
    }

    void DisplayCell(Cell cell)
    {
        GameObject iconGO = new GameObject("Sprite");
        SpriteRenderer iconSR = iconGO.AddComponent<SpriteRenderer>();
        iconGO.transform.SetParent(transform);
        iconGO.transform.position = new Vector3(cell.WorldPos.x, cell.WorldPos.y + 10f, cell.WorldPos.z);

        if (cell.Cost == 0)
        {
            iconSR.sprite = PickSprite;
            iconSR.color = Color.black;

            Quaternion newRot = Quaternion.Euler(90f, 0f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.Cost == byte.MaxValue)
        {
            iconSR.sprite = CantGoSprite;
            iconSR.color = Color.red;

            Quaternion newRot = Quaternion.Euler(90f, 0f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == GridDirection.North)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 0f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == GridDirection.South)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 180f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == GridDirection.East)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 90f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == GridDirection.West)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 270f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == GridDirection.NorthEast)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 45f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == GridDirection.NorthWest)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 315f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == GridDirection.SouthEast)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 135f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == GridDirection.SouthWest)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 225f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else
        {
            iconSR.sprite = NorthSprite;
        }
    }

    void DisplayAllCells()
    {
        if (CurrFlowField == null)
            return;

        foreach (Cell cell in CurrFlowField.Grid)
            DisplayCell(cell);
    }

    void DisplayDestinationCells()
    {
        if (CurrFlowField == null)
            return;

        DisplayCell(CurrFlowField.DestinationCell);
    }
}
