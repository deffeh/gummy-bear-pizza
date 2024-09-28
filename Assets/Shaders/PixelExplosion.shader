Shader "Unlit/PixelExplosion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Resolution ("Resolution", int) = 10
        _ColorResolution ("ColorResolution", int) = 10
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent" }
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
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _Resolution;
            int _ColorResolution;
            float4 _Color1;
            float4 _Color2;

            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 baseUV =( i.uv - .5) * .5;
                baseUV += floor((float2(cos(i.color.a * 5), sin(i.color.a * 5)) * (1 - i.color.a)) * _Resolution)/_Resolution;
                baseUV *= _Resolution * 2;
                baseUV = floor(baseUV);
                baseUV /= _Resolution * 2;
                
                i.uv -= .5;
                i.uv *= 2;
                i.uv *= _Resolution;
                i.uv = floor(i.uv);
                i.uv/= _Resolution;
                float f = smoothstep(0.5,1., dot(i.uv, i.uv));
                i.uv += .2;

                fixed4 col = 1.;

                
                f = 1 - f;
                // sample the texture
                col.rgb *= i.color.rgb;
                col.a *= f;
                float f2 = smoothstep(0,1.,dot(baseUV, baseUV));
                col.a *= f2;
                // col.a *= smoothstep(0., .1, i.color.a);
                col.a *= 10;
                col.a = floor(col.a);
                col.a /= 10;
                return clamp(col, 0., 1.);
            }
            ENDCG
        }
    }
}
