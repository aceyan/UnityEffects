using UnityEngine;
using System.Collections;

public class CreateDepthMap : MonoBehaviour 
{
    public Material material;
    public Shader depthMapShader;
	void Start () 
    {
        Camera camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.white;
        camera.SetReplacementShader(depthMapShader, "RenderType");
        RenderTexture depthMap = new RenderTexture(Screen.width, Screen.height, 0);
        depthMap.format = RenderTextureFormat.ARGB32;
        camera.targetTexture = depthMap;
	}

    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination, material);
    //}
}
