Shader "Custom/RainShaderURP"
{
    Properties
    {
        _MainTex ("Color (RGB) Alpha (A)", 2D) = "gray" {}
        _TintColor ("Tint Color (RGB)", Color) = (1, 1, 1, 1)
        _InvFade ("Soft Particles Factor", Range(0.01, 100.0)) = 1.0
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderPipeline"="UniversalRenderPipeline" }
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Back
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _TintColor;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                o.uv = TransformUV(v.texcoord, _MainTex_ST);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                return col * _TintColor;
            }
            ENDHLSL
        }
    }

    Fallback "Unlit/Transparent"
}
