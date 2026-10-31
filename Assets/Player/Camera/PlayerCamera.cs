using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private CinemachineCamera _virtualCamera;
    [SerializeField] private float _mouseSensitivity = 7;


    public Transform CameraTransform => _virtualCamera.transform;
    public Vector3 Position => _virtualCamera.transform.position;
    public Quaternion Rotation => _virtualCamera.transform.rotation;

    public bool IsRotationLocked { get; private set; } = false;
    public bool StabilizeRotation { get; private set; }

    private Vector2 _rotation;

    private void Start()
    {
        _virtualCamera.Prioritize();
        SyncRotation();
    }

    public void Look(InputAction lookAction)
    {
        if (!IsRotationLocked)
        {
            Vector2 mouseDelta = lookAction.ReadValue<Vector2>()
                * _mouseSensitivity * Time.deltaTime;

            _rotation.x += mouseDelta.x;
            _rotation.y += mouseDelta.y;
            _rotation.y = Mathf.Clamp(_rotation.y, -90f, 90f);
        }

        transform.localRotation = Quaternion.Euler(0f, _rotation.x, 0f);

        if (StabilizeRotation)
        {
            float yaw = transform.eulerAngles.y;

            _virtualCamera.transform.rotation = Quaternion.Euler(
                -_rotation.y,
                yaw,
                0f
            );
        }
        else
        {
            _virtualCamera.transform.localRotation =
                Quaternion.Euler(-_rotation.y, 0f, 0f);
        }
    }

    public void SetStabilizeRotation(bool enabled)
    {
        StabilizeRotation = enabled;
        SyncRotation();
    }

    public void SetRotationLocked(bool locked)
    {
        IsRotationLocked = locked;

        if (!locked) SyncRotation();
    }

    public void SyncRotation()
    {
        _rotation.x = transform.localEulerAngles.y;

        float pitch = _virtualCamera.transform.eulerAngles.x;

        if (pitch > 180f)
            pitch -= 360f;

        _rotation.y = Mathf.Clamp(-pitch, -90f, 90f);
    }

    public void SetRotation(Quaternion rotation)
    {
        Vector3 euler = rotation.eulerAngles;

        float pitch = euler.x;
        if (pitch > 180f)
            pitch -= 360f;

        _rotation.x = euler.y;
        _rotation.y = -pitch;

        transform.rotation = Quaternion.Euler(0f, _rotation.x, 0f);

        _virtualCamera.transform.localRotation =
            Quaternion.Euler(-_rotation.y, 0f, 0f);
    }

    public void LookAt(Vector3 direction)
    {
        SetRotation(Quaternion.LookRotation(direction));
    }

    public void SetRotationEuler(Vector3 rotation)
    {
        SetRotation(Quaternion.Euler(rotation));
    }

    public void SetMouseSensitivity(float value)
    {
        _mouseSensitivity = value;
    }

}
