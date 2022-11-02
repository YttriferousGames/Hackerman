Shader "Hidden/GBShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
        _Variants ("Color variants", Integer) = 8
    }
    SubShader
    {
        // No culling or depth
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
            float _Variants;

            fixed4 frag (v2f i) : SV_Target
            {
                const int palette_num = 4;
                const fixed3 palette[palette_num] = {
                    fixed3(0.886,0.953,0.894),
                    fixed3(0.58,0.89,0.267),
                    fixed3(0.275,0.529,0.561),
                    fixed3(0.2,0.173,0.314),
                };

                int closest = -1;
                fixed closeness = -1.0;
                fixed offset = 0.0;
                fixed4 col = tex2D(_MainTex, i.uv);
                for (int x = 0; x < palette_num; x++) {
                    fixed3 diff = col.rgb - palette[x];
                    fixed c = dot(diff, diff);
                    if (closeness < 0 || c < closeness) {
                        closest = x;
                        closeness = c;
                        offset = (diff.r + diff.g + diff.b) / 3.0;
                    }
                }
                int num_variants = max(1, _Variants);
                col.rgb = palette[closest] + fixed(int(num_variants * offset)) / num_variants;

                return col;
            }
            ENDCG
        }
    }
}
