Shader "FullScreen/CustomFullScreenPostProcess"
{
    Properties
    {
        // This property is necessary to make the CommandBuffer.Blit bind the source texture to _MainTex
        _MainTex("Main Texture", 2DArray) = "grey" {}
        _Desaturation("Desaturate enabled", Range(0,1)) = 0
        [Toggle]_InvertDepth("Invert Depth", Float) = 0
        [Toggle]_VisualizeDepth("Visualize Depth", Float) = 0
        _DepthExponent("Depth Explonent", Range(0.1,2)) = 1
        _DepthOffset("Depth Offset", Range(-5,5)) = 0
        [Toggle]_InvertColorGrade("Invert Color Grade", Float) = 0
        [Toggle]_VisualizeColorGrade("Visualize Color Grade", Float) = 0
        _ColorGradeExponent("Color Grade Explonent", Range(0.1,2)) = 1
        _ColorGrade("Color Grading", Color) = (1,1,1,1)
        _ColorGradeClampMin("Color Grade Clamp Low End", Range(-5,1)) = 0
        _ColorGradeClampMax("Color Grade Clamp High End", Range(-1,5)) = 1
    }

        HLSLINCLUDE

#pragma vertex Vert

#pragma target 4.5
#pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

#include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassCommon.hlsl"

    float _Desaturation, _InvertDepth, _VisualizeDepth, _DepthExponent, _DepthOffset, _DepthBrightness;
    float _InvertColorGrade, _VisualizeColorGrade, _ColorGradeExponent, _ColorGradeOffset, _ColorGradeClampMin, _ColorGradeClampMax;
    float3 _ColorGrade;
    TEXTURE2D_X(_MainTex);

    float4 FullScreenPass(Varyings varyings) : SV_Target
    {
        UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(varyings);
        float depth = LoadCameraDepth(varyings.positionCS.xy);
        PositionInputs posInput = GetPositionInput(varyings.positionCS.xy, _ScreenSize.zw, depth, UNITY_MATRIX_I_VP, UNITY_MATRIX_V);
        float colorGradeIntensity = posInput.linearDepth;
        float3 viewDirection = GetWorldSpaceNormalizeViewDir(posInput.positionWS);
        float3 color = _ColorGrade;

        if (_CustomPassInjectionPoint != CUSTOMPASSINJECTIONPOINT_BEFORE_RENDERING) {
            color = float3(CustomPassLoadCameraColor(varyings.positionCS.xy, 0));
        }

        float desaturationIntensity = 0.3 * color.x + 0.59 * color.y + 0.11 * color.z;
        color = desaturationIntensity * _Desaturation + color * (1 - _Desaturation);
        depth = pow(depth, _DepthExponent);
        if (_InvertDepth > 0) {
            depth = 1 - depth;
        }
        depth += _DepthOffset;
        float3 depthcolor = float3(depth, depth, depth);
        colorGradeIntensity = pow(colorGradeIntensity, _ColorGradeExponent);
        if (_InvertColorGrade > 0) {
            colorGradeIntensity = 1 - colorGradeIntensity;
        }
        float3 colorGradedColor = lerp(color, _ColorGrade, colorGradeIntensity);
        colorGradedColor = clamp(colorGradedColor, _ColorGradeClampMin, _ColorGradeClampMax);
        color = lerp(color, colorGradedColor, depth);
        if (_VisualizeDepth > 0) {
            return float4(depthcolor, 1);
        }
        else if (_VisualizeColorGrade > 0) {
            return float4(colorGradedColor, 1);
        }
        else {
            return float4(color, 1);
        }
    }

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "EnemyCustomPostProcess"

            ZWrite Off
            ZTest Always
            Blend SrcAlpha OneMinusSrcAlpha
            Cull Off

            HLSLPROGRAM
                #pragma fragment FullScreenPass
            ENDHLSL
        }
    }
    Fallback Off
}
