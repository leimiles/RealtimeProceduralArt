Shader "Sofunny/shd_Preview_V1"
{
    Properties
    {
        [HideInInspector]
        _Color ("Color", Color) = (1.0, 1.0, 1.0, 1.0)
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag
            // make fog work

            #include "UnityCG.cginc"
            half4 _Color;
            

            struct appdata
            {
                float4 vertex : POSITION;
                half3 normal_Object : NORMAL;
            };

            struct v2f
            {

                half3 normal_World : NORMAL;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal_World = UnityObjectToWorldNormal(v.normal_Object);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return half4(i.normal_World * 0.5 + 0.5, 0.2) * _Color;
            }
            ENDCG

        }
    }
}
