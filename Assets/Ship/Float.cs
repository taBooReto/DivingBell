
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Float : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform[] _floatPoints;
    private Rigidbody _rb;
    private WaterSystems _waterSystems;

    [Header("Buoyancy")]
    [SerializeField] private float _buoyancy = 60f;
    [SerializeField] private float _damping = 30f;

    [Header("Stability")]
    [SerializeField] private float _centerOfMassOffset = -0.5f;
    [SerializeField] private float _angularDragValue = 3f;
    [SerializeField] private float _maxAngularVelocity = 10f;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _waterSystems = Object.FindFirstObjectByType<WaterSystems>();

        _rb.centerOfMass = new Vector3(0f, _centerOfMassOffset, 0f);

        _rb.angularDamping = _angularDragValue;
        _rb.maxAngularVelocity = _maxAngularVelocity;
    }

    private void FixedUpdate()
    {
        if (_waterSystems == null || _floatPoints.Length == 0)
            return;

        for (int i = 0; i < _floatPoints.Length; i++)
        {
            float waterLevel = _waterSystems.GetWaterLevel(_floatPoints[i].position);
            float depth = waterLevel - _floatPoints[i].position.y;

            if (depth > 0f)
            {
                float buoyantForce = _buoyancy * depth;

                _rb.AddForceAtPosition(
                    Vector3.up * buoyantForce,
                    _floatPoints[i].position,
                    ForceMode.Force
                );
            }
        }

        float verticalVelocity = _rb.linearVelocity.y;
        float dampingForce = -verticalVelocity * _damping;

        _rb.AddForce(Vector3.up * dampingForce, ForceMode.Force);
    }
}
