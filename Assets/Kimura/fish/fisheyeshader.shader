Shader "Hidden/Custom/FisheyeEffectStrong"
{
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Distortion ("Distortion", Range(0, 1)) = 0.6
        _Zoom ("Zoom", Range(0.5, 2.0)) = 1.0
    }

    SubShader
    {
        Cull Off ZWrite Off ZTest Always
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Distortion;
            float _Zoom;

            struct appdata_t { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float4 vertex : SV_POSITION; float2 uv : TEXCOORD0; };

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = (i.uv - 0.5) * _Zoom + 0.5; // ÉYÅ[ÉÄëÄçÏÇí«â¡
                float2 centered = uv * 2.0 - 1.0;
                float r = length(centered);
                float theta = atan2(centered.y, centered.x);
                float rd = r + _Distortion * pow(r, 2);
                float2 distortedUV = rd * float2(cos(theta), sin(theta));
                distortedUV = (distortedUV + 1.0) * 0.5;

                if (any(distortedUV < 0) || any(distortedUV > 1)) discard;
                return tex2D(_MainTex, distortedUV);
            }
            ENDCG
        }
    }
}
