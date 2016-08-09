using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Assets.Script.Utils;

/// <summary>
/// 生成灯光的深度图
/// </summary>
public class CreateDepthMap : MonoBehaviour 
{
    public Shader depthMapShader;
    private Camera _mainCamera;//主相机
    private Camera _lightCamera;//灯光相机
    private List<Vector4> _vList = new List<Vector4>();
	void Start () 
    {
        _lightCamera = GetComponent<Camera>();
        _lightCamera.depthTextureMode = DepthTextureMode.Depth;
        _lightCamera.clearFlags = CameraClearFlags.SolidColor;
        _lightCamera.backgroundColor = Color.white;//背景色设为白色，表示背景的地方离视点最远，不会受到阴影的影响
        _lightCamera.SetReplacementShader(depthMapShader, "RenderType");//使用替换渲染方式为知道的renderType类型生成深度图
        RenderTexture depthMap = new RenderTexture(Screen.width, Screen.height, 0);
        depthMap.format = RenderTextureFormat.ARGB32;
        _lightCamera.targetTexture = depthMap;
        //
        foreach (Camera item in Camera.allCameras)
        {
            if (item.CompareTag("MainCamera"))
            {
                _mainCamera = item;
                break;
            }
        }
	}

    void LateUpdate()
    {
        ShadowUtils.SetLightCamera(_mainCamera, _lightCamera);
    }
}
