using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    public GridController GridController;
    public GameObject UnitPrefab;

    public int NumUnitPerSpawn;
    public float MoveSpeed;

    List<GameObject> _units = new List<GameObject>();

    void Reset()
    {
        NumUnitPerSpawn = 10;
        MoveSpeed = 5f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
            SpawnUnits();
        if (Input.GetKeyDown(KeyCode.D))
            DestroyUnits();
    }

    void FixedUpdate()
    {
        if (GridController.CurrFlowField == null)
            return;

        foreach (GameObject unit in _units)
        {
            Cell nodeBelow = GridController.CurrFlowField.GetCellFromWorldPos(unit.transform.position);
            Vector3 movedir = new Vector3(nodeBelow.BestDirection.Vector.x, 0, nodeBelow.BestDirection.Vector.y);
            Rigidbody unitRB = unit.GetComponent<Rigidbody>();
            unitRB.velocity = movedir * MoveSpeed;
        }
    }

    void SpawnUnits()
    {
        Vector2Int gridSize = GridController.GridSize;
        float nodeRadius = GridController.CellRadius;
        Vector2 maxSpawnPos = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius, gridSize.y * nodeRadius * 2 + nodeRadius);
        int colMask = LayerMask.GetMask("Impassible", "Units");
        Vector3 newPos;
        for (int i = 0; i < NumUnitPerSpawn; i++)
        {
            GameObject newUnit = Instantiate(UnitPrefab);
            newUnit.transform.parent = transform;
            _units.Add(newUnit);

            do
            {
                newPos = new Vector3(Random.Range(0, maxSpawnPos.x), 0, Random.Range(0, maxSpawnPos.y));
                newUnit.transform.position = newPos;
            }
            while (Physics.OverlapSphere(newPos, 0.25f, colMask).Length > 0);
        }
    }

    void DestroyUnits()
    {
        foreach (GameObject go in _units)
            Destroy(go);

        _units.Clear();
    }
}
