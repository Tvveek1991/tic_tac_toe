Shader "Custom/StandardFadeWithShadowsAndEmission"
{
    Properties
    {
        _Color ("Main Color", Color) = (1,1,1,0.5)
        _MainTex ("Base (RGB)", 2D) = "white" { }
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        _Emission ("Emission", Color) = (0,0,0,0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows alpha:fade

        struct Input
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        fixed4 _Color;
        float _Glossiness;
        float _Metallic;
        fixed4 _Emission;

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            o.Albedo = c.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;

            // Add emission
            o.Emission = _Emission.rgb;
        }
        ENDCG
    }
    FallBack "Diffuse"
}