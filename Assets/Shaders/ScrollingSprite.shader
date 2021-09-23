// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Thing/Sprite Scrolling" 
{
    Properties
    {
        _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)
        _ScrollSpeedX("Scroll Speed X", float) = 1
        _ScrollSpeedY("Scroll Speed Y", float) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnitySprites.cginc"

            // sampler2D _MainTex;
            float4 _MainTex_ST;
            // float4 _Color;
            float _ScrollSpeedX;
            float _ScrollSpeedY;

            struct vinput
            {
                float4 vertex : POSITION;
                float4 texcoord : TEXCOORD0;
            };

            struct voutput
            {
                float4 vertex : SV_POSITION;
                float4 texcoord : TEXCOORD0;
            };

            voutput vert(vinput i)
            {
                voutput result;
                result.vertex = UnityObjectToClipPos(i.vertex);
                result.texcoord.xy = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
                result.texcoord.zw = _MainTex_ST.zw;
                return result;
            }

            float4 frag(voutput o) : COLOR
            {
                float2 newUVs = float2(
                    fmod(o.texcoord.x + _Time.x * _ScrollSpeedX, 1),
                    fmod(o.texcoord.y + _SinTime.x * _ScrollSpeedY, 1)
                );

                return tex2D(_MainTex, newUVs) * _Color;
            }

            ENDCG
        }
    }
}