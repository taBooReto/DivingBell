using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Rotatable : MonoBehaviour, Interactive
{
    public enum RotationAxis
    {
        X,
        Y,
        Z
    }

    [Header("Input")]
    [SerializeField] private InputAction _pointerPosition;
    [SerializeField] private Camera _camera;

    [Header("Rotation")]
    [SerializeField] private RotationAxis _rotationAxis = RotationAxis.Z;
    [SerializeField] private float _minAngle = -180f;
    [SerializeField] private float _maxAngle = 180f;

    [Header("Inertia")]
    [SerializeField] private bool _useInertia = true;
    [SerializeField] private float _drag = 2f;
    [SerializeField] private float _maxAngularSpeed = 1080f;

    [Header("Events")]
    [SerializeField] private UnityEvent<float> _onRotationChanged;

    private bool _isInteracting;
    private Vector3 _grabDirection;
    private float _angularVelocity;

    private Quaternion _initialRotation;
    private float _currentAngle;

    public float NormalizedRotation => _currentAngle < 0f
    ? _minAngle < 0f ? _currentAngle / -_minAngle : 0f
    : _maxAngle > 0f ? _currentAngle / _maxAngle : 0f;

    private Vector3 Axis => _rotationAxis switch
    {
        RotationAxis.X => Vector3.right,
        RotationAxis.Y => Vector3.up,
        _ => Vector3.forward
    };

    private void Awake()
    {
        _initialRotation = transform.localRotation;
        _camera = _camera ? _camera : Camera.main;
    }

    private void OnEnable()
    {
        _pointerPosition.Enable();
    }

    private void OnDisable()
    {
        _pointerPosition.Disable();
        Release();
    }

    public bool Interaction(GameObject interactor)
    {
        Ray ray = _camera.ScreenPointToRay(
            _pointerPosition.ReadValue<Vector2>()
        );

        if (!Physics.Raycast(ray, out RaycastHit hit))
            return false;

        if (hit.collider.transform != transform &&
            !hit.collider.transform.IsChildOf(transform))
            return false;

        Vector3 axis = transform.TransformDirection(Axis);
        Plane plane = new Plane(axis, transform.position);

        if (!plane.Raycast(ray, out float distance))
            return false;

        _grabDirection = (
            ray.GetPoint(distance) - transform.position
        ).normalized;

        if (_grabDirection.sqrMagnitude < 0.0001f)
            return false;

        _isInteracting = true;
        _angularVelocity = 0f;

        return true;
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        if (deltaTime <= 0f)
            return;

        if (_isInteracting)
        {
            Ray ray = _camera.ScreenPointToRay(
                _pointerPosition.ReadValue<Vector2>()
            );

            Vector3 axis = transform.TransformDirection(Axis);
            Plane plane = new Plane(axis, transform.position);

            if (plane.Raycast(ray, out float distance))
            {
                Vector3 targetDirection =
                    ray.GetPoint(distance) - transform.position;

                if (targetDirection.sqrMagnitude >= 0.0001f)
                {
                    targetDirection.Normalize();

                    float angle = Vector3.SignedAngle(
                        _grabDirection,
                        targetDirection,
                        axis
                    );

                    float previousAngle = _currentAngle;

                    _currentAngle = Mathf.Clamp(
                        _currentAngle + angle,
                        _minAngle,
                        _maxAngle
                    );

                    float appliedAngle = _currentAngle - previousAngle;

                    _angularVelocity = _useInertia
                        ? Mathf.Clamp(
                            appliedAngle / deltaTime,
                            -_maxAngularSpeed,
                            _maxAngularSpeed
                        )
                        : 0f;

                    _grabDirection = targetDirection;
                }
            }
        }
        else if (_useInertia && _angularVelocity != 0f)
        {
            float nextAngle = _currentAngle + _angularVelocity * deltaTime;

            _currentAngle = Mathf.Clamp(
                nextAngle,
                _minAngle,
                _maxAngle
            );

            if (!Mathf.Approximately(nextAngle, _currentAngle))
            {
                _angularVelocity = 0f;
            }
            else
            {
                _angularVelocity *= Mathf.Exp(-_drag * deltaTime);
            }

            if (Mathf.Abs(_angularVelocity) < 0.01f)
                _angularVelocity = 0f;
        }

        transform.localRotation =
            _initialRotation * Quaternion.AngleAxis(_currentAngle, Axis);

        _onRotationChanged?.Invoke(NormalizedRotation);
    }

    public void Release()
    {
        _isInteracting = false;

        if (!_useInertia)
            _angularVelocity = 0f;
    }
}