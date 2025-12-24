Shader "Hidden/PS1PostProcess"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DitherStrength ("Dither Strength", Range(0, 1)) = 0.05
        _ColorSteps ("Color Steps", Float) = 32
        _EnableColorBanding ("Enable Color Banding", Int) = 1
    }
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _DitherStrength;
            float _ColorSteps;
            int _EnableColorBanding;

            // Dither matrix 4x4 (Bayer)
            float dither4x4(float2 position, float brightness)
            {
                int x = int(fmod(position.x, 4.0));
                int y = int(fmod(position.y, 4.0));
                
                int index = x + y * 4;
                float limit = 0.0;
                
                if (index == 0) limit = 0.0625;
                if (index == 1) limit = 0.5625;
                if (index == 2) limit = 0.1875;
                if (index == 3) limit = 0.6875;
                if (index == 4) limit = 0.8125;
                if (index == 5) limit = 0.3125;
                if (index == 6) limit = 0.9375;
                if (index == 7) limit = 0.4375;
                if (index == 8) limit = 0.25;
                if (index == 9) limit = 0.75;
                if (index == 10) limit = 0.125;
                if (index == 11) limit = 0.625;
                if (index == 12) limit = 1.0;
                if (index == 13) limit = 0.5;
                if (index == 14) limit = 0.875;
                if (index == 15) limit = 0.375;
                
                return brightness < limit ? 0.0 : 1.0;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                // Color banding (ограничение цветов как в PS1)
                if (_EnableColorBanding == 1)
                {
                    col.rgb = floor(col.rgb * _ColorSteps) / _ColorSteps;
                }
                
                // Dithering
                float2 pixelPos = i.uv * _ScreenParams.xy;
                float brightness = (col.r + col.g + col.b) / 3.0;
                float ditherValue = dither4x4(pixelPos, brightness);
                col.rgb += (ditherValue - 0.5) * _DitherStrength;
                
                return col;
            }
            ENDCG
        }
    }
}

