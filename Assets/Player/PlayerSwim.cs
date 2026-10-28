
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwim : MonoBehaviour
{
    [Header("Swim Settings")]
    [SerializeField] private InputAction _swimAction;
    [SerializeField] private WaterSystems _waterSystems;
    [SerializeField] private Transform _playerCamera;
    [SerializeField] private PlayerWalk _playerWalk;
    [SerializeField] private Rigidbody _rb;

    [SerializeField] private float speed = 4f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float waterDrag = 1.5f;

    private bool _isDived;
    private float _depth;

    private void OnEnable()
    {
        _swimAction.Enable();
    }

    private void OnDisable()
    {
        _swimAction.Disable();
    }

    private void Update()
    {
        float waterLevel = _waterSystems.GetWaterLevel(transform.position);
        _depth = waterLevel - transform.position.y;

        if (_depth > 0.2f && !_isDived)
        {
            _rb.useGravity = false;
            _isDived = true;

            if (_playerWalk != null)
                _playerWalk.SetWalkAbility(false);
        }
        else if (_depth < -0.2f && _isDived)
        {
            _rb.useGravity = true;
            _isDived = false;

            if (_playerWalk != null)
                _playerWalk.SetWalkAbility(true);
        }
    }

    private void FixedUpdate()
    {
        if (!_isDived)
            return;

        Vector3 input = _swimAction.ReadValue<Vector3>();

        Vector3 move =
            _playerCamera.forward * input.z +
            _playerCamera.right * input.x +
            Vector3.up * input.y;

        move = Vector3.ClampMagnitude(move, 1f);

        // Accelerate toward the desired swimming velocity.
        Vector3 targetVelocity = move * speed;

        _rb.linearVelocity = Vector3.MoveTowards(
            _rb.linearVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        // Gradually slow down in water.
        _rb.linearVelocity *=
            1f / (1f + waterDrag * Time.fixedDeltaTime);
    }
}
