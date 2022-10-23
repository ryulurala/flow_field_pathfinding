using UnityEditor;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public enum DisplayFieldType
    {
        CostField,          // Only. Cost Field
        IntegrationField,   // Only Intergration Field
        AllIcons,           // Direction Field + Destination
        DestinationIcon,    // Only. Destination
    }

    public bool DisplayGrid;        // Grid 표시
    public DisplayFieldType DisplayType;


    public Vector2Int GridSize;     // Grid Size 지정
    public float CellRadius;        // Grid 한 칸의 반지름

    public FlowField CurrFlowField;

    public Sprite PickSprite;       // 목적지 아이콘
    public Sprite CantGoSprite;     // 갈 수 없는 표시 아이코
    public Sprite NorthSprite;      // 위 방향 아이콘


    void Reset()
    {
        DisplayGrid = true;
        DisplayType = DisplayFieldType.AllIcons;

        GridSize = new Vector2Int(40, 25);
        CellRadius = 0.5f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CurrFlowField = new FlowField(CellRadius, GridSize);
            CurrFlowField.CreateGrid();

            CurrFlowField.CreateCostField();

            // 마우스로 찍은 곳 = DestinationCell
            Vector3 worldMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Cell destinationCell = CurrFlowField.FindCellFromWorldPos(worldMousePos);

            CurrFlowField.CreateIntegrationField(destinationCell);

            CurrFlowField.CreateFlowField();
        }
        DrawFlowField();
    }

    void OnDrawGizmos()
    {
        DrawGrid(Color.black);  // Grid 표시
        DrawCostField();        // Cost 표시
    }

    void DrawGrid(Color drawColor)
    {
        if (!DisplayGrid)
            return;

        Gizmos.color = drawColor;
        float biasX = (float)GridSize.x / 2;
        float biasY = (float)GridSize.y / 2;

        for (int x = 0; x < GridSize.x; x++)
        {
            for (int y = 0; y < GridSize.y; y++)
            {
                Vector3 center = new Vector3(CellRadius * 2 * x + CellRadius - biasX, 0, CellRadius * 2 * y + CellRadius - biasY);
                Vector3 size = Vector3.one * CellRadius * 2;

                Gizmos.DrawWireCube(center, size);
            }
        }
    }

    void DrawCostField()
    {
        if (CurrFlowField == null)
            return;

#if UNITY_EDITOR
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleCenter;

        switch (DisplayType)
        {
            case DisplayFieldType.CostField:
                foreach (Cell cell in CurrFlowField.Grid)
                    Handles.Label(cell.WorldPos, cell.Cost.ToString(), style);

                break;
            case DisplayFieldType.IntegrationField:
                foreach (Cell cell in CurrFlowField.Grid)
                    Handles.Label(cell.WorldPos, cell.BestCost.ToString(), style);

                break;
        }
#endif
    }

    void DrawFlowField()
    {
        // Clear icons
        foreach (Transform tf in transform)
            GameObject.Destroy(tf.gameObject);

        if (CurrFlowField == null)
            return;

        switch (DisplayType)
        {
            case DisplayFieldType.AllIcons:
                foreach (Cell cell in CurrFlowField.Grid)
                    DisplayCell(cell);

                break;
            case DisplayFieldType.DestinationIcon:
                DisplayCell(CurrFlowField.DestinationCell);

                break;
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
        else if (cell.BestDirection == CellDirection.North)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 0f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == CellDirection.South)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 180f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == CellDirection.East)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 90f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == CellDirection.West)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 270f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == CellDirection.NorthEast)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 45f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == CellDirection.NorthWest)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 315f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == CellDirection.SouthEast)
        {
            iconSR.sprite = NorthSprite;

            Quaternion newRot = Quaternion.Euler(90f, 135f, 0f);
            iconGO.transform.rotation = newRot;
        }
        else if (cell.BestDirection == CellDirection.SouthWest)
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
}
