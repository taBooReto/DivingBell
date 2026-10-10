Shader "Hidden/Shader/RetroQuantization"
{
    Properties
    {
        _MainTex("Main Texture", 2DArray) = "grey" {}
    }

    HLSLINCLUDE

    #pragma target 4.5
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/ShaderLibrary/ShaderVariables.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/FXAA.hlsl"
    #include "Packages/com.unity.render-pipelines.high-definition/Runtime/PostProcessing/Shaders/RTUpscale.hlsl"

    struct Attributes
    {
        uint vertexID : SV_VertexID;
        UNITY_VERTEX_INPUT_INSTANCE_ID
    };

    struct Varyings
    {
        float4 positionCS : SV_POSITION;
        float2 texcoord : TEXCOORD0;
        UNITY_VERTEX_OUTPUT_STEREO
    };

    Varyings Vert(Attributes input)
    {
        Varyings output;

        UNITY_SETUP_INSTANCE_ID(input);
        UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

        output.positionCS =
            GetFullScreenTriangleVertexPosition(input.vertexID);

        output.texcoord =
            GetFullScreenTriangleTexCoord(input.vertexID);

        return output;
    }

    TEXTURE2D_X(_MainTex);

    float _ColorLevels;
    float _Intensity;
    float _PixelSize;

    float3 QuantizeColor(float3 color, float levels)
    {
        levels = max(levels, 2.0);

        float steps = levels - 1.0;

        return round(color * steps) / steps;
    }

    float2 PixelateUV(float2 uv, float pixelSize)
    {
        pixelSize = max(pixelSize, 1.0);

        float2 screenSize = _ScreenSize.xy;
        float2 pixelResolution = screenSize / pixelSize;

        return (floor(uv * pixelResolution) + 0.5) / pixelResolution;
    }

    float4 CustomPostProcess(Varyings input) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

        float2 uv = PixelateUV(
            input.texcoord,
            _PixelSize
        );

        uv = ClampAndScaleUVForBilinearPostProcessTexture(uv);

        float3 sourceColor = SAMPLE_TEXTURE2D_X(
            _MainTex,
            s_point_clamp_sampler,
            uv
        ).rgb;

        float3 quantizedColor = QuantizeColor(
            sourceColor,
            _ColorLevels
        );

        float3 finalColor = lerp(
            sourceColor,
            quantizedColor,
            saturate(_Intensity)
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
            Name "Retro Quantization"

            ZWrite Off
            ZTest Always
            Blend Off
            Cull Off

            HLSLPROGRAM

            #pragma vertex Vert
            #pragma fragment CustomPostProcess

            ENDHLSL
        }
    }

    Fallback Off
}