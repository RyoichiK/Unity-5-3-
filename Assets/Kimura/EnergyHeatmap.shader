// ========== EnergyHeatmap_MultiPeople_WithBraille (視線方向ぼかし+点字優先描画) ==========
Shader "Custom/EnergyHeatmap_MultiPeople_WithBraille"
{
    Properties
    {
        _MaskTex("Mask Texture", 2D) = "white" {}
        _BrailleRedMask("Braille Highlight Mask", 2D) = "white" {}
        _MaskWorldMinX("Mask Min X", Float) = 0
        _MaskWorldMaxX("Mask Max X", Float) = 1
        _MaskWorldMinZ("Mask Min Z", Float) = 0
        _MaskWorldMaxZ("Mask Max Z", Float) = 1

        _Gamma("Gamma", Float) = 0.4
        _Sigma("Sigma", Float) = 4.0
        _Beta("Beta", Float) = 1.0
        _WScale("W Scale", Float) = 1.0
        _AlphaMax("Alpha Max", Float) = 1.0
        _MaxDist("Max Distance", Float) = 30
        _CameraRangeXZ("Camera Range XZ", Float) = 30
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            #define MAX_PEOPLE 8

            sampler2D _MaskTex;
            sampler2D _BrailleRedMask;
            float _MaskWorldMinX, _MaskWorldMaxX;
            float _MaskWorldMinZ, _MaskWorldMaxZ;

            float4 _PeoplePos[MAX_PEOPLE];
            float4 _PeopleDir[MAX_PEOPLE];
            float _CurrentAngles[MAX_PEOPLE];
            float _PeopleCount;
            float4 _CameraPos;

            float _Gamma, _Sigma, _Beta, _WScale, _AlphaMax, _MaxDist, _CameraRangeXZ;

            struct appdata { float4 vertex : POSITION; };
            struct v2f {
                float4 pos : SV_POSITION;
                float3 world : TEXCOORD0;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 uv;
                uv.x = saturate((i.world.x - _MaskWorldMinX) / (_MaskWorldMaxX - _MaskWorldMinX));
                uv.y = saturate((i.world.z - _MaskWorldMinZ) / (_MaskWorldMaxZ - _MaskWorldMinZ));

                if (tex2D(_MaskTex, uv).r < 0.5) discard;

                // 点字ブロック赤優先描画
                if (tex2D(_BrailleRedMask, uv).r > 0.5)
                {
                    return fixed4(1, 0, 0, _AlphaMax);
                }

                fixed4 col = fixed4(0, 0, 1, 0.2);
                float maxEnergy = 0;

                for (int p = 0; p < (int)_PeopleCount && p < MAX_PEOPLE; ++p)
                {
                    float2 camDelta = _PeoplePos[p].xz - _CameraPos.xz;
                    if (length(camDelta) > _CameraRangeXZ) continue;

                    float2 delta = float2(i.world.x, i.world.z) - _PeoplePos[p].xz;
                    float dist = length(delta);
                    if (dist > _MaxDist) continue;

                    float2 dirNorm = normalize(_PeopleDir[p].xz);
                    float2 relNorm = normalize(delta);
                    float dotAngle = dot(dirNorm, relNorm);
                    float thetaDeg = degrees(acos(clamp(dotAngle, -1, 1)));

                    float angle = _CurrentAngles[p];
                    if (thetaDeg > angle / 2.0) continue;

                    float Atheta = pow(max(dotAngle, 0), _Gamma);
                    float Gdist = exp(-(dist * dist) / (2.0 * _Sigma * _Sigma));
                    float normTheta = thetaDeg / (angle / 2.0);
                    float coneMask = saturate(cos(normTheta * UNITY_PI * 0.5));

                    float E = Atheta * Gdist * coneMask;
                    float energy = pow(saturate(E * _WScale), _Beta);
                    maxEnergy = max(maxEnergy, energy);
                }

                if (maxEnergy < 0.001) return col;

                if (maxEnergy < 0.5)
                    col = lerp(fixed4(0, 0, 1, 1), fixed4(1, 0.5, 0, 1), maxEnergy * 2.0);
                else
                    col = lerp(fixed4(1, 0.5, 0, 1), fixed4(1, 0, 0, 1), (maxEnergy - 0.5) * 2.0);

                col.a = lerp(0.2, _AlphaMax, maxEnergy);
                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}
