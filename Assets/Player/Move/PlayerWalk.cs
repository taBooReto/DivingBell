using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerWalk : MonoBehaviour
{
    [SerializeField] private float _speed = 2;

    private bool _canWalk = true;
    private Vector2 _direction;

    [SerializeField] private UnityEvent _onStartWalking;
    [SerializeField] private UnityEvent _onStopWalking;

    private bool _isWalking;

    public void Walk(InputAction moveAction, Rigidbody rb)
    {
        if (!_canWalk)
        {
            StopWalking();
            return;
        }

        _direction = moveAction.ReadValue<Vector2>();

        if (_direction != Vector2.zero)
        {
            StartWalking();

            Vector3 step = transform.forward * _direction.y + transform.right * _direction.x;
            step *= _speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + step);
        }
        else
        {
            StopWalking();
        }
    }

    private void StartWalking()
    {
        if (_isWalking) return;

        _isWalking = true;
        _onStartWalking?.Invoke();
    }

    private void StopWalking()
    {
        if (!_isWalking) return;

        _isWalking = false;
        _onStopWalking?.Invoke();
    }

    public void SetWalkAbility(bool value)
    {
        _canWalk = value;

        //if (!value)
        //{
        //    _rb.linearVelocity = Vector3.zero;
        //    _rb.angularVelocity = Vector3.zero;
        //}
    }

    public Vector2 GetWalkDir()
    {
        return _direction;
    }
}