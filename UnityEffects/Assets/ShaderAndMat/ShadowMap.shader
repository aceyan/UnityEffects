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
			uniform float _NearClip;
			uniform float _FarClip;
			float4 _DepthMap_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//float4 worldpos = mul(_Object2World, v.vertex);
				//o.projectionPos = mul(  _ViewProjectionMat *  _Object2World, v.vertex);//投影到贴图的齐次剪裁空间

				matrix mvp = mul(_ViewProjectionMat ,_Object2World);
				o.projectionPos = mul( mvp,float4(v.vertex.xyz,1));//投影到贴图的齐次剪裁空间
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
				//
				float4 uvPos = i.projectionPos;
				uvPos.x = uvPos.x * 0.5f + uvPos.w * 0.5f;//变换到[0,w]
				uvPos.y = uvPos.y * 0.5f * scale + uvPos.w * 0.5f;//变换到[0,w]
				//uvPos.z = uvPos.z * 0.5f + uvPos.w * 0.5f;//变换到[0,w]
				//uvPos = uvPos / uvPos.w;//变换到[0,1]纹理空间


				float depth =  DecodeFloatRGBA(tex2D(_DepthMap, uvPos.xy/ uvPos.w));//从深度图中取出深度

				float depthPixel = uvPos.z / uvPos.w;//像素深度,要分openGL和Dx平台来
				//if(uvPos.z > 0)
				//{
					//return float4(1,0,0,1);
				//}
				//else
				//{
				//	return float4(0,1,0,1);
				//}

				//公式：http://www.humus.name/temp/Linearize%20depth.txt
				//depthPixel = _NearClip * (depthPixel + 1.0) / (_FarClip + _NearClip - depthPixel * (_FarClip - _NearClip));



				//depthPixel = (2 * _NearClip) / (_FarClip + _NearClip - depthPixel * (_FarClip - _NearClip));
				depthPixel =  _NearClip / (_FarClip - depthPixel*(_FarClip - _NearClip));
				float4 textureCol = tex2D(_MainTex, i.uv);
				float4 shadowCol = (depthPixel - depth) > 0.0002 ? 0.3 : 1;

				return textureCol * shadowCol;
				
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
