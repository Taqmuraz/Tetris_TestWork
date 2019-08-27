Shader "Unlit/GameFieldShader"
{
	Properties
	{
		_SizeX ("Size X", int) = 10
		_SizeY ("Size Y", int) = 20
		_CeilWidth ("Border width", float) = 0.01
		_Color_0 ("Color_0", Color) = (1,1,1,1)
		_Color_1 ("Color_1", Color) = (1,1,1,1)
		_Color_2 ("Color_2", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
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

			int _SizeX;
			int _SizeY;
			fixed _CeilWidth;
			half4 _Color_0;
			half4 _Color_1;
			half4 _Color_2;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float2 size = float2(_SizeX, _SizeY);
				float2 pos = i.uv * size;
				int2 pos_int = (int2)pos;

				float2 delta = abs(pos - pos_int);

				float4 clr;

				if (delta.x < _CeilWidth || delta.y < _CeilWidth || (1 - delta.x) < _CeilWidth || (1 - delta.y) < _CeilWidth)
				{
					clr = _Color_2;
				} else
				{
					if ((pos_int.x + pos_int.y) & 1)
					{
						clr = _Color_0;
					} else
					{
						clr = _Color_1;
					}
				}


				return clr;
			}
			ENDCG
		}
	}
}
