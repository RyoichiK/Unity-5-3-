Shader "Custom/MaskCameraDepthCompare"
{
    Properties
    {
        _MaskCameraDepthTexture ("Mask Camera Depth", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MaskCameraDepthTexture;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD1;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = o.pos.xy * 0.5 + 0.5;
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float maskDepth = tex2D(_MaskCameraDepthTexture, i.uv).r;
                float myDepth = i.screenPos.z / i.screenPos.w;
                float linearMyDepth = Linear01Depth(myDepth);

                if (maskDepth < linearMyDepth - 0.01)
                    return fixed4(1, 0, 0, 1);  // Ô
                else
                    return fixed4(0, 0, 1, 1);  // Â
            }
            ENDCG
        }
    }
}
