
using System.Collections;
using UnityEngine;

public class BellWinchController : MonoBehaviour
{

    [Header("Rope")]
    [SerializeField] private ConfigurableJoint _joint;
    [SerializeField] private float _minRopeLength = 1f;
    [SerializeField] private float _stowedRopeLength = 0.5f;
    [SerializeField] private float _divingSpeed = 0.25f;

    [Header("Rope Visual")]
    [SerializeField] private float _shaftRotationSpeed = 360f;
    [SerializeField] private Transform _shaft;

    private Coroutine _stowCoroutine;

    private bool _isLocked;
    public bool IsLocked => _isLocked;
    public bool IsStowed { get; private set; } = true;


    public float RopeLength => _joint.linearLimit.limit;
    public float MinRopeLength => _minRopeLength;
    public float StowedRopeLength => _stowedRopeLength;

    public float Dive()
    {
        if (_stowCoroutine != null)
            return 0f;

        float delta = _divingSpeed * Time.fixedDeltaTime;
        return ChangeRopeLength(delta, _minRopeLength);
    }

    public float Surface()
    {
        if (_stowCoroutine != null)
            return 0f;

        float delta = -_divingSpeed * Time.fixedDeltaTime;
        return ChangeRopeLength(delta, _minRopeLength);
    }

    public void SetLocked(bool locked)
    {
        _isLocked = locked;
    }

    public void SetStowed(bool stowed)
    {
        if (_isLocked) return;
            
        IsStowed = stowed;

        if (_stowCoroutine != null)
        {
            StopCoroutine(_stowCoroutine);
            _stowCoroutine = null;
        }

        float targetLength = stowed
            ? _stowedRopeLength
            : _minRopeLength;

        _stowCoroutine = StartCoroutine(
            MoveRopeToLength(targetLength)
        );
    }

    private IEnumerator MoveRopeToLength(float targetLength)
    {
        while (!Mathf.Approximately(RopeLength, targetLength))
        {
            float newLength = Mathf.MoveTowards(
                RopeLength,
                targetLength,
                _divingSpeed * Time.fixedDeltaTime
            );

            ChangeRopeLength(
                newLength - RopeLength,
                _stowedRopeLength
            );

            yield return new WaitForFixedUpdate();
        }

        _stowCoroutine = null;
    }

    private float ChangeRopeLength(float delta, float minLength)
    {
        float currentLength = RopeLength;
        float newLength = Mathf.Max(
            minLength,
            currentLength + delta
        );

        float actualDelta = newLength - currentLength;

        if (Mathf.Approximately(actualDelta, 0f))
            return 0f;

        SoftJointLimit limit = _joint.linearLimit;
        limit.limit = newLength;
        _joint.linearLimit = limit;

        RotateShaft(actualDelta);

        return actualDelta;
    }

    private void RotateShaft(float delta)
    {
        _shaft.Rotate(
            Vector3.right,
            delta * _shaftRotationSpeed,
            Space.Self
        );
    }
}
