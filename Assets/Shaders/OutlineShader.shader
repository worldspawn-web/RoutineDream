Shader "Custom/Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1, 1, 0, 1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.1)) = 0.03
    }
    
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent+1" }
        
        Pass
        {
            Name "OUTLINE"
            Cull Front
            ZWrite On
            ZTest LEqual
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            
            struct v2f
            {
                float4 pos : SV_POSITION;
            };
            
            float _OutlineWidth;
            float4 _OutlineColor;
            
            v2f vert(appdata v)
            {
                v2f o;
                float3 norm = normalize(v.normal);
                float3 outlinePos = v.vertex.xyz + norm * _OutlineWidth;
                o.pos = UnityObjectToClipPos(float4(outlinePos, 1.0));
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                return _OutlineColor;
            }
            ENDCG
        }
    }
}

