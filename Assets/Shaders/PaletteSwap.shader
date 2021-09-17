// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Thing/PaletteSwap"
{
    Properties{
        _MainTex("", 2D) = "" {}
    }
        SubShader{

            Pass{
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                struct v2f {
                    float4 pos : POSITION;
                    half2 uv : TEXCOORD0;
                };

                v2f vert(appdata_img v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = MultiplyUV(UNITY_MATRIX_TEXTURE0, v.texcoord.xy);
                    return o;
                }

                sampler2D _MainTex;

                uniform float4 _InColorA;
                uniform float4 _InColorB;
                uniform float4 _InColorC;
                uniform float4 _InColorD;

                uniform float4 _OutColorA;
                uniform float4 _OutColorB;
                uniform float4 _OutColorC;
                uniform float4 _OutColorD;

                fixed4 frag(v2f i) : COLOR
                {
                    /*float4 start = tex2D(_MainTex, i.uv);

                    float4 final = start;

                    float testA = saturate(abs(start.x - _InColorA.x) + abs(start.y - _InColorA.y) + abs(start.z - _InColorA.z));
                    final = lerp(final, _OutColorA, 1 - testA);

                    float testB = saturate(abs(start.x - _InColorB.x) + abs(start.y - _InColorB.y) + abs(start.z - _InColorB.z));
                    final = lerp(final, _OutColorB, 1 - testB);

                    float testC = saturate(abs(start.x - _InColorC.x) + abs(start.y - _InColorC.y) + abs(start.z - _InColorC.z));
                    final = lerp(final, _OutColorC, 1 - testC);

                    float testD = saturate(abs(start.x - _InColorD.x) + abs(start.y - _InColorD.y) + abs(start.z - _InColorD.z));
                    final = lerp(final, _OutColorD, 1 - testD);

                    return final;*/

                    //Forgive me lord for I'm about to sin

                    float4 start = tex2D(_MainTex, i.uv);
                    float _offset = 0.004;

                    if ((start.r >= _InColorA.r - _offset && start.r <= _InColorA.r + _offset) &&
                        (start.g >= _InColorA.g - _offset && start.g <= _InColorA.g + _offset) &&
                        (start.b >= _InColorA.b - _offset && start.b <= _InColorA.b + _offset))
                    {
                        start.rgb = _OutColorA.rgb;
                    }

                    if ((start.r >= _InColorB.r - _offset && start.r <= _InColorB.r + _offset) &&
                        (start.g >= _InColorB.g - _offset && start.g <= _InColorB.g + _offset) &&
                        (start.b >= _InColorB.b - _offset && start.b <= _InColorB.b + _offset))
                    {
                        start.rgb = _OutColorB.rgb;
                    }

                    if ((start.r >= _InColorC.r - _offset && start.r <= _InColorC.r + _offset) &&
                        (start.g >= _InColorC.g - _offset && start.g <= _InColorC.g + _offset) &&
                        (start.b >= _InColorC.b - _offset && start.b <= _InColorC.b + _offset))
                    {
                        start.rgb = _OutColorC.rgb;
                    }

                    if ((start.r >= _InColorD.r - _offset && start.r <= _InColorD.r + _offset) &&
                        (start.g >= _InColorD.g - _offset && start.g <= _InColorD.g + _offset) &&
                        (start.b >= _InColorD.b - _offset && start.b <= _InColorD.b + _offset))
                    {
                        start.rgb = _OutColorD.rgb;
                    }

                    return start;
                }
                ENDCG
            }
    }
}