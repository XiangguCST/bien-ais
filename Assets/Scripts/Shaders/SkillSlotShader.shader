Shader "Custom/SkillSlotShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_EmissionColor("Emission", Color) = (1, 1, 1, 1)
		_GlowIntensity("Glow Intensity", Range(0, 1)) = 0
		_GrayScale("Gray Scale", Range(0, 1)) = 0
		_OutlineColor("Outline Color", Color) = (1,1,1,1)
		_Outline("Outline width", Range(0.01, 0.03)) = 0.01
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

				// 定义输入结构
				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
				};

		// 定义输出结构
		struct v2f
		{
			float2 uv : TEXCOORD0;
			float4 vertex : SV_POSITION;
		};

		// 定义Shader属性
		sampler2D _MainTex;
		float4 _Color;
		float4 _EmissionColor;
		float _GlowIntensity;
		float _GrayScale;
		float4 _OutlineColor;
		float _Outline;

		// 顶点着色器
		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = UnityObjectToClipPos(v.vertex);
			o.uv = v.uv;
			return o;
		}

		// 片段着色器
		fixed4 frag(v2f i) : SV_Target
		{
			// 获取纹理颜色
			fixed4 col = tex2D(_MainTex, i.uv) * _Color;

		// 判断是否需要应用灰度效果
		if (_GrayScale > 0)
		{
			// 计算灰度
			float gray = dot(col.rgb, fixed3(0.3, 0.59, 0.11));
			// 平滑过渡到灰度效果
			col.rgb = lerp(col.rgb, gray.xxx, _GrayScale);
		}

		// 添加辉光效果
		col.rgb += _EmissionColor.rgb * _GlowIntensity;

		// 计算描边
		float2 uv_step = float2(_Outline, _Outline);
		float alpha = min(min(i.uv.x, i.uv.y), min(1.0 - i.uv.x, 1.0 - i.uv.y));
		if (alpha < _Outline)
		{
			col = _OutlineColor;
		}
		return col;
	}
	ENDCG
}
		}
}
