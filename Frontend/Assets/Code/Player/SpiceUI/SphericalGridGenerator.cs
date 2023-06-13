using System.Collections.Generic;
using UnityEngine;

namespace Code.Player.SpiceUI
{
    [ExecuteAlways]
    public class SphericalGridGenerator : MonoBehaviour
    {
        private static readonly List<Vector3> _ADJACENCIES =
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
        private float GridConstant,
            Scale,
            GizmoRadius;

        [Range(0, 1)]
        [SerializeField]
        private float VirtRealLerp;

        [Range(0, 1)]
        [SerializeField]
        private float AdjacencyLineMagnitude;

        [SerializeField]
        private Vector3 Rotation;

        [SerializeField]
        private bool DrawCubes;

        [SerializeField]
        private GameObject NodePrefab;

        private readonly List<(Vector3, Vector3)> lines = new();
        private readonly List<Vector3> positions = new();

        private void Start()
        {
            if (!Application.isPlaying)
                return;

            GeneratePositions();

            foreach (Vector3 pos in positions)
                Instantiate(NodePrefab, pos, Quaternion.identity, transform);
        }

        private void OnDrawGizmos()
        {
            foreach (Vector3 pos in positions)
                if (DrawCubes)
                    Gizmos.DrawWireCube(pos, Vector3.one * GizmoRadius);
                else
                    Gizmos.DrawWireSphere(pos, GizmoRadius);

            foreach ((Vector3, Vector3) pair in lines)
                Gizmos.DrawLine(pair.Item1, pair.Item2);
        }

        private void OnValidate()
        {
            GeneratePositions();
        }

        private void GeneratePositions()
        {
            positions.Clear();
            lines.Clear();

            Quaternion rotation = Quaternion.Euler(Rotation);

            float bound = Mathf.Floor(GridConstant);
            for (float i = -bound; i <= bound; i++)
                for (float j = -bound; j <= bound; j++)
                    for (float k = -bound; k <= bound; k++)
                    {
                        var virt = new Vector3(i, j, k);
                        var real = new Vector3(j + k, i + k, i + j);

                        if (virt.sqrMagnitude >= GridConstant * GridConstant)
                            continue;

                        Vector3 center = Scale * Vector3.LerpUnclamped(virt, real, VirtRealLerp);
                        center = rotation * center;

                        positions.Add(center);

                        foreach (Vector3 adjacency in _ADJACENCIES)
                        {
                            Vector3 direction = rotation * adjacency;
                            Vector3 end = center + direction * AdjacencyLineMagnitude;
                            lines.Add((center, end));
                        }
                    }

            Debug.Log(positions.Count.ToString());
        }
    }
}
