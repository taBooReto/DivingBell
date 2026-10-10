using UnityEngine;
public class DivingBellShake : MonoBehaviour
{
    [SerializeField] private float _force = 2f;
    [SerializeField] private float _frequency = 3f;

    [SerializeField] private Rigidbody _rb;

    private float _seedX;
    private float _seedZ;

    private bool _isPlaying = false;

    private void Start()
    {
        _seedX = Random.Range(0f, 100f);
        _seedZ = Random.Range(100f, 200f);
    }

    private void FixedUpdate()
    {
        if (!_isPlaying) return;

        float time = Time.time * _frequency;

        Vector3 direction = new Vector3(
            Mathf.PerlinNoise(_seedX, time) * 2f - 1f,
            0f,
            Mathf.PerlinNoise(_seedZ, time) * 2f - 1f
        );

        _rb.AddForce(direction * _force, ForceMode.Force);
    }

    public void SetPlaying(bool value)
    {
        _isPlaying = value;
    }
}