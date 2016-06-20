Shader "UnityEffects/ShaowMap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
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
				float4 projectionPos :  TEXCOORD1;//投影坐标
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 projectionPos :  TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			uniform matrix _ProjectionMat;//灯光相机投影矩阵

			uniform sampler2D _DepthMap;//深度图
			float4 _DepthMap_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float4 worldpos = mul(_Object2World, v.vertex);
				projectionPos = mul(_ProjectionMat, worldpos);//投影到贴图的齐次剪裁空间
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
