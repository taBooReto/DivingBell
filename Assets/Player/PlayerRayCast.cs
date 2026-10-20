using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRayCast : MonoBehaviour
{
    [SerializeField] private InputAction _interactAction;
    [SerializeField] private PlayerCamera _camera;
    [SerializeField] private float _rayLength = 2;

    public event Action OnNoInteraction;

    private void OnEnable()
    {
        _interactAction.Enable();
        _interactAction.performed += OnInteract;
    }

    private void OnDisable()
    {
        _interactAction.performed -= OnInteract;
        _interactAction.Disable();
    }

    private void OnInteract(InputAction.CallbackContext context)
    {
        if (_camera.IsRotationLocked)
            return;

        Interactive interactive = GetInteractive();

        if (interactive != null && interactive.Interaction(gameObject) )
            return;

        OnNoInteraction?.Invoke();
    }

    public Interactive GetInteractive()
    {
        if (Physics.Raycast(
                _camera.Position,
                _camera.Rotation * Vector3.forward,
                out RaycastHit hit,
                _rayLength))
        {
            if (hit.collider.TryGetComponent(out Interactive interactive))
                return interactive;
        }

        return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (_camera != null)
        {
            Gizmos.DrawLine(
                _camera.Position,
                _camera.Position + _camera.Rotation * Vector3.forward * _rayLength
            );
        }
    }
}