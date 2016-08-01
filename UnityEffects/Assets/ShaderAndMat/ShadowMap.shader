Shader "UnityEffects/ShadowMap"
{
	//渲染接受阴影物体
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
				matrix mvp = mul(_ViewProjectionMat ,_Object2World);
				o.projectionPos = mul( mvp,float4(v.vertex.xyz,1));//投影到贴图的齐次剪裁空间
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			
				//
				float4 uvPos = i.projectionPos;
				uvPos.x = uvPos.x * 0.5f + uvPos.w * 0.5f;//变换到[0,w]
				uvPos.y = uvPos.y * 0.5f    + uvPos.w * 0.5f;//变换到[0,w]
				//我们要把投影的点映射到纹理，就必须考虑不同平台uv空间y的方向
				#if UNITY_UV_STARTS_AT_TOP
				//Dx like
				uvPos.y = uvPos.w - uvPos.y;
				#endif


				float depth =  DecodeFloatRGBA(tex2D(_DepthMap, uvPos.xy/ uvPos.w));//从深度图中取出深度

				float depthPixel = uvPos.z / uvPos.w;//像素深度


				#if (defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)) && defined(SHADER_API_MOBILE)
				//GL like
				depthPixel = depthPixel * 0.5f + 0.5; 

				//todo test
				//如果DepthMap.shader使用了Linear01Depth转换后的深度来生成深度图，那么这里也要将像素深度进行相应的插值
				//深度差值公式：http://www.humus.name/temp/Linearize%20depth.txt
				//depthPixel = (2 * _NearClip) / (_FarClip + _NearClip - depthPixel * (_FarClip - _NearClip));
				
				#else
				//DX like
				depthPixel = depthPixel;

				//todo test
				//如果DepthMap.shader使用了Linear01Depth转换后的深度来生成深度图，那么这里也要将像素深度进行相应的插值
				//depthPixel =  _NearClip / (_FarClip - depthPixel*(_FarClip - _NearClip));

				#endif

				float4 textureCol = tex2D(_MainTex, i.uv);

				//使用一个偏移值，手动调整深度的误差
				float4 shadowCol = (depthPixel - depth > 0.002)  ? 0.3 : 1;

				return textureCol * shadowCol;
				
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
