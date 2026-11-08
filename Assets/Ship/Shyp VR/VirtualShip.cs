using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class VirtualShip : MonoBehaviour
{
    [SerializeField] private Rigidbody _realShipRb;

    private Rigidbody _rb;

    private readonly HashSet<Rigidbody> _onBoard = new();
    private Quaternion _lastShipRotation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;
        _rb.interpolation = RigidbodyInterpolation.Interpolate;

        _lastShipRotation = _realShipRb.rotation;
    }

    private void FixedUpdate()
    {
        Quaternion currentRotation = _realShipRb.rotation;

        Quaternion deltaRotation =
            currentRotation * Quaternion.Inverse(_lastShipRotation);

        float deltaYaw = Vector3.SignedAngle(
            Vector3.ProjectOnPlane(_lastShipRotation * Vector3.forward, Vector3.up),
            Vector3.ProjectOnPlane(currentRotation * Vector3.forward, Vector3.up),
            Vector3.up
        );

        Quaternion yawRotation =
            Quaternion.AngleAxis(deltaYaw, Vector3.up);

        foreach (Rigidbody body in _onBoard)
        {
            if (body == null)
                continue;

            if (body.TryGetComponent(out ShipPlayerAdapter adapter))
            {
                adapter.ApplyShipRotation(yawRotation);
            }
            else
            {
                body.MoveRotation(yawRotation * body.rotation);
            }
        }

        _rb.MovePosition(_realShipRb.position);
        _rb.MoveRotation(currentRotation);

        _lastShipRotation = currentRotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Rigidbody body = other.attachedRigidbody;

        if (body == null ||
            body == _rb ||
            body == _realShipRb)
            return;

        _onBoard.Add(body);
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody body = other.attachedRigidbody;

        if (body != null)
            _onBoard.Remove(body);
    }
}