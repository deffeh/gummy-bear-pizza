Shader "Unlit/PixelParticle"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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

            float _Color1Cutoff;
            float _Color1Range;
            float _Color2Cutoff;
            float _Color2Range;
            
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
                i.uv -= .5f;
                i.uv *= 2;
                i.uv *= _Resolution;
                i.uv = floor(i.uv);
                i.uv/= _Resolution;
                float f = smoothstep(0.5,1., dot(i.uv, i.uv));
                i.uv += .2;
                float f2 = 1 - dot(i.uv, i.uv);

                fixed4 col = 1.;
                f2 = floor(f2 * _ColorResolution)/_ColorResolution;
                col = lerp(col, _Color1, smoothstep(_Color1Cutoff, _Color1Cutoff + _Color1Range, f2));
                col = lerp(col, _Color2, smoothstep(_Color2Cutoff, _Color2Cutoff + _Color2Range, f2));

                f = 1 - f;
                // sample the texture
                col *= i.color;
                col.a *= f;
                return clamp(col, 0., 1.);
            }
            ENDCG
        }
    }
}
