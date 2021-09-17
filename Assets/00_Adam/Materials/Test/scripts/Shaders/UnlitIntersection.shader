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

            //ideal shader
            //if in the volumn of the unity object (a cube maybe)
            //then shade it tint it with the color of the highlight (Example blue color)
            //else if infront of the shader color it normally
            //else if behind the shader tint it the color of shader (Example Yellow tint)

            //end effect is an object that can be placed over other objects to change the color of the areas inside of the 
            //volumn a tint of highlighted color
            //while indicating that the objects behind it are behind it but not inside of it by showing the normal tint addition
            //while not affecting anything infront of it
            //The obejcts might be non-uniformly scaled

            fixed4 frag(v2f i, UNITY_VPOS_TYPE vpos : VPOS) : SV_Target
            {
                // height,width position of the rendered pixel / hight,width of cammera target texture in pixels
                float2 screenuv = vpos.xy / _ScreenParams.xy;

                //ret tex2D(s, t) s [in] The sampler state, t [in] The texture coordinate
                //Main camera depth texture, screen position of pixel
                float screenDepth = tex2D(_CameraDepthTexture, screenuv);

                //Linear01Depth(i): given high precision value from depth texture i, returns corresponding linear depth in range between 0 and 1.
                screenDepth = Linear01Depth(screenDepth);

                //Depth of rendered target - depth of rendered pixel
                float diff = screenDepth - Linear01Depth(vpos.z);

                float intersect = 0;

                // if there is a pixel 
                if (diff > 0)
                    intersect = 1 - smoothstep(0, _ProjectionParams.w, diff);
                //intersect = 1 - smoothstep(0, _ProjectionParams.w * _FadeLength, diff);

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