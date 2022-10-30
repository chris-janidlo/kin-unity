using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SphericalGridGenerator : MonoBehaviour
{
    private static readonly float
        Sqrt3 = Mathf.Sqrt(3f),
        TwoThirdsSqrt6 = Mathf.Sqrt(6f) * 2f / 3f;

    [SerializeField] private GridType grid;
    [SerializeField] private float radius, gizmoRadius;
    [SerializeField] private bool drawCubes;
    [SerializeField] private GameObject nodePrefab;

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

        Gizmos.DrawWireSphere(Vector3.zero, radius);
    }

    private void OnValidate()
    {
        GeneratePositions();
    }

    private void GeneratePositions()
    {
        _positions.Clear();

        switch (grid)
        {
            case GridType.Cartesian:
                GeneratePositionsCartesian();
                break;
            case GridType.HexagonalClosePacked:
                GeneratePositionsHcp();
                break;
            case GridType.FaceCenteredCubic:
                GeneratePositionsFcc();
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }

        Debug.Log(_positions.Count.ToString());
    }

    private void GeneratePositionsCartesian()
    {
        for (var x = (int)-radius; x < radius; x++)
        for (var y = (int)-radius; y < radius; y++)
        for (var z = (int)-radius; z < radius; z++)
        {
            var position = new Vector3(x, y, z);
            if (position.sqrMagnitude < radius * radius)
                _positions.Add(position);
        }
    }

    private void GeneratePositionsHcp()
    {
        var bound = Mathf.Floor(radius) + 1;
        for (var i = -bound; i <= bound; i++)
        for (var j = -bound; j <= bound; j++)
        for (var k = -bound; k <= bound; k++)
        {
            // from https://en.wikipedia.org/wiki/Close-packing_of_equal_spheres#Simple_HCP_lattice
            float
                x = (2f * i + (j + k) % 2f) * .5f,
                z = Sqrt3 * (j + k % 2f / 3f) * .5f,
                y = TwoThirdsSqrt6 * k * .5f;

            var position = new Vector3(x, y, z);
            if (position.sqrMagnitude < radius * radius)
                _positions.Add(position);
        }
    }

    // seems to generate proper FCC, just off-centered
    private void GeneratePositionsFcc()
    {
        var bound = Mathf.Floor(radius) + 1;
        for (var i = -bound; i <= bound; i++)
        for (var j = -bound; j <= bound; j++)
        for (var k = -bound; k <= bound; k++)
        {
            float
                x = (2f * i + (j + k % 3f) % 2f) * .5f,
                z = Sqrt3 * (j + k % 3f / 3f) * .5f,
                y = TwoThirdsSqrt6 * k * .5f;

            var position = new Vector3(x, y, z);
            if (position.sqrMagnitude < radius * radius)
                _positions.Add(position);
        }
    }

    private enum GridType
    {
        Cartesian,
        HexagonalClosePacked,
        FaceCenteredCubic
    }
}