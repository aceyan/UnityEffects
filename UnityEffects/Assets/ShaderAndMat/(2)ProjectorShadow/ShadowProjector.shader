Shader "UnityEffects/ShadowProjector" {
	Properties {
		_ShadowTex ("Shadow", 2D) = "gray" {}
		_bulerWidth ("BulerWidth", float) = 1
	}
	SubShader {
		Tags { "Queue"="Transparent" }
		Pass {
			ZWrite Off
			ColorMask RGB
			Blend DstColor Zero
			Offset -1, -1

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos:POSITION;
				float4 sproj:TEXCOORD0;
			};

			float4x4 _Projector;
			sampler2D _ShadowTex;
			uniform half4 _ShadowTex_TexelSize;
			float _bulerWidth;

			v2f vert(float4 vertex:POSITION){
				v2f o;
				o.pos = mul(UNITY_MATRIX_MVP, vertex);
				o.sproj = mul(_Projector, vertex);
				return o;
			}

			float4 frag(v2f i):COLOR{
				float4 finalCol = float4(0.4,0.4,0.4,0.4);
				float a = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.sproj)).a;
				float4 uv4= UNITY_PROJ_COORD(i.sproj);
				float2 uv = uv4.xy / uv4.w ;

				a += tex2D(_ShadowTex, uv + _ShadowTex_TexelSize.xy * _bulerWidth * float2(1,0)).a;
				a += tex2D(_ShadowTex, uv + _ShadowTex_TexelSize.xy * _bulerWidth * float2(0,1)).a;
				a += tex2D(_ShadowTex, uv + _ShadowTex_TexelSize.xy * _bulerWidth * float2(-1,0)).a;
				a += tex2D(_ShadowTex, uv + _ShadowTex_TexelSize.xy * _bulerWidth * float2(0,-1)).a;

			
				a = a/5;
				if(a > 0)
				{
					return  float4(1,1,1,1) - finalCol * a;
				}
				else
				{
					return float4(1,1,1,1) ;
				}
			}

			ENDCG
		}
	} 
	FallBack "Diffuse"
}
