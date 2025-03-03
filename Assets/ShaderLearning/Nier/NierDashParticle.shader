Shader "Custom/NierGhostParticleDisperse" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _NoiseTex ("Noise Texture", 2D) = "gray" {} // Defines particle pattern
        _Fade ("Fade", Range(0,1)) = 1 // 1 = solid, 0 = fully dispersed
        _GhostColor ("Ghost Color", Color) = (0.2, 0.5, 1.0, 1) // Default NieR blue
        _DisperseSpeed ("Disperse Speed", Range(0, 2)) = 0.5 // How fast particles move
    }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha One // Additive for glowing particles
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
            float4 _GhostColor;
            float _DisperseSpeed;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.noiseUV = o.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                // Sample base texture
                fixed4 col = tex2D(_MainTex, i.uv);
                if (col.a < 0.1) discard; // Skip fully transparent areas

                // Noise for particle pattern
                fixed noise = tex2D(_NoiseTex, i.noiseUV).r;

                // Disperse effect: offset UVs based on noise and fade
                float2 disperseDir = normalize(i.uv - 0.5); // Radiate from center
                float disperseAmount = (1.0 - _Fade) * _DisperseSpeed * noise;
                float2 dispersedUV = i.uv + disperseDir * disperseAmount;

                // Sample texture at dispersed position
                col = tex2D(_MainTex, dispersedUV);
                col.rgb = _GhostColor.rgb * col.a; // Apply custom color

                // Fade alpha based on _Fade and distance
                col.a *= _Fade * (1.0 - disperseAmount);

                return col;
            }
            ENDHLSL
        }
    }
}