
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSwim : MonoBehaviour
{
    public event System.Action<bool> OnDiveStateChanged;
    private WaterSystems _waterSystems;

    [SerializeField] private float speed = 4f;
    [SerializeField] private float acceleration = 3f;
    [SerializeField] private float waterDrag = 1.5f;

    private bool _isDived;
    private float _depth;
    private bool _canSwim = true;

    private void Start()
    {
        _waterSystems = Object.FindAnyObjectByType<WaterSystems>();
        Debug.Assert(_waterSystems != null, "WaterSystems not found in the scene.");
    }

    public void UpdateDiveState()
    {
        float waterLevel = _waterSystems.GetWaterLevel(transform.position);
        _depth = waterLevel - transform.position.y;

        if (_depth > 0.2f && !_isDived)
        {
            _isDived = true;
            OnDiveStateChanged?.Invoke(true);
        }
        else if (_depth < -0.2f && _isDived)
        {
            _isDived = false;
            OnDiveStateChanged?.Invoke(false);
        }
    }

    public void Swim(InputAction moveAction, InputAction swimVerticalAction, Transform camera, Rigidbody rb)
    {
        if (!_isDived || !_canSwim)
            return;

        Vector3 input = new Vector3();
        input.x = moveAction.ReadValue<Vector2>().x;
        input.z = moveAction.ReadValue<Vector2>().y;
        input.y = swimVerticalAction.ReadValue<float>();

        Vector3 move =
            camera.forward * input.z +
            camera.right * input.x +
            Vector3.up * input.y;

        move = Vector3.ClampMagnitude(move, 1f);

        // Accelerate toward the desired swimming velocity.
        Vector3 targetVelocity = move * speed;

        rb.linearVelocity = Vector3.MoveTowards(
            rb.linearVelocity,
            targetVelocity,
            acceleration * Time.fixedDeltaTime
        );

        // Gradually slow down in water.
        rb.linearVelocity *=
            1f / (1f + waterDrag * Time.fixedDeltaTime);
    }

    public void SetSwimAbility(bool canSwim)
    {
        _canSwim = canSwim;
    }
}
