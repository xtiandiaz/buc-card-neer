// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "GUI/Text Shader OneSided" 
{
    Properties 
    {
        _MainTex ("Font Texture", 2D) = "white" {}
        _Color ("Text Color", Color) = (1,1,1,1)
        [PerRendererData] _FogColor("Fog Color", Color) = (1,1,1,1)
        [PerRendererData] _FogIntensity("Fog Intensity", Range(0, 1.0)) = 0
    }

    SubShader 
    {
        Tags 
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }
        Lighting Off Cull Back ZTest Always ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass 
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON
            #include "UnityCG.cginc"
            
            UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _FogColor)
                UNITY_DEFINE_INSTANCED_PROP(fixed, _FogIntensity)
            UNITY_INSTANCING_BUFFER_END(PerDrawSprite)
            
            #define _FogColorPDS        UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, _FogColor)
            #define _FogIntensityPDS    UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, _FogIntensity)

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            uniform float4 _MainTex_ST;
            uniform fixed4 _Color;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color * _Color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseColor = fixed4(i.color.r, i.color.g, i.color.b, tex2D(_MainTex, i.texcoord).a * i.color.a);                
                fixed4 c = lerp(baseColor, _FogColorPDS, _FogIntensityPDS) * baseColor.a;
                return c;
            }
            ENDCG
        }
    }
}
