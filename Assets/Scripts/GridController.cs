using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    public Vector2Int GridSize;
    public float CellRadius = 0.5f;

    public FlowField CurrFlowField;

    void Reset()
    {
        CellRadius = 0.5f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            InitializeFlowField();
        }
    }

    void InitializeFlowField()
    {
        CurrFlowField = new FlowField(CellRadius, GridSize);
        CurrFlowField.CreateGrid();
    }
}
