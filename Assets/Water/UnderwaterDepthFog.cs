using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class UnderwaterDepthFog : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Volume _globalVolume;
    [SerializeField] private Transform _target;

    [Header("Depth")]
    [SerializeField] private float _surfaceY = 0f;
    [SerializeField] private float _maxDepth = 500f;

    [Header("Fog")]
    [SerializeField] private float _surfaceMeanFreePath = 500f;
    [SerializeField] private float _deepMeanFreePath = 5f;

    [Header("Exposure")]
    [SerializeField] private float _surfaceExposure = 0f;
    [SerializeField] private float _deepExposure = 8f;

    [Header("Indoor values")]
    [SerializeField] private float _indoorMeanFreePath = 1000f;
    [SerializeField] private float _indoorExposure = 0f;

    [Header("Transition")]
    [SerializeField] private float _transitionSpeed = 3f;

    private Fog _fog;
    private Exposure _exposure;

    private bool _effectEnabled = false;

    private float _currentMeanFreePath;
    private float _currentExposure;

    private void Awake()
    {
        if (_globalVolume == null || _target == null)
        {
            Debug.LogError("Global Volume or Target is not assigned.", this);
            enabled = false;
            return;
        }

        if (!_globalVolume.profile.TryGet(out _fog))
        {
            Debug.LogError("Fog override was not found.", this);
            enabled = false;
            return;
        }

        if (!_globalVolume.profile.TryGet(out _exposure))
        {
            Debug.LogError("Exposure override was not found.", this);
            enabled = false;
            return;
        }

        _currentMeanFreePath = _fog.meanFreePath.value;
        _currentExposure = _exposure.fixedExposure.value;
    }

    private void Update()
    {
        float targetMeanFreePath;
        float targetExposure;

        if (_effectEnabled)
        {
            float depth = Mathf.Max(0f, _surfaceY - _target.position.y);
            float t = Mathf.Clamp01(depth / _maxDepth);
            t = Mathf.SmoothStep(0f, 1f, t);

            targetMeanFreePath = Mathf.Lerp(
                _surfaceMeanFreePath,
                _deepMeanFreePath,
                t
            );

            targetExposure = Mathf.Lerp(
                _surfaceExposure,
                _deepExposure,
                t
            );
        }
        else
        {
            targetMeanFreePath = _indoorMeanFreePath;
            targetExposure = _indoorExposure;
        }

        _currentMeanFreePath = Mathf.Lerp(
            _currentMeanFreePath,
            targetMeanFreePath,
            Time.deltaTime * _transitionSpeed
        );

        _currentExposure = Mathf.Lerp(
            _currentExposure,
            targetExposure,
            Time.deltaTime * _transitionSpeed
        );

        _fog.meanFreePath.Override(_currentMeanFreePath);
        _exposure.fixedExposure.Override(_currentExposure);
    }

    public void EnableDepthEffect()
    {
        _effectEnabled = true;
    }

    public void DisableDepthEffect()
    {
        _effectEnabled = false;
    }

    public void SetDepthEffectEnabled(bool enabled)
    {
        _effectEnabled = enabled;
    }
}