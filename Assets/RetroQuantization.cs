using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

[Serializable]
[VolumeComponentMenu("Post-processing/Custom/Retro Quantization")]
public sealed class RetroQuantization :
    CustomPostProcessVolumeComponent,
    IPostProcessComponent
{
    [Tooltip("Number of color levels per RGB channel.")]
    public ClampedFloatParameter colorLevels =
        new ClampedFloatParameter(16f, 2f, 64f);

    [Tooltip("Strength of the effect.")]
    public ClampedFloatParameter intensity =
        new ClampedFloatParameter(1f, 0f, 1f);

    [Tooltip("Size of one virtual pixel.")]
    public ClampedFloatParameter pixelSize =
    new ClampedFloatParameter(4f, 1f, 32f);

    private const string ShaderName =
        "Hidden/Shader/RetroQuantization";

    private Material _material;

    public override CustomPostProcessInjectionPoint injectionPoint =>
        CustomPostProcessInjectionPoint.AfterPostProcess;

    public bool IsActive()
    {
        return _material != null && intensity.value > 0f;
    }

    public override void Setup()
    {
        Shader shader = Shader.Find(ShaderName);

        if (shader == null)
        {
            Debug.LogError($"Shader not found: {ShaderName}");
            return;
        }

        _material = CoreUtils.CreateEngineMaterial(shader);
    }

    public override void Render(
    CommandBuffer cmd,
    HDCamera camera,
    RTHandle source,
    RTHandle destination)
    {
        if (_material == null)
            return;

        _material.SetTexture("_MainTex", source);
        _material.SetFloat("_ColorLevels", colorLevels.value);
        _material.SetFloat("_Intensity", intensity.value);
        _material.SetFloat("_PixelSize", pixelSize.value);

        HDUtils.DrawFullScreen(cmd, _material, destination);
    }

    public override void Cleanup()
    {
        CoreUtils.Destroy(_material);
    }
}