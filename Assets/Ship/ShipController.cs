using UnityEngine;

public class ShipController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _enginePoint;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private WaterSystems _waterSystems;

    [Header("Movement")]
    [SerializeField] private float _forwardSpeed = 100f;
    [SerializeField] private float _backwardSpeed = 50f;

    [Header("Steering")]
    [SerializeField] private float _turnSpeed = 30f;

    public Vector2 _input;

    private void FixedUpdate()
    {
        if (_input == Vector2.zero)
            return;

        float waterLevel = _waterSystems.GetWaterLevel(_enginePoint.position);

        if (waterLevel <= _enginePoint.position.y)
            return;

        float enginePower = _input.y;
        float steering = _input.x;

        float speed = enginePower >= 0f
            ? _forwardSpeed
            : _backwardSpeed;

        Vector3 forwardForce =
            transform.forward * enginePower * speed;

        Vector3 steeringForce =
            transform.right * steering * enginePower * _turnSpeed;

        Vector3 totalForce = forwardForce + steeringForce;

        _rb.AddForceAtPosition(totalForce, _enginePoint.position);
    }

    public void SetMoveDirection(float value)
    {
        _input.x = Mathf.Clamp(value, -1f, 1f);
    }

    public void SetEnginePower(float value)
    {
        _input.y = Mathf.Clamp(value, -1f, 1f);
    }
}