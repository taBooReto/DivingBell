using UnityEngine;

public class DiveController : MonoBehaviour
{
    [SerializeField] private Rigidbody[] _divingBodies;
    [SerializeField] private float _divingSpeed = 0.25f;
    private bool _isDiving = false;
    private void FixedUpdate()
    {
        if (!_isDiving) return;

        for (int i = 0; i < _divingBodies.Length; i++)
        {
            _divingBodies[i].MovePosition(_divingBodies[i].position - (Vector3.up * _divingSpeed * Time.fixedDeltaTime));
        }
    }

    public void SetDiving(bool value)
    {
        _isDiving = value;
    }
}
