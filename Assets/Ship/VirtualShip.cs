
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class VirtualShip : MonoBehaviour
{
    [SerializeField] private Rigidbody _realShipRb;
    private Rigidbody _rb;

    [HideInInspector] public List<Rigidbody> onTheBoard = new List<Rigidbody>();

    private Vector3 lastPosition;
    private Quaternion lastRotation;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = true;

        lastPosition = _realShipRb.position;
        lastRotation = _realShipRb.rotation;
    }

    private void FixedUpdate()
    {
        _rb.position = _realShipRb.position;
        _rb.rotation = _realShipRb.rotation;
        _rb.position = lastPosition;
        _rb.rotation = lastRotation;

        Vector3 deltaPosition = _realShipRb.position - lastPosition;
        Quaternion deltaRotation = _realShipRb.rotation * Quaternion.Inverse(lastRotation);

        foreach (Rigidbody rb in onTheBoard)
        {
            if (rb == null) continue;

            rb.MovePosition(rb.position + deltaPosition);

            Vector3 dir = rb.position - _realShipRb.position;
            dir = deltaRotation * dir;
            Vector3 newPos = _realShipRb.position + dir;

            rb.MovePosition(newPos);
            rb.MoveRotation(deltaRotation * rb.rotation);
        }

        lastPosition = _realShipRb.position;
        lastRotation = _realShipRb.rotation;
    }

    private void OnTriggerEnter(Collider other)
    {
        Transform root = other.transform.root;
        Rigidbody rb = root.GetComponent<Rigidbody>();

        if (rb == null ||
            rb == _rb ||
            rb == _realShipRb ||
            onTheBoard.Contains(rb))
            return;

        root.SetParent(transform, true);
        onTheBoard.Add(rb);
    }

    private void OnTriggerExit(Collider other)
    {
        Transform root = other.transform.root;
        Rigidbody rb = root.GetComponent<Rigidbody>();

        if (rb == null || !onTheBoard.Contains(rb))
            return;

        root.SetParent(null, true);
        onTheBoard.Remove(rb);
    }

    //Apply object's weight on the real ship
    private void OnCollisionStay(Collision collision)
    {
        if (onTheBoard.Contains(collision.rigidbody))
        {
            ContactPoint contact = collision.contacts[0];

            Vector3 force = Vector3.down * collision.rigidbody.mass * Physics.gravity.magnitude;

            _realShipRb.AddForceAtPosition(force, contact.point);
        }
    }
}
