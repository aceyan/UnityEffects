Shader "UnityEffects/ShaowMap"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="ShadowMap" }
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
			uniform matrix _ViewProjectionMat;//视矩阵*投影矩阵
			uniform sampler2D _DepthMap;//深度图
			float4 _DepthMap_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//float4 worldpos = mul(_Object2World, v.vertex);
				//o.projectionPos = mul(  _ViewProjectionMat *  _Object2World, v.vertex);//投影到贴图的齐次剪裁空间

				matrix mvp = mul(_ViewProjectionMat ,_Object2World);
				o.projectionPos = mul( mvp, v.vertex);//投影到贴图的齐次剪裁空间
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//
				float4 uvPos = i.projectionPos;
				uvPos.x = uvPos.x * 0.5 + uvPos.w * 0.5;//变换到[0,w]
				uvPos.y = uvPos.y * 0.5 + uvPos.w * 0.5;//变换到[0,w]
				uvPos = uvPos / uvPos.w;//变换到[0,1]纹理空间

				float depth = tex2D(_DepthMap, uvPos.xy).r;//从深度图中取出深度
				
				float depthPixel = uvPos.z;//像素深度
				fixed4 textureCol = tex2D(_MainTex, i.uv);
				fixed4 shadowCol = depthPixel > depth ? fixed4(0.3,0.3,0.3,1) : 1;

				return textureCol * shadowCol;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
