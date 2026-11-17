
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

[ExecuteAlways]
public class UnderwaterDepthFog : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WaterSurface _water;
    [SerializeField] private Transform _target;

    [Header("Editor Preview")]
    [SerializeField] private bool _previewInEditor = false;
    [SerializeField] private bool _previewAboveWater = false;
    [SerializeField, Range(0f, 500f)]
    private float _previewDepth = 0f;

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

    private void OnEnable()
    {
        if (!Application.isPlaying && _previewInEditor)
            ApplyPreview();
    }

    private void OnValidate()
    {
        _maxDepth = Mathf.Max(0.01f, _maxDepth);
        _previewDepth = Mathf.Clamp(_previewDepth, 0f, _maxDepth);

#if UNITY_EDITOR
        if (!Application.isPlaying && _previewInEditor)
            UnityEditor.EditorApplication.delayCall += ApplyPreview;
#endif
    }

    private void Update()
    {
        if (!Application.isPlaying)
            return;

        if (!_effectEnabled || !_target)
            return;

        float depth = Mathf.Max(
            0f,
            _surfaceY - _target.position.y
        );

        ApplyEffect(depth);
    }

    private void ApplyPreview()
    {
        if (this == null || Application.isPlaying || !_previewInEditor)
            return;

        ApplyEffect(
            _previewAboveWater ? 0f : _previewDepth
        );
    }

    private void ApplyEffect(float depth)
    {
        if (!_water)
            return;

        float t = Mathf.Clamp01(
            depth / Mathf.Max(_maxDepth, 0.01f)
        );

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
            ApplyEffect(0f);
    }
}
