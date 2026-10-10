using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private InputAction _lookAction;
    [SerializeField] private Transform _body;

    [SerializeField] private float _mouseSensitivity = 20f;
    [SerializeField] private float _height = 1.5f;

    private float _pitch;

    private void OnEnable()
    {
        _lookAction.Enable();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnDisable()
    {
        _lookAction.Disable();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void Update()
    {
        Vector2 mouseDelta = _lookAction.ReadValue<Vector2>();

        RotateBody(mouseDelta.x);
        RotateCameraVertically(mouseDelta.y);
    }

    private void LateUpdate()
    {
        FollowBody();
    }

    private void RotateBody(float mouseX)
    {
        float horizontalRotation =
            mouseX * _mouseSensitivity * Time.deltaTime;

        // Обертаємо тіло відносно світової осі Y.
        _body.Rotate(
            Vector3.up,
            horizontalRotation,
            Space.World
        );
    }

    private void RotateCameraVertically(float mouseY)
    {
        _pitch -= mouseY * _mouseSensitivity * Time.deltaTime;
        _pitch = Mathf.Clamp(_pitch, -90f, 90f);
    }

    private void FollowBody()
    {
        // Позиція камери не залежить від нахилу тіла.
        transform.position =
            _body.position + Vector3.up * _height;

        // Беремо від тіла тільки його поворот по світовій осі Y.
        float bodyYaw = _body.eulerAngles.y;

        // X — власний нахил камери.
        // Y — поточний поворот тіла.
        // Z завжди 0, тому камера не нахиляється набік.
        transform.rotation =
            Quaternion.Euler(_pitch, bodyYaw, 0f);
    }
}