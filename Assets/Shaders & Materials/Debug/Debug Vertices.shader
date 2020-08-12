// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Debug Vertices"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass{
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
    
            struct AppData
            {
                float4 vertex : POSITION; 
            };
            
            struct v2f
            {
                float4 vertex:SV_POSITION;
            };
            
            v2f vert(AppData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
    
            fixed4 frag (v2f v) :SV_Target
            {
                fixed4 res = fixed4(v.vertex.xyz,1);
                return res;
            }
            ENDCG
        }
    }   
}
