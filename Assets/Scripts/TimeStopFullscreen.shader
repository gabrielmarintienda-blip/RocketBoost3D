Shader "Custom/TimeStopFullscreen"
{
    Properties
    {
        _Radius ("Radius", Float) = -1
        _EdgeWidth ("Edge Width", Float) = 0.15
        _Center ("Center (viewport)", Vector) = (0.5, 0.5, 0, 0)
        _Reverse ("Reverse", Float) = 0
        _Desaturate ("Desaturate", Range(0, 1)) = 0.35
        _RingColor ("Ring Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off
        ZTest Always
        Cull Off

        Pass
        {
            Name "TimeStopPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            float _Radius;
            float _EdgeWidth;
            float4 _Center;
            float _Reverse;
            float _Desaturate;
            float4 _RingColor;

            float4 Frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float4 col = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, uv);

                // Distance to the wave center, corrected by aspect ratio so the wave is a circle
                float2 d = uv - _Center.xy;
                d.x *= _ScreenParams.x / _ScreenParams.y;
                float dist = length(d);

                // 1 inside the wave (soft edge), 0 outside
                float inside = 1.0 - smoothstep(_Radius, _Radius + _EdgeWidth, dist);
                // Bright ring at the wave front
                float ring = smoothstep(_Radius - _EdgeWidth, _Radius, dist) * inside;

                // Reverse wave: the inside shows normal colors, the outside stays negative
                float mask = lerp(inside, 1.0 - inside, _Reverse);

                // The World look: negative colors, partly desaturated
                float3 negative = 1.0 - col.rgb;
                float gray = dot(negative, float3(0.299, 0.587, 0.114));
                float3 stopped = lerp(negative, gray.xxx, _Desaturate);

                float3 result = lerp(col.rgb, stopped, mask);
                result += ring * _RingColor.rgb * 0.6;

                return float4(result, col.a);
            }
            ENDHLSL
        }
    }
}