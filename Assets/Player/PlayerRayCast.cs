using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerRayCast : MonoBehaviour
{
    public InputAction interactAction;
    public Transform cam;
    public float rayLength;

    private Interactive _currentInteractive;

    private void OnEnable()
    {
        interactAction.Enable();
    }

    private void OnDisable()
    {
        ReleaseCurrent();
        interactAction.Disable();
    }

    private void Update()
    {
        if (!interactAction.IsPressed())
        {
            ReleaseCurrent();
            return;
        }

        Interactive interactive = null;

        if (Physics.Raycast(cam.position, cam.forward, out RaycastHit hit, rayLength))
        {
            hit.collider.TryGetComponent(out interactive);
        }

        if (ReferenceEquals(interactive, _currentInteractive))
            return;

        ReleaseCurrent();

        if (interactive != null)
        {
            _currentInteractive = interactive;
            _currentInteractive.Interact(gameObject);
        }
    }

    private void ReleaseCurrent()
    {
        if (_currentInteractive == null)
            return;

        _currentInteractive.Release(gameObject);
        _currentInteractive = null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        if (cam != null)
            Gizmos.DrawLine(cam.position, cam.position + cam.forward * rayLength);
    }
}