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

    Transform _root;
    Transform Root
    {
        get
        {
            if (_root == null)
            {
                _root = new GameObject("Root").transform;
                _root.SetParent(transform);
            }

            return _root;
        }
    }

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
            Cell nodeBelow = GridController.CurrFlowField.FindCellFromWorldPos(unit.transform.position);
            Vector3 movedir = new Vector3(nodeBelow.BestDirection.Vector.x, 0, nodeBelow.BestDirection.Vector.y);
            Rigidbody unitRB = unit.GetComponent<Rigidbody>();
            unitRB.velocity = movedir * MoveSpeed;
        }
    }

    void SpawnUnits()
    {
        Vector2Int gridSize = GridController.GridSize;
        Vector2 bias = new Vector2((float)gridSize.x / 2, (float)gridSize.y / 2);

        float nodeRadius = GridController.CellRadius;
        Vector2 spawnDist = new Vector2(gridSize.x * nodeRadius * 2 + nodeRadius - bias.x, gridSize.y * nodeRadius * 2 + nodeRadius - bias.y);
        int layerMask = LayerMask.GetMask("Impassable", "Units");

        float maxScale = Mathf.Max(UnitPrefab.transform.lossyScale.x, UnitPrefab.transform.lossyScale.y, UnitPrefab.transform.lossyScale.z);

        for (int i = 0; i < NumUnitPerSpawn; i++)
        {
            GameObject newUnit = Instantiate(UnitPrefab);
            newUnit.transform.SetParent(Root);
            _units.Add(newUnit);

            // for. Passible or Rough Terrain 셀과 겹치도록
            Vector3 newPos;
            do
            {
                newPos = new Vector3(Random.Range(-spawnDist.x, spawnDist.x), 0, Random.Range(-spawnDist.y, spawnDist.y));
                newUnit.transform.position = newPos;
            }
            while (Physics.OverlapSphere(newPos, maxScale, layerMask).Length > 0);
        }
    }

    void DestroyUnits()
    {
        Destroy(Root.gameObject);

        _units.Clear();
    }
}
