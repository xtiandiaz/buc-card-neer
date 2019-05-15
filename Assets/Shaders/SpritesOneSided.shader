// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Sprites/OneSided"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [PerRendererData] _FogColor("Fog Color", Color) = (1,1,1,1)
        [PerRendererData] _FogIntensity("Fog Intensity", Range(0, 1.0)) = 0
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
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

        Cull Back
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            
            UNITY_INSTANCING_BUFFER_START(PerDrawSprite)
                // SpriteRenderer.Color while Non-Batched/Instanced.
                UNITY_DEFINE_INSTANCED_PROP(fixed4, unity_SpriteRendererColorArray)
                UNITY_DEFINE_INSTANCED_PROP(fixed2, unity_SpriteFlipArray)
                
                UNITY_DEFINE_INSTANCED_PROP(fixed4, _FogColor)
                UNITY_DEFINE_INSTANCED_PROP(fixed, _FogIntensity)
            UNITY_INSTANCING_BUFFER_END(PerDrawSprite)
        
            #define _RendererColor      UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteRendererColorArray)
            #define _FogColorPDS        UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, _FogColor)
            #define _FogIntensityPDS    UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, _FogIntensity)
            #define _Flip               UNITY_ACCESS_INSTANCED_PROP(PerDrawSprite, unity_SpriteFlipArray)
            // Material Color.
            fixed4 _Color;
            
            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            inline float4 UnityFlipSprite(in float3 pos, in fixed2 flip)
            {
                return float4(pos.xy * flip, pos.z, 1.0);
            }
            
            v2f SpriteVert(appdata_t IN)
            {
                v2f OUT;
            
                UNITY_SETUP_INSTANCE_ID (IN);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
            
                OUT.vertex = UnityFlipSprite(IN.vertex, _Flip);
                OUT.vertex = UnityObjectToClipPos(OUT.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color * _Color * _RendererColor;
            
                return OUT;
            }
            
            sampler2D _MainTex;
            
            fixed4 SpriteFrag(v2f IN) : SV_Target
            {
                fixed4 baseColor = tex2D(_MainTex, IN.texcoord) * IN.color;
                fixed4 c = lerp(baseColor, _FogColorPDS, _FogIntensityPDS);
                return fixed4(c.r, c.g, c.b, 1.0) * baseColor.a;
            }               
            
            ENDCG
        }
    }
}
