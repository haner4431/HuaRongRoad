// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/UV rotation"
{
	Properties
	{
	    _Color("Color",Color) = (1,1,1,1)
		_MainTex("Texture", 2D) = "white" {}
	_Angle("Angle", Range(-5.0,  5.0)) = 0.0
	}
		SubShader
	{
		Tags{ "RenderType" = "Opaque" }
 
			Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
	 
			struct v2f {
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};
	 
		float _Angle;
	 
		v2f vert(appdata_base v)
		{
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
	 
			float2 pivot = float2(0.5, 0.5);         //中心点
	 
			float cosAngle = cos(_Angle);
			float sinAngle = sin(_Angle);
			float2x2 rot = float2x2(cosAngle, -sinAngle, sinAngle, cosAngle);  //确定以z轴旋转
	 
			float2 uv = v.texcoord.xy - pivot;
			o.uv = mul(rot, uv);           //旋转矩阵相乘变换坐标
			o.uv += pivot;
			
			return o;
		}
	 
		sampler2D _MainTex;
	    fixed4 _Color;
		fixed4 frag(v2f i) : SV_Target
		{
	 
			return tex2D(_MainTex , i.uv)* _Color ;
		}
	 
			ENDCG
		}
	}
}