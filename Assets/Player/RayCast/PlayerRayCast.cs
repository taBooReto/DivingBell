using System;
using UnityEngine;

public class PlayerRayCast : MonoBehaviour
{
    [SerializeField] private PlayerCamera _camera;
    [SerializeField] private float _rayLength = 2f;

    public event Action OnNoInteraction;

    private Interactive _activeInteractive;

    public void OnInteract()
    {
        if (_camera.IsRotationLocked)
            return;

        Interactive interactive = GetInteractive();

        if (interactive != null && interactive.Interaction(gameObject))
        {
            _activeInteractive = interactive;
            return;
        }

        OnNoInteraction?.Invoke();
    }

    public void OnInteractReleased()
    {
        ReleaseActiveInteractive();
    }

    private void Update()
    {
        if (_activeInteractive != null &&
            GetInteractive() != _activeInteractive)
        {
            ReleaseActiveInteractive();
        }
    }

    private void OnDisable()
    {
        ReleaseActiveInteractive();
    }

    private void ReleaseActiveInteractive()
    {
        if (_activeInteractive == null)
            return;

        _activeInteractive.Release();
        _activeInteractive = null;
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
                _camera.Position +
                _camera.Rotation * Vector3.forward * _rayLength
            );
        }
    }
}