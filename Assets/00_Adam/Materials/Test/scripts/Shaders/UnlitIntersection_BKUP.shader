Shader "Intersection/Unlit"
{
    Properties
    {
        _Color("Color", Color) = (0,0,0,0)
        _GlowColor("Glow Color", Color) = (1, 1, 1, 1)
        _FadeLength("Fade Length", Range(0, 5)) = 0.15
        _Threshold("Threshold", Range(0, 5)) = 2
    }
        SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite On

        Tags
        {
            "RenderType" = "Transparent"
            "Queue" = "Transparent"
        }

        Pass
        {
            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v, out float4 vertex : SV_POSITION)
            {
                v2f o;
                vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }

            sampler2D _CameraDepthTexture;
            fixed4 _Color;
            fixed4 _GlowColor;
            float _FadeLength;
            float _Threshold;

            fixed4 frag(v2f i, UNITY_VPOS_TYPE vpos : VPOS) : SV_Target
            {
                float2 screenuv = vpos.xy / _ScreenParams.xy;
                float screenDepth = Linear01Depth(tex2D(_CameraDepthTexture, screenuv));
                float diff = screenDepth - Linear01Depth(vpos.z);
                float intersect = 0;

                if (diff > 0)
                    intersect = 1 - smoothstep(0, _ProjectionParams.w * _FadeLength, diff);

                //This section sets the highlights
                fixed4 glowColor = fixed4(lerp(intersect, _GlowColor.rgb, pow(intersect, 0)), 1); //1
                glowColor = glowColor - (1 - intersect);
                glowColor = glowColor * _GlowColor.a;

                //works                 fixed4 background = fixed4(lerp(intersect, _Color.rgb, pow((1 - intersect), 4)), 1); //1
                fixed4 background = fixed4(lerp(intersect, _Color.rgb, pow((1 - intersect), 100)), 1); //1
                background = background - (intersect);
                background = background * _Color.a;

                fixed4 output;
                output = glowColor;

                half3 delta = abs(glowColor.rgb - background.rgb);
                if ((delta.r + delta.g + delta.b) > _Threshold)
                {
                    output = background;
                }

                return output;

                //return intersect;
            }
            ENDCG
        }
    }
}


//
//Shader "Intersection/Unlit"
//{
//    Properties
//    {
//        _Color("Color", Color) = (0,0,0,0)
//        _GlowColor("Glow Color", Color) = (1, 1, 1, 1)
//        _FadeLength("Fade Length", Range(0, 5)) = 0.15
//    }
//        SubShader
//    {
//        Blend SrcAlpha OneMinusSrcAlpha
//        ZWrite On
//
//        Tags
//        {
//            "RenderType" = "Transparent"
//            "Queue" = "Transparent"
//        }
//
//        Pass
//        {
//            CGPROGRAM
//            #pragma target 3.0
//            #pragma vertex vert
//            #pragma fragment frag
//
//            #include "UnityCG.cginc"
//
//            struct appdata
//            {
//                float4 vertex : POSITION;
//                float2 uv : TEXCOORD0;
//                float3 normal : NORMAL;
//            };
//
//            struct v2f
//            {
//                float2 uv : TEXCOORD0;
//            };
//
//            sampler2D _MainTex;
//            float4 _MainTex_ST;
//
//            v2f vert(appdata v, out float4 vertex : SV_POSITION)
//            {
//                v2f o;
//                vertex = UnityObjectToClipPos(v.vertex);
//                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
//
//                return o;
//            }
//
//            sampler2D _CameraDepthTexture;
//            fixed4 _Color;
//            fixed3 _GlowColor;
//            float _FadeLength;
//
//            fixed4 frag(v2f i, UNITY_VPOS_TYPE vpos : VPOS) : SV_Target
//            {
//                float2 screenuv = vpos.xy / _ScreenParams.xy;
//                float screenDepth = Linear01Depth(tex2D(_CameraDepthTexture, screenuv));
//                float diff = screenDepth - Linear01Depth(vpos.z);
//                float intersect = 0;
//
//                if (diff > 0)
//                    intersect = 1 - smoothstep(0, _ProjectionParams.w * _FadeLength, diff);
//
//                fixed4 glowColor = fixed4(lerp(_Color.rgb, _GlowColor, pow(intersect, 4)), 1);
//
//                fixed4 col = _Color * _Color.a + glowColor;
//                col.a = _Color.a;
//                //return col;
//                return col;
//            }
//            ENDCG
//        }
//    }
//}