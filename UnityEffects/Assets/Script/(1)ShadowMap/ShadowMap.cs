using UnityEngine;
using System.Collections;
/// <summary>
/// 接受阴影的物体
/// </summary>
public class ShadowMap : MonoBehaviour
{

    private Material _mat;
    private Camera _lightCamera;
    void Start()
    {
        MeshRenderer render = GetComponent<MeshRenderer>();
        _mat = render.material;

        foreach (Camera item in Camera.allCameras)
        {
            if (item.CompareTag("LightCamera"))
            {
                _lightCamera = item;
                break;
            }
        }
    }

    void OnWillRenderObject()
    {
        if (_mat != null && _lightCamera != null)
        {
            //Gl
            //_mat.SetMatrix("_ViewProjectionMat", _lightCamera.projectionMatrix * _lightCamera.worldToCameraMatrix);// unity的camera projectionMatrix是GL风格的列矩阵： http://docs.unity3d.com/ScriptReference/Camera-projectionMatrix.html 投影后的z-[-w,w]
            //真正的平台相关的投影矩阵
            _mat.SetMatrix("_ViewProjectionMat", GL.GetGPUProjectionMatrix(_lightCamera.projectionMatrix, true) * _lightCamera.worldToCameraMatrix);
            _mat.SetTexture("_DepthMap", _lightCamera.targetTexture);
            _mat.SetFloat("_NearClip", _lightCamera.nearClipPlane);
            _mat.SetFloat("_FarClip", _lightCamera.farClipPlane);
        }
    }

}
