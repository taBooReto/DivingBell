using UnityEngine;

public class DivingBellCurrent : MonoBehaviour
{
    [Header("Current Force")]
    [SerializeField] private float _horizontalForce = 1f;
    [SerializeField] private float _currentSpeed = 0.1f;

    [SerializeField] private Rigidbody _rb;

    private float _noiseOffsetX;
    private float _noiseOffsetZ;
    private float _torqueNoiseOffset;

    private void Awake()
    {
        _noiseOffsetX = Random.Range(0f, 100f);
        _noiseOffsetZ = Random.Range(100f, 200f);
        _torqueNoiseOffset = Random.Range(200f, 300f);
    }

    private void FixedUpdate()
    {
        ApplyCurrentForce();
        ApplyCurrentTorque();
    }

    private void ApplyCurrentForce()
    {
        float time = Time.time * _currentSpeed;

        float currentX =
            Mathf.PerlinNoise(_noiseOffsetX, time) * 2f - 1f;

        float currentZ =
            Mathf.PerlinNoise(_noiseOffsetZ, time) * 2f - 1f;

        Vector3 currentForce = new Vector3(
            currentX * _horizontalForce,
            0,
            currentZ * _horizontalForce
        );

        _rb.AddForce(currentForce, ForceMode.Force);
    }

    private void ApplyCurrentTorque()
    {
        float time = Time.time;

        float torqueX =
            Mathf.PerlinNoise(_torqueNoiseOffset, time) * 2f - 1f;

        float torqueZ =
            Mathf.PerlinNoise(time, _torqueNoiseOffset) * 2f - 1f;

        Vector3 torque = new Vector3(
            torqueX,
            0f,
            torqueZ
        );

        _rb.AddTorque(torque, ForceMode.Force);
    }
}
