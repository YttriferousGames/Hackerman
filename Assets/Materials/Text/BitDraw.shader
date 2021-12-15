// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/BitDraw" {
Properties {
   _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
   [MaterialToggle] _smoothPixels("Smooth Pixels", Float) = 0
   [MaterialToggle] _garbleNull("Garble NUL characters", Float) = 0
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Cull Off
    Blend SrcAlpha OneMinusSrcAlpha 

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                // Fixes artifacts in nearest sampling
                /*centroid*/ half2 texcoord : TEXCOORD0;
                fixed4 color : COLOR;
                UNITY_FOG_COORDS(1)
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            float _smoothPixels;
            float _garbleNull;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            // TODO fix edge artifacts
            // Probably by disabling smooth on garbled characters
            float2 garble(float2 uv) {
                const float sixteenth = 1. / 16.;
                if (_garbleNull > 0.5 && uv.x < sixteenth && uv.y > (1. - sixteenth)) {
                    float n = floor((60. * _Time.y) % 256.);
                    uv.x += sixteenth * (n % 16.);
                    uv.y += sixteenth * floor(n / 16.);
                    return uv;
                } else {
                    return uv;
                }
            }

            float4 texturePointSmooth(float2 uv) {
                if (_smoothPixels > 0.5f) {
                    // https://github.com/CptPotato/GodotThings/tree/master/SmoothPixelFiltering#the-algorithm
                    float2 ddX = ddx(uv);
                    float2 ddY = ddy(uv);
                    float2 lxy = sqrt(ddX * ddX + ddY * ddY);

                    float2 uv_pixels = uv * _MainTex_TexelSize.zw;

                    float2 uv_pixels_floor = round(uv_pixels) - float2(0.5, 0.5);
                    float2 uv_dxy_pixels = uv_pixels - uv_pixels_floor;

                    uv_dxy_pixels = clamp((uv_dxy_pixels - float2(0.5, 0.5)) * _MainTex_TexelSize.xy / lxy + float2(0.5, 0.5), 0.0, 1.0);

                    uv = uv_pixels_floor * _MainTex_TexelSize.xy;

                    return tex2Dgrad(_MainTex, uv + uv_dxy_pixels * _MainTex_TexelSize.xy, ddX, ddY);
                } else {
                    return tex2D(_MainTex, uv);
                }
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col;
                col = fixed4(i.color.rgb, i.color.a * texturePointSmooth(garble(i.texcoord)).a);
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG
    }
}

}
