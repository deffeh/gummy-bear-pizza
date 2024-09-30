Shader "Particles/BulletEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Col ("Color", Color) = (1.,1.,1.,1.)
        _ColInner ("Inner Color", Color) = (1.,1.,1.,1.)
        _Resolution ("Resolution", int) = 10
        _ColorResolution ("ColorResolution", int) = 10
        _Color1 ("Color1", Color) = (1,1,1,1)
        _Color2 ("Color2", Color) = (1,1,1,1)
        _Color1Cutoff("Cutoff1", float) = .5
        _Color1Range("Range1", float) = .2
        _Color2Cutoff("Cutoff2", float) = .8
        _Color2Range("Range2", float) = .2
    }
    SubShader
    {
        Tags
        {
            "Queue" = "Transparent" "RenderType"="Transparent"
        }
        LOD 100

        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        BlendOp Add
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Libraries/NoiseLibrary.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 scrPos : TEXCOORD1;
                float4 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColInner;
            v2f vert (appdata v)
            {
                 v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = (i.uv - .5) * 2;
                fixed4 col = i.color;

                float a = 1 - i.color.a *  (1+ (sin(_Time[1] * 50) * .1));
                col.a = (smoothstep(a, a + 3, pow(clamp(1 - dot(uv,uv),0,1), 2.5)) * 1.1);
                col.a *= 1.1;
                col.rgb = float3(1,.6,.2);;

                // apply fog
                return clamp(col,0,.99);
            }
            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "Libraries/NoiseLibrary.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
                float4 scrPos : TEXCOORD1;
                float4 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _ColInner;

            int _Resolution;
            int _ColorResolution;
            float4 _Color1;
            float4 _Color2;

            float _Color1Cutoff;
            float _Color1Range;
            float _Color2Cutoff;
            float _Color2Range;

            
            v2f vert (appdata v)
            {
                 v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                o.scrPos = ComputeScreenPos(o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                i.uv -= .5f;
                i.uv *= 20;
                i.uv *= _Resolution;
                i.uv = floor(i.uv);
                i.uv/= _Resolution;
                float f = smoothstep(0.5,1., dot(i.uv, i.uv));
                f = 1 - f;

                fixed4 col = i.color;
                
                // sample the texture
                col.a *= f;
                return clamp(col, 0., 1.);
            }
            ENDCG
        }
    }
}
