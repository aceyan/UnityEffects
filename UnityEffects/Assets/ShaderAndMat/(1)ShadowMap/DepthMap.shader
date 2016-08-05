Shader "UnityEffects/DepthMap"
{
	//生成深度图
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
				float4 texPos : TEXCOORD1;
			};

			struct v2f
			{
				float4 texPos : TEXCOORD1;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
            uniform sampler2D _CameraDepthTexture;

			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texPos = o.vertex;
				o.texPos.x = o.vertex.x * 0.5f + 0.5f * o.vertex.w;//变换到x[0,w] y[0,w]的空间
				o.texPos.y = o.vertex.y * 0.5f  + 0.5f * o.vertex.w;
				if (_ProjectionParams.x < 0)//uv空间与平台有关
					 o.texPos.y = o.vertex.w - o.texPos.y;//http://docs.unity3d.com/Manual/SL-PlatformDifferences.html
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = float4(0,0,0,1);
				
				//todo test 也可以利用Linear01Depth把深度差值到线性的01空间，0表示eye位置，1表示远裁平面位置。这里的线性是指与投影前view空间的z成线性关系，直接从_CameraDepthTexture中取出的深度是与1/z成线性关系的，但也依旧是深度越大表示离eye越远
				//float z = Linear01Depth(tex2D(_CameraDepthTexture, i.texPos.xy / i.texPos.w).r);//先把texPos归一化到 [0,1] 纹理坐标空间，对深度图进行采样

				//unity 生成的深度图的深度保存在r通道中

				float z = tex2D(_CameraDepthTexture, i.texPos.xy / i.texPos.w).r;//先把texPos归一化到 [0,1] 纹理坐标空间，对深度图进行采样

				return EncodeFloatRGBA(z);
			}
			ENDCG
		}
	}

	Subshader 
	{
		Tags { "RenderType"="Opaque"}
		Pass {
    		Lighting Off Fog { Mode off } 
			SetTexture [_MainTex] {
				constantColor (1,1,1,1)
				combine constant
			}
		}    
	}  
	FallBack "Diffuse"//要想在ReplacementShader使用_CameraDepthTexture，那么记得增加shadowCaster pass,或者FallBack一个
}
