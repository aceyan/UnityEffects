using UnityEngine;
using System.Collections;

public class CreateDepthMap : MonoBehaviour 
{

    public Shader depthMapShader;
    public RenderTexture depthMap;
    public Material material;
	// Use this for initialization
	void Start () 
    {
        Camera camera = GetComponent<Camera>();
        camera.depthTextureMode = DepthTextureMode.Depth;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = Color.white;
        camera.SetReplacementShader(depthMapShader, "RenderType");
        camera.targetTexture = depthMap;
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination, material);
    //}
}
