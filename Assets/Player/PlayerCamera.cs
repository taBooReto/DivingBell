using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [Header("Look Settings")]
    [SerializeField] private InputAction _lookAction;
    [SerializeField] private CinemachineCamera _virtualCamera;
    [SerializeField] private float _mouseSensitivity = 7;

    public bool IsRotationLocked { get; private set; } = false;
    private Vector2 _rotation;

    private void Start()
    {
        _virtualCamera.Prioritize();
        SyncRotation();
    }

    private void OnEnable()
    {
        _lookAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnDisable()
    {
        _lookAction.Disable();
        //Cursor.lockState = CursorLockMode.None;
    }

    private void Update()
    {
        if (IsRotationLocked) return;

        Vector2 mouseDelta = _lookAction.ReadValue<Vector2>() * _mouseSensitivity * Time.deltaTime;

        _rotation.x += mouseDelta.x;
        _rotation.y += mouseDelta.y;
        _rotation.y = Mathf.Clamp(_rotation.y, -90f, 90f);

        _virtualCamera.transform.localRotation = Quaternion.Euler(-_rotation.y, 0f, 0f);

        transform.localRotation = Quaternion.Euler(0f, _rotation.x, 0f);
    }

    public void SetRotationLocked(bool locked)
    {
        IsRotationLocked = locked;

        if (!locked) SyncRotation();
    }

    public void SyncRotation()
    {
        _rotation.x = transform.eulerAngles.y;
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
