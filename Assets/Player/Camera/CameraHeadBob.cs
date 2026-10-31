using UnityEngine;

public class CameraHeadBob : MonoBehaviour
{
    [Header("Head Bob")]
    [SerializeField] private float _frequency = 8f;
    [SerializeField] private float _verticalAmplitude = 0.05f;
    [SerializeField] private float _horizontalAmplitude = 0.05f;

    [Header("Smoothing")]
    [SerializeField] private float _startSpeed = 8f;
    [SerializeField] private float _returnSpeed = 10f;

    private bool _isWalking;

    private Vector3 _startLocalPosition;
    private float _bobTimer;
    private float _bobStrength;

    private void Awake()
    {
        _startLocalPosition = transform.localPosition;
    }

    private void Update()
    {
        float targetStrength = _isWalking ? 1f : 0f;
        float smoothSpeed = _isWalking ? _startSpeed : _returnSpeed;

        _bobStrength = Mathf.MoveTowards(_bobStrength, targetStrength, smoothSpeed * Time.deltaTime);

        if (_bobStrength > 0f)
        {
            _bobTimer += Time.deltaTime * _frequency;

            float horizontalOffset = Mathf.Cos(_bobTimer * 0.5f) * _horizontalAmplitude * _bobStrength;

            float verticalOffset = Mathf.Sin(_bobTimer) * _verticalAmplitude * _bobStrength;

            Vector3 targetPosition = _startLocalPosition + new Vector3(horizontalOffset, verticalOffset, 0f);

            transform.localPosition = targetPosition;
        }
        else
        {
            _bobTimer = 0f;
            transform.localPosition = Vector3.Lerp(transform.localPosition, _startLocalPosition, _returnSpeed * Time.deltaTime);
        }
    }

    public void SetWalking(bool walking)
    {
        _isWalking = walking;
    }
}