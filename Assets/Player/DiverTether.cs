
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class DiverTether : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputAction _lengthAction;

    [Header("References")]
    [SerializeField] private Rigidbody _playerRb;
    [SerializeField] private Transform _playerTetherPoint;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private TetherChain _segmentPrefab;

    [Header("Tether")]
    [SerializeField, Min(0.1f)] private float _tetherLength = 5f;
    [SerializeField, Min(1)] private int _tetherChainCount = 10;

    [Header("Length Control")]
    [SerializeField] private float _minLength = 1f;
    [SerializeField] private float _maxLength = 20f;
    [SerializeField] private float _lengthChangeSpeed = 2f;

    [Header("Simulation")]
    [SerializeField, Range(1, 40)] private int _iterations = 20;
    [SerializeField, Range(0f, 1f)] private float _damping = 0.98f;
    [SerializeField, Range(0f, 1f)] private float _bendStiffness = 0.15f;

    private Rigidbody _bellRb;
    private Rigidbody[] _segments;

    private Vector3[] _positions;
    private Vector3[] _previousPositions;

    private float _segmentLength;

    private void OnEnable()
    {
        _lengthAction.Enable();
    }

    private void OnDisable()
    {
        _lengthAction.Disable();
    }

    private void Start()
    {
        CreateTether();
    }

    private void CreateTether()
    {
        if (_playerRb == null ||
            _playerTetherPoint == null ||
            _lineRenderer == null ||
            _segmentPrefab == null)
        {
            Debug.LogError("DiverTether: Missing references!", this);
            enabled = false;
            return;
        }

        _bellRb = GetComponent<Rigidbody>();

        _tetherChainCount = Mathf.Max(1, _tetherChainCount);

        _maxLength = Mathf.Max(0.1f, _maxLength);
        _minLength = Mathf.Clamp(_minLength, 0.1f, _maxLength);
        _tetherLength = Mathf.Clamp(
            _tetherLength, _minLength, _maxLength
        );

        UpdateSegmentLength();

        int pointCount = _tetherChainCount + 2;

        _segments = new Rigidbody[_tetherChainCount];
        _positions = new Vector3[pointCount];
        _previousPositions = new Vector3[pointCount];

        Vector3 start = _bellRb.position;
        Vector3 end = _playerTetherPoint.position;

        for (int i = 0; i < pointCount; i++)
        {
            float t = (float)i / (pointCount - 1);
            Vector3 position = Vector3.Lerp(start, end, t);

            _positions[i] = position;
            _previousPositions[i] = position;
        }

        for (int i = 0; i < _tetherChainCount; i++)
        {
            TetherChain segment = Instantiate(
                _segmentPrefab,
                _positions[i + 1],
                Quaternion.identity
            );

            segment.name = $"Tether Segment {i}";

            Rigidbody rb = segment.Rb;
            rb.isKinematic = true;

            _segments[i] = rb;
        }

        _lineRenderer.useWorldSpace = true;
        _lineRenderer.loop = false;
        _lineRenderer.positionCount = pointCount;
    }

    private void UpdateSegmentLength()
    {
        _segmentLength =
            _tetherLength / (_tetherChainCount + 1);
    }

    private void FixedUpdate()
    {
        if (_segments == null)
            return;

        UpdateTetherLength();
        LimitPlayerDistance();

        int last = _positions.Length - 1;

        Vector3 start = _bellRb.position;
        Vector3 end = _playerTetherPoint.position;

        // Verlet integration.
        for (int i = 1; i < last; i++)
        {
            Vector3 current = _positions[i];

            Vector3 velocity =
                (current - _previousPositions[i]) * _damping;

            _positions[i] = current + velocity;
            _previousPositions[i] = current;
        }

        _positions[0] = start;
        _positions[last] = end;

        // Distance and bend constraints.
        for (int iteration = 0; iteration < _iterations; iteration++)
        {
            for (int i = 0; i < last; i++)
            {
                SolveDistance(i, i + 1, _segmentLength);
            }

            if (_bendStiffness > 0f)
            {
                for (int i = 0; i < last - 1; i++)
                {
                    SolveBend(i, i + 2);
                }
            }

            _positions[0] = start;
            _positions[last] = end;
        }

        for (int i = 0; i < _segments.Length; i++)
        {
            _segments[i].MovePosition(_positions[i + 1]);
        }
    }

    private void UpdateTetherLength()
    {
        float input = _lengthAction.ReadValue<float>();

        if (Mathf.Abs(input) < 0.01f)
            return;

        _tetherLength +=
            input * _lengthChangeSpeed * Time.fixedDeltaTime;

        _tetherLength = Mathf.Clamp(
            _tetherLength,
            _minLength,
            _maxLength
        );

        UpdateSegmentLength();
    }

    private void LimitPlayerDistance()
    {
        Vector3 start = _bellRb.position;
        Vector3 end = _playerTetherPoint.position;

        Vector3 delta = end - start;
        float distance = delta.magnitude;

        if (distance <= _tetherLength || distance < 0.00001f)
            return;

        Vector3 direction = delta / distance;
        float excess = distance - _tetherLength;

        _playerRb.position -= direction * excess;

        float outwardSpeed = Vector3.Dot(
            _playerRb.linearVelocity,
            direction
        );

        if (outwardSpeed > 0f)
        {
            _playerRb.linearVelocity -= direction * outwardSpeed;
        }
    }

    private void SolveDistance(int a, int b, float maxDistance)
    {
        Vector3 delta = _positions[b] - _positions[a];
        float distance = delta.magnitude;

        if (distance <= maxDistance || distance < 0.00001f)
            return;

        Vector3 correction =
            delta * ((distance - maxDistance) / distance);

        int last = _positions.Length - 1;

        bool fixedA = a == 0 || a == last;
        bool fixedB = b == 0 || b == last;

        if (fixedA && fixedB)
            return;

        if (fixedA)
        {
            _positions[b] -= correction;
        }
        else if (fixedB)
        {
            _positions[a] += correction;
        }
        else
        {
            _positions[a] += correction * 0.5f;
            _positions[b] -= correction * 0.5f;
        }
    }

    private void SolveBend(int a, int b)
    {
        float maxDistance = _segmentLength * 2f;

        Vector3 delta = _positions[b] - _positions[a];
        float distance = delta.magnitude;

        if (distance >= maxDistance || distance < 0.00001f)
            return;

        Vector3 correction =
            delta.normalized *
            (maxDistance - distance) *
            _bendStiffness;

        int last = _positions.Length - 1;

        bool fixedA = a == 0 || a == last;
        bool fixedB = b == 0 || b == last;

        if (fixedA && fixedB)
            return;

        if (fixedA)
        {
            _positions[b] += correction;
        }
        else if (fixedB)
        {
            _positions[a] -= correction;
        }
        else
        {
            _positions[a] -= correction * 0.5f;
            _positions[b] += correction * 0.5f;
        }
    }

    private void LateUpdate()
    {
        if (_positions == null)
            return;

        _positions[0] = _bellRb.position;
        _positions[_positions.Length - 1] =
            _playerTetherPoint.position;

        for (int i = 0; i < _positions.Length; i++)
        {
            _lineRenderer.SetPosition(i, _positions[i]);
        }
    }
}
