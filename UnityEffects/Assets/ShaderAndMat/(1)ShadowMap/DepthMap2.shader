Shader "UnityEffects/DepthMap2"
{
	//生成深度图,直接绘制物体深度
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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
			};



			sampler2D _MainTex;
			float4 _MainTex_ST;
    
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float z = i.vertex.z / i.vertex.w;//剪裁空间深度
				
				if(UNITY_NEAR_CLIP_VALUE < 0)//https://docs.unity3d.com/Manual/SL-PlatformDifferences.html
				{//GL
					z = z / 2 + 0.5;//GL需要将深度值转到[0,1]
				}

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
}
