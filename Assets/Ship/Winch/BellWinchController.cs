using UnityEngine;

public class BellWinchController : MonoBehaviour
{
    [Header("Rope")]
    [SerializeField] private ConfigurableJoint _joint;
    [SerializeField] private float _minRopeLength = 1f;
    [SerializeField] private float _divingSpeed = 0.25f;

    [Header("Rope Visual")]
    [SerializeField] private float _shaftRotationSpeed = 360f;
    [SerializeField] private Transform _shaft;

    public float RopeLength => _joint.linearLimit.limit;
    public float MinRopeLength => _minRopeLength;
    

    public float Dive()
    {
        float delta = _divingSpeed * Time.fixedDeltaTime;
        return ChangeRopeLength(delta);
    }

    public float Surface()
    {
        float delta = _divingSpeed * Time.fixedDeltaTime;
        return ChangeRopeLength(-delta);
    }

    private float ChangeRopeLength(float delta)
    {
        float currentLength = RopeLength;
        float newLength = Mathf.Max(_minRopeLength, currentLength + delta);

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