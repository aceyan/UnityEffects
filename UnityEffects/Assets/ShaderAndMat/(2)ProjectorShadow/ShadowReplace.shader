Shader "UnityEffects/ShadowReplace"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_AlphaCutoff ("Cutoff", float) = 0.5
	}
	Subshader 
	{
		Tags { "RenderType"="Opaque" "Queue"="Geometry"}
		Pass {
    		Lighting Off Fog { Mode off } 
			SetTexture [_MainTex] {
				constantColor (1,1,1,1)
				combine constant
			}
		}    
	}
	Subshader 
	{
		Tags { "RenderType"="TransparentCutout" "Queue"="AlphaTest"}
		Pass {
    		Lighting Off Fog { Mode off } 
			AlphaTest Greater [_AlphaCutoff]
			Color [_TintColor]
			SetTexture [_MainTex] {
				constantColor (1,1,1,1)
				combine constant, previous * texture
			}
		}    
	}
	Subshader 
	{
		Tags { "RenderType"="TransparentAlphaBlended" "Queue"="Transparent"}
		Pass {
    		Lighting Off Fog { Mode off } 
			ZWrite Off
			Blend SrcAlpha OneMinusSrcAlpha
			Color [_TintColor]
			SetTexture [_MainTex] {
				constantColor (1,1,1,1)
				combine constant, previous * texture
			}
		}    
	}
	Subshader 
	{
		Tags { "RenderType"="TransparentAlphaAdditve" "Queue"="Transparent"}
		Pass {
    		Lighting Off Fog { Mode off } 
			ZWrite Off
			Blend SrcAlpha One
			Color [_TintColor]
			SetTexture [_MainTex] {
				constantColor (1,1,1,1)
				combine constant, previous * texture
			}
		}    
	}
}
