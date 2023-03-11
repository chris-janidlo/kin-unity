using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SphericalGridGenerator : MonoBehaviour
{
    private static readonly List<Vector3> Adjacencies =
        new()
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

    [Min(0.01f)]
    [SerializeField]
    private float gridConstant,
        scale,
        gizmoRadius;

    [Range(0, 1)]
    [SerializeField]
    private float virtRealLerp;

    [Range(0, 1)]
    [SerializeField]
    private float adjacencyLineMagnitude;

    [SerializeField]
    private Vector3 rotation;

    [SerializeField]
    private bool drawCubes;

    [SerializeField]
    private GameObject nodePrefab;

    private readonly List<(Vector3, Vector3)> _lines = new();
    private readonly List<Vector3> _positions = new();

    private void Start()
    {
        if (!Application.isPlaying)
            return;

        GeneratePositions();

        foreach (var pos in _positions)
            Instantiate(nodePrefab, pos, Quaternion.identity, transform);
    }

    private void OnDrawGizmos()
    {
        foreach (var pos in _positions)
            if (drawCubes)
                Gizmos.DrawWireCube(pos, Vector3.one * gizmoRadius);
            else
                Gizmos.DrawWireSphere(pos, gizmoRadius);

        foreach (var pair in _lines)
            Gizmos.DrawLine(pair.Item1, pair.Item2);
    }

    private void OnValidate()
    {
        GeneratePositions();
    }

    private void GeneratePositions()
    {
        _positions.Clear();
        _lines.Clear();

        var rotation = Quaternion.Euler(this.rotation);

        var bound = Mathf.Floor(gridConstant);
        for (var i = -bound; i <= bound; i++)
            for (var j = -bound; j <= bound; j++)
                for (var k = -bound; k <= bound; k++)
                {
                    var virt = new Vector3(i, j, k);
                    var real = new Vector3(j + k, i + k, i + j);

                    if (virt.sqrMagnitude >= gridConstant * gridConstant)
                        continue;

                    var center = scale * Vector3.LerpUnclamped(virt, real, virtRealLerp);
                    center = rotation * center;

                    _positions.Add(center);

                    foreach (var adjacency in Adjacencies)
                    {
                        var direction = rotation * adjacency;
                        var end = center + direction * adjacencyLineMagnitude;
                        _lines.Add((center, end));
                    }
                }

        Debug.Log(_positions.Count.ToString());
    }
}
