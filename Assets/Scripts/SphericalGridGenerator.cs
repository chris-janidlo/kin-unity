using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SphericalGridGenerator : MonoBehaviour
{
    private static readonly float
        Sqrt3 = Mathf.Sqrt(3f),
        TwoThirdsSqrt6 = Mathf.Sqrt(6f) * 2f / 3f,
        FccUnitPackingRadius = 1f / Mathf.Sqrt(2f);

    private static readonly List<Vector3> Adjacencies = new()
    {
        new Vector3(1, 1, 0),
        new Vector3(1, -1, 0),
        new Vector3(-1, 1, 0),
        new Vector3(-1, -1, 0),
        new Vector3(1, 0, 1),
        new Vector3(1, 0, -1),
        new Vector3(-1, 0, 1),
        new Vector3(-1, 0, -1),
        new Vector3(0, 1, 1),
        new Vector3(0, 1, -1),
        new Vector3(0, -1, 1),
        new Vector3(0, -1, -1)
    };

    [SerializeField] private GridType grid;
    [Min(0.01f)] [SerializeField] private float gridRadius, packingRadius, gizmoRadius;
    [Range(0, 1)] [SerializeField] private float adjacencyLineMagnitude;
    [SerializeField] private Vector3 fccRotation;
    [SerializeField] private bool drawCubes;
    [SerializeField] private GameObject nodePrefab;

    private readonly List<(Vector3, Vector3)> _lines = new();
    private readonly List<Vector3> _positions = new();

    private void Start()
    {
        if (!Application.isPlaying) return;

        GeneratePositions();

        foreach (var pos in _positions) Instantiate(nodePrefab, pos, Quaternion.identity, transform);
    }

    private void OnDrawGizmos()
    {
        foreach (var pos in _positions)
            if (drawCubes)
                Gizmos.DrawWireCube(pos, Vector3.one * gizmoRadius);
            else
                Gizmos.DrawWireSphere(pos, gizmoRadius);

        foreach (var pair in _lines) Gizmos.DrawLine(pair.Item1, pair.Item2);

        Gizmos.DrawWireSphere(Vector3.zero, gridRadius);
    }

    private void OnValidate()
    {
        GeneratePositions();
    }

    private void GeneratePositions()
    {
        _positions.Clear();
        _lines.Clear();

        switch (grid)
        {
            case GridType.Cartesian:
                GeneratePositionsCartesian();
                break;
            case GridType.HexagonalClosePacked:
                GeneratePositionsHcp();
                break;
            case GridType.FaceCenteredCubicD3PlusRotation:
                GeneratePositionsFccD3PlusRotation();
                break;
            case GridType.FaceCenteredCubicModulo:
                GeneratePositionsFccModulo();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        Debug.Log(_positions.Count.ToString());
    }

    private void GeneratePositionsCartesian()
    {
        for (var x = (int)-gridRadius; x < gridRadius; x++)
        for (var y = (int)-gridRadius; y < gridRadius; y++)
        for (var z = (int)-gridRadius; z < gridRadius; z++)
            DuplicateToOctantsIfInGrid(x, y, z);
    }

    private void GeneratePositionsHcp()
    {
        var bound = Mathf.Ceil(gridRadius / packingRadius);
        for (var i = -bound; i <= bound; i++)
        for (var j = -bound; j <= bound; j++)
        for (var k = -bound; k <= bound; k++)
        {
            // from https://en.wikipedia.org/wiki/Close-packing_of_equal_spheres#Simple_HCP_lattice
            float
                x = (2f * i + (j + k) % 2f) * packingRadius,
                z = Sqrt3 * (j + k % 2f / 3f) * packingRadius,
                y = TwoThirdsSqrt6 * k * packingRadius;

            AddPositionIfInGrid(x, y, z);
        }
    }

    private void GeneratePositionsFccD3PlusRotation()
    {
        var rotation = Quaternion.Euler(fccRotation);

        var bound = Mathf.Ceil(gridRadius * FccUnitPackingRadius / packingRadius);
        for (var i = -bound; i <= bound; i++)
        for (var j = -bound; j <= bound; j++)
        for (var k = -bound; k <= bound; k++)
            if ((i + j + k) % 2 == 0)
            {
                var center = packingRadius / FccUnitPackingRadius * new Vector3(i, j, k);
                center = rotation * center;

                if (center.sqrMagnitude >= gridRadius * gridRadius) continue;

                _positions.Add(center);

                foreach (var adjacency in Adjacencies)
                {
                    var direction = rotation * adjacency;
                    var end = center + direction * adjacencyLineMagnitude;
                    _lines.Add((center, end));
                }
            }
    }

    private void GeneratePositionsFccModulo()
    {
        var bound = Mathf.Ceil(gridRadius / packingRadius);
        for (var i = -bound; i <= bound; i++)
        for (var j = -bound; j <= bound; j++)
        for (var k = -bound; k <= bound; k++)
        {
            float
                x = (2f * i + (j + k % 3f) % 2f) * packingRadius,
                z = Sqrt3 * (j + k % 3f / 3f) * packingRadius,
                y = TwoThirdsSqrt6 * k * packingRadius;

            AddPositionIfInGrid(x, y, z);
        }
    }

    private void AddPositionIfInGrid(float x, float y, float z)
    {
        if (x * x + y * y + z * z < gridRadius * gridRadius)
            _positions.Add(new Vector3(x, y, z));
    }

    private void DuplicateToOctantsIfInGrid(float x, float y, float z)
    {
        if (x * x + y * y + z * z >= gridRadius * gridRadius) return;

        if (x == 0 && y == 0 && z == 0)
        {
            _positions.Add(Vector3.zero);
            return;
        }

        _positions.Add(new Vector3(x, y, z));
        _positions.Add(new Vector3(x, y, -z));
        _positions.Add(new Vector3(x, -y, z));
        _positions.Add(new Vector3(x, -y, -z));
        _positions.Add(new Vector3(-x, y, z));
        _positions.Add(new Vector3(-x, y, -z));
        _positions.Add(new Vector3(-x, -y, z));
        _positions.Add(new Vector3(-x, -y, -z));
    }

    private enum GridType
    {
        Cartesian,
        HexagonalClosePacked,
        FaceCenteredCubicD3PlusRotation,
        FaceCenteredCubicModulo
    }
}