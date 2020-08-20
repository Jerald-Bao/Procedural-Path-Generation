Shader "Custom/blueprint"
{
    Properties
    {
        _Color0 ("Color0", Color) = (1,1,1,1)
        _Emission ("Emission", Color) = (1,1,1,1)
        _EmissionIntensity ("EmissionIntensity", Float) = 0
        _Intensity ("Intensity", Float )=0
        _Smoothness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Lighting Off
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        // We want it to be unlit so it would be shaded conspicuously in lighting from every angle and with any intensities etc. 
        #pragma surface surf Standard  vertex:vert alpha

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0


        struct Input
        {
           float2 uv_MainTex;
            float3 vertexNormal;
        };
        
        half _Smoothness;
        float _Intensity;
        float _EmissionIntensity;
        half _Metallic;
        fixed4 _Color0;
        fixed4 _Emission;

        void vert (inout appdata_full v, out Input o) {
           UNITY_INITIALIZE_OUTPUT(Input,o);
           o.vertexNormal = v.normal;
        }
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float3 up=float3(0,1,0);
            o.Albedo = _Color0.rgb *abs(dot(IN.vertexNormal,up)) *  _Intensity;
            o.Smoothness=_Smoothness;
            o.Metallic=_Metallic;
            //o.Gloss=lerp(0,1,abs(dot(IN.vertexNormal,up)));
            o.Emission = _Emission * _EmissionIntensity;
            // Metallic and smoothness come from slider variables
            o.Alpha = 0.5;
        }
        
        ENDCG
        
    }
}
