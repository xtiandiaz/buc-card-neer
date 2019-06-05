Shader "Sly Tides / Sea"
{
    Properties 
	{
        _MainTex("Base (RGB)", 2D) = "white" {}
        _FadeColor("Fade Color (RGB)", Color) = (1,1,1,1)
        _StartColor("Start Color (RGB)", Color) = (1,1,1,1)
        _EndColor("End Color (RGB)", Color) = (1,1,1,1)
        _Curvature("Curvature", Range(0, 1)) = 0
        _Depth("Depth", Range(0, 100)) = 0
        _PivotOffset("Pivot Offset", Float) = 0
        _TidalFrequency("Tidal Frequency", Range(0, 50)) = 0
        _TidalAmplitude("Tidal Amplitude", Range(0, 1)) = 0
  	}
	
	SubShader 
	{
		Tags 
		{ 
		    "RenderType"="Opaque" 
		    "Queue"="Geometry"
		}
		
		Pass
		{
			Lighting Off
			ZWrite On
			//ZTest Always

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			half4 _FadeColor;
			half4 _StartColor;
			half4 _EndColor;
			float _Curvature;
			float _Depth;
			float _PivotOffset;
			float _TidalFrequency;
			float _TidalAmplitude;
			
			struct appdata
            {
                float3 pos : POSITION;
                float3 uv : TEXCOORD0;
            };
			
			struct v2f 
			{
			    float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };
        
            v2f vert(appdata IN)
            {
                v2f o;
                
                float factor = saturate(IN.pos.y - _PivotOffset);
                float3 vOffset = float4(
                    0, 
                    0, 
                    (cos(IN.pos.x * _TidalFrequency + _Time[2]) + sin(IN.pos.y * _TidalFrequency + _Time[1])) * _TidalAmplitude, 
                    0.0f);
                
                o.pos = UnityObjectToClipPos(IN.pos + vOffset);
                
                o.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                
                return o;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
			    float v = IN.uv.y / _MainTex_ST.y;
			    return lerp(_StartColor, _EndColor, IN.uv.y);
			}
			ENDCG
		}
	}
	FallBack "Mobile/Unlit"
}