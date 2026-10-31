using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class UnderwaterDepthFog : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaterSurface _water;
    [SerializeField] private Transform _target;

    [Header("Depth")]
    [SerializeField] private float _surfaceY = 0f;
    [SerializeField] private float _maxDepth = 500f;

    [Header("Maximum Distance")]
    [SerializeField] private float _surfaceMaxDistance = 100f;
    [SerializeField] private float _deepMaxDistance = 3.5f;

    [Header("Absorption Distance")]
    [SerializeField] private float _surfaceAbsorptionDistance = 100f;
    [SerializeField] private float _deepAbsorptionDistance = 5f;

    [Header("Scattering Color")]
    [SerializeField] private Color _surfaceScatteringColor = Color.cyan;
    [SerializeField] private Color _deepScatteringColor = Color.blue;

    private bool _effectEnabled = true;

    private void Update()
    {
        if (!_effectEnabled)
            return;

        float depth = Mathf.Max(0f, _surfaceY - _target.position.y);
        float t = Mathf.Clamp01(depth / _maxDepth);

        _water.maxRefractionDistance = Mathf.Lerp(
            _surfaceMaxDistance,
            _deepMaxDistance,
            t
        );

        _water.absorptionDistance = Mathf.Lerp(
            _surfaceAbsorptionDistance,
            _deepAbsorptionDistance,
            t
        );

        _water.scatteringColor = Color.Lerp(
            _surfaceScatteringColor,
            _deepScatteringColor,
            t
        );
    }

    public void EnableDepthEffect()
    {
        SetDepthEffectEnabled(true);
    }

    public void DisableDepthEffect()
    {
        SetDepthEffectEnabled(false);
    }

    public void SetDepthEffectEnabled(bool enabled)
    {
        _effectEnabled = enabled;

        if (!enabled)
        {
            _water.maxRefractionDistance = _surfaceMaxDistance;
            _water.absorptionDistance = _surfaceAbsorptionDistance;
            _water.scatteringColor = _surfaceScatteringColor;
        }
    }
}