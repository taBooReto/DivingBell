using System.Collections.Generic;
using UnityEngine;

public class RopeRenderer : MonoBehaviour
{
    [SerializeField] private GameObject _rope;
    [SerializeField] private Transform[] _points;

    private List<Transform> _ropeSegments = new List<Transform>();

    private void Start()
    {
        for (int i = 0; i < _points.Length - 1; i++)
        {
            GameObject rope = Instantiate(_rope);
            _ropeSegments.Add(rope.transform);
        }
    }

    private void Update()
    {
        for (int i = 0; i < _ropeSegments.Count; i++)
        {
            Vector3 start = _points[i].position;
            Vector3 end = _points[i + 1].position;

            Vector3 direction = end - start;

            _ropeSegments[i].position = start;
            _ropeSegments[i].rotation = Quaternion.FromToRotation(Vector3.up, direction);

            Vector3 scale = _ropeSegments[i].localScale;
            scale.y = direction.magnitude;
            _ropeSegments[i].localScale = scale;
        }
    }
}