
Shader "FullScreen/UnderwaterFog"
{
    Properties
    {
        _UnderwaterFogColor("Fog Color", Color) = (0.02, 0.16, 0.20, 1)
        _UnderwaterFogStart("Fog Start", Float) = 5
        _UnderwaterFogEnd("Fog End", Float) = 40
        _UnderwaterFogDensity("Fog Density", Range(0, 3)) = 1
    }

    HLSLINCLUDE

    #pragma vertex Vert
    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch switch2

    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

    float4 _UnderwaterFogColor;
    float _UnderwaterFogStart;
    float _UnderwaterFogEnd;
    float _UnderwaterFogDensity;

    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);

        // Read camera depth
        float depth = LoadCameraDepth(varyings.positionCS.xy);

        PositionInputs posInput = GetPositionInput(
            varyings.positionCS.xy,
            _ScreenSize.zw,
            depth,
            UNITY_MATRIX_I_VP,
            UNITY_MATRIX_V
        );

        // Linear distance from camera
        float distanceToCamera = posInput.linearDepth;

        // Read original scene color
        float3 sceneColor = CustomPassLoadCameraColor(
            varyings.positionCS.xy,
            0
        );

        // Distance where fog begins
        float fogDistance = max(
            0.0,
            distanceToCamera - _UnderwaterFogStart
        );

        // Fog density
        float density = max(
            0.0,
            _UnderwaterFogDensity
        );

        // Exponential fog
        // At Fog End and Density = 1,
        // approximately 99% of scene color is absorbed.
        float fogRange = max(
            0.001,
            _UnderwaterFogEnd - _UnderwaterFogStart
        );

        float fogFactor = 1.0 - exp(
            -4.60517 * density * fogDistance / fogRange
        );

        fogFactor = saturate(fogFactor);

        // Mix scene color with underwater fog color
        float3 finalColor = lerp(
            sceneColor,
            _UnderwaterFogColor.rgb,
            fogFactor
        );

        return float4(finalColor, 1.0);
    }

    ENDHLSL

    SubShader
    {
        Tags
        {
            "RenderPipeline" = "HDRenderPipeline"
        }

        Pass
        {
            Name "Underwater Fog"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPass
            ENDHLSL
        }
    }

    Fallback Off
}
