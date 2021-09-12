// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/SilhouetteDissolve"
{
    Properties
    {
        _DissolveMap ("Dissolve Map", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (1,1,1,1)
        _EdgeThreshold ("Edge Threshold", float) = 0.1
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0

        [PerRendererData] _Color ("Color", Color) = (1,1,1,1)
        [PerRendererData] _DissolveAmount ("Dissolve Amount", Float) = 0.5
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ PIXELSNAP_ON
            #pragma shader_feature ETC1_EXTERNAL_ALPHA
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                float2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _EdgeColor;
            float _DissolveAmount;
            sampler2D _DissolveMap;
            float _EdgeThreshold;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;
                #ifdef PIXELSNAP_ON
                OUT.vertex = UnityPixelSnap (OUT.vertex);
                #endif

                return OUT;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float value = tex2D(_DissolveMap, IN.texcoord).r;

                float4 output = _Color;
                if (value < _DissolveAmount) {
                    output.a = 0;
                }
                else if (value < _DissolveAmount + _EdgeThreshold) {
                    output.a = (_DissolveAmount - (value - _EdgeThreshold))
                        / _EdgeThreshold;
                    output.rgb = lerp(_Color.rgb, _EdgeColor.rgb, output.a);
                }

                // premultiply alpha
                output.rgb *= output.a;

                return output;
            }
            ENDCG
        }
    }
}