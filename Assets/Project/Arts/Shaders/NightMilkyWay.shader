Shader "Project/World/Night Milky Way"
{
    Properties
    {
        _Tint("Tint", Color) = (0.78, 0.86, 1, 1)
        _MilkyWayTint("Milky Way Tint", Color) = (0.55, 0.68, 1, 1)
        _StarIntensity("Star Intensity", Range(0, 5)) = 1.6
        _MilkyWayIntensity("Milky Way Intensity", Range(0, 5)) = 1.25
        _MilkyWayWidth("Milky Way Width", Range(0.02, 0.45)) = 0.16
        _Fade("Fade", Range(0, 1)) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent-100"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            ZTest Always
            Cull Front

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 directionWS : TEXCOORD0;
            };

            CBUFFER_START(UnityPerMaterial)
                half4 _Tint;
                half4 _MilkyWayTint;
                half _StarIntensity;
                half _MilkyWayIntensity;
                half _MilkyWayWidth;
                half _Fade;
            CBUFFER_END

            float Hash13(float3 p)
            {
                p = frac(p * 0.1031);
                p += dot(p, p.yzx + 33.33);
                return frac((p.x + p.y) * p.z);
            }

            float ValueNoise(float3 p)
            {
                float3 i = floor(p);
                float3 f = smoothstep(0.0, 1.0, frac(p));

                float n000 = Hash13(i + float3(0, 0, 0));
                float n100 = Hash13(i + float3(1, 0, 0));
                float n010 = Hash13(i + float3(0, 1, 0));
                float n110 = Hash13(i + float3(1, 1, 0));
                float n001 = Hash13(i + float3(0, 0, 1));
                float n101 = Hash13(i + float3(1, 0, 1));
                float n011 = Hash13(i + float3(0, 1, 1));
                float n111 = Hash13(i + float3(1, 1, 1));

                float nx00 = lerp(n000, n100, f.x);
                float nx10 = lerp(n010, n110, f.x);
                float nx01 = lerp(n001, n101, f.x);
                float nx11 = lerp(n011, n111, f.x);
                float nxy0 = lerp(nx00, nx10, f.y);
                float nxy1 = lerp(nx01, nx11, f.y);
                return lerp(nxy0, nxy1, f.z);
            }

            float StarLayer(float3 direction, float density, float sharpness)
            {
                float3 cell = floor(direction * density);
                float star = Hash13(cell);
                return pow(saturate((star - 0.985) * 66.0), sharpness);
            }

            Varyings Vert(Attributes input)
            {
                Varyings output;
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(positionWS);
                output.directionWS = normalize(positionWS - _WorldSpaceCameraPos);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float3 direction = normalize(input.directionWS);

                float stars = StarLayer(direction, 130.0, 6.0);
                stars += StarLayer(direction + 17.13, 210.0, 9.0) * 0.65;
                stars += StarLayer(direction - 31.71, 360.0, 12.0) * 0.35;
                stars *= saturate(direction.y * 2.8 + 0.25);

                float3 galaxyDirection = normalize(float3(
                    direction.x * 0.52 + direction.y * 0.22 - direction.z * 0.82,
                    direction.x * -0.35 + direction.y * 0.93 + direction.z * 0.08,
                    direction.x * 0.78 + direction.y * 0.28 + direction.z * 0.56
                ));

                float band = exp(-pow(abs(galaxyDirection.y) / max(_MilkyWayWidth, 0.001), 2.0));
                float cloud = ValueNoise(galaxyDirection * 7.0) * 0.6 + ValueNoise(galaxyDirection * 18.0) * 0.4;
                float milkyWay = band * smoothstep(0.25, 0.9, cloud) * saturate(direction.y * 2.2 + 0.25);

                float starGlow = stars * _StarIntensity;
                float galaxyGlow = milkyWay * _MilkyWayIntensity;
                float alpha = saturate((starGlow + galaxyGlow * 0.55) * _Fade);
                float3 color = _Tint.rgb * starGlow + _MilkyWayTint.rgb * galaxyGlow;

                return half4(color, alpha);
            }
            ENDHLSL
        }
    }
}
