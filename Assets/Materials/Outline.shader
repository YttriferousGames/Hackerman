Shader "Custom/Outline"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineWidth ("Outline Width", Integer) = 1

        _Jitter ("Jitter", Range(1, 64)) = 1
        [MaterialToggle] _UseVertexColors ("Use vertex colors", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        float _Jitter;

        // https://forum.unity.com/threads/proper-vertex-shaders-in-surface-shaders.1167938/
        void vert (inout appdata_full v) {
            if (_Jitter > 1.0) {
                float4 worldPos = mul(unity_ObjectToWorld, float4(v.vertex.xyz, 1.0));
                float4 viewPos = mul(UNITY_MATRIX_V, worldPos);
                float4 clipPos = mul(unity_CameraProjection, viewPos);
                    float2 j = (_ScreenParams.xy / _Jitter) / clipPos.z;
                    clipPos.xy = round(clipPos.xy * j) / j;
                v.vertex = mul(unity_WorldToObject, mul(UNITY_MATRIX_I_V, mul(unity_CameraInvProjection, clipPos)));
                v.vertex /= v.vertex.w;
            }
        }



        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG

        Pass {

            Cull Front

            CGPROGRAM

            #pragma vertex VertexProgram
            #pragma fragment FragmentProgram

            half _OutlineWidth;
            float _Jitter;
            float _UseVertexColors;

            float4 VertexProgram(float4 position : POSITION, float3 normal : NORMAL, float4 col : COLOR) : SV_POSITION {
                float4 clipPosition = UnityObjectToClipPos(position);
                float3 clipNormal = mul((float3x3) UNITY_MATRIX_VP, mul((float3x3) UNITY_MATRIX_M, _UseVertexColors > 0.5 ? col.rgb : normal));

                float2 offset = normalize(clipNormal.xy) / _ScreenParams.xy * _OutlineWidth * clipPosition.w * 2;
                if (_Jitter >= 1.0) {
                    float2 j = (_ScreenParams.xy / _Jitter) / clipPosition.z;
                    clipPosition.xy = round(clipPosition.xy * j) / j;
                }

                clipPosition.xy += offset;

                return clipPosition;
            }

            half4 _OutlineColor;

            half4 FragmentProgram() : SV_TARGET {
                // Disable if width less than 1
                clip(_OutlineWidth - 1);
                return half4(_OutlineColor.rgb, 1.0);
            }

            ENDCG

        }
    }
    FallBack "Diffuse"
}
