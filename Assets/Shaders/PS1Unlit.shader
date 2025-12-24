Shader "PS1/Unlit"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _AffineMapping ("Affine Texture Mapping", Range(0, 1)) = 1
        _VertexJitter ("Vertex Jitter", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float fog : TEXCOORD1;
                float depth : TEXCOORD2;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _AffineMapping;
                float _VertexJitter;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                
                // Симуляция jitter вершин как в PS1 (из-за отсутствия субпиксельной точности)
                float4 clipPos = TransformObjectToHClip(IN.positionOS.xyz);
                
                // Snap to pixel grid
                float4 snappedPos = clipPos;
                float gridSize = 320.0 * _VertexJitter; // Разрешение сетки
                snappedPos.xy = floor(snappedPos.xy * gridSize) / gridSize;
                
                OUT.positionHCS = lerp(clipPos, snappedPos, _VertexJitter);
                OUT.depth = clipPos.w;
                
                // Affine texture mapping (как в PS1 - без перспективной коррекции)
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.uv *= lerp(OUT.depth, 1.0, _AffineMapping); // Affine = умножаем на глубину
                
                OUT.fog = ComputeFogFactor(clipPos.z);
                
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                // Восстанавливаем UV для affine mapping
                float2 uv = IN.uv / lerp(IN.depth, 1.0, _AffineMapping);
                
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv) * _Color;
                
                // Применяем туман
                col.rgb = MixFog(col.rgb, IN.fog);
                
                return col;
            }
            ENDHLSL
        }
    }
}

