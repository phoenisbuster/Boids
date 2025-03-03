Shader "Custom/NierGhostDissolve" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "gray" {} // For dissolve pattern
        _Fade ("Fade", Range(0,1)) = 1 // 1 = solid, 0 = fully dissolved
        _BloomIntensity ("Bloom Intensity", Range(0,2)) = 1 // Glow peak
        _GhostColor ("Ghost Color", Color) = (0.2, 0.5, 1.0, 1) // Default NieR blue
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha One // Additive blending for glow
        ZWrite Off

        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float2 noiseUV : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            float4 _MainTex_ST;
            float _Fade;
            float _BloomIntensity;
            float4 _GhostColor;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noiseUV = o.uv; // Reuse UVs for noise
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Base texture and blue tint
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = _GhostColor.rgb * col.a; // Blue Energy Blade tint

                // Bloom effect: amplify brightness at start
                float bloom = _BloomIntensity * smoothstep(1.0, 0.7, _Fade); // Peaks near Fade = 1
                col.rgb *= (1.0 + bloom);

                // Dissolve effect: use noise to erode
                fixed noise = tex2D(_NoiseTex, i.noiseUV).r;
                float dissolve = step(noise, _Fade); // Fade acts as threshold
                col.a *= dissolve;

                return col;
            }
            ENDHLSL
        }
    }
}