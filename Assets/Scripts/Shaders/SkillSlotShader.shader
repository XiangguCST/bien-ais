﻿Shader "Custom/SkillSlotShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_EmissionColor("Emission", Color) = (1, 1, 1, 1)
		_GlowIntensity("Glow Intensity", Range(0, 1)) = 0
		_GrayScale("Gray Scale", Range(0, 1)) = 0
	}
		SubShader
		{
			Tags { "RenderType" = "Opaque" }
			LOD 100

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#include "UnityCG.cginc"

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

				struct v2f
				{
					float2 uv : TEXCOORD0;
					float4 vertex : SV_POSITION;
				};

				sampler2D _MainTex;
				float4 _Color;
				float4 _EmissionColor;
				float _GlowIntensity;
				float _GrayScale;

				v2f vert(appdata v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uv = v.uv;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv) * _Color;
					if (_GrayScale > 0.5)
					{
						float gray = dot(col.rgb, fixed3(0.3, 0.59, 0.11));
						return fixed4(gray, gray, gray, col.a);
					}
					else
					{
						col.rgb += _EmissionColor.rgb * _GlowIntensity;
						return col;
					}
				}
				ENDCG
			}
		}
}