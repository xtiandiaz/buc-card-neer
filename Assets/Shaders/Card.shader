// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Card" 
{
    Properties 
    {
        [PerRendererData] _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
        [PerRendererData] _ItemTex ("Item Texture", 2D) = "white" {}
    }

    SubShader 
    {
        Tags 
        {
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent"
        }
        
        LOD 100
        
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
    
        Pass 
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t 
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f 
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoord1 : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _ItemTex;
            float4 _MainTex_ST;
            float4 _ItemTex_ST;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.texcoord1 = TRANSFORM_TEX(v.texcoord1, _ItemTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 baseCol = tex2D(_MainTex, i.texcoord);
                fixed4 itemCol = tex2D(_ItemTex, i.texcoord1);
                fixed3 col = baseCol.rgb + itemCol.rgb * itemCol.a;
                return fixed4(col.r, col.g, col.b, baseCol.a);
            }
            
            ENDCG
        }
    }
}
