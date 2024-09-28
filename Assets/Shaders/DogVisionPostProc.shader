Shader "Hidden/DogVisionPostProc"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // just invert the colors
                float3 R = float3(0.33066007f, 0.66933993f, 0);
                float3 G = float3(0.33066007f, 0.66933993f, 0);
                float3 B = float3(-0.02785538f, 0.02785538f, 1);
                
                col = 
                float4(col.r * R.r + col.g * R.g + col.b * R.b,
                        col.r * G.r + col.g * G.g + col.b * G.b,
                        col.r * B.r + col.g * B.g + col.b * B.b,
                        1);
                return col;
            }
            ENDCG
        }
    }
}
