// VFX1 - Vjezbe 4: prikaz jednog kanala channel-packed maske (T_Maske_RGB)
// Kanal: 0 = R (tocka), 1 = G (prsten), 2 = B (sum), 3 = A
Shader "VFX1/MaskaKanal"
{
    Properties
    {
        _BaseMap ("Maska", 2D) = "white" {}
        _Kanal ("Kanal (0 R, 1 G, 2 B, 3 A)", Range(0, 3)) = 0
        _Boja ("Boja", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" "RenderType" = "Opaque" "Queue" = "Geometry" }
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float _Kanal;
                float4 _Boja;
            CBUFFER_END

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; };

            Varyings vert (Attributes i)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(i.positionOS.xyz);
                o.uv = TRANSFORM_TEX(i.uv, _BaseMap);
                return o;
            }

            half4 frag (Varyings i) : SV_Target
            {
                half4 m = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv);
                int k = (int)round(_Kanal);
                half v = k == 0 ? m.r : (k == 1 ? m.g : (k == 2 ? m.b : m.a));
                return half4(v * _Boja.rgb, 1);
            }
            ENDHLSL
        }
    }
}
