using UnityEngine;
using System.Collections;

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
           //_mat.SetMatrix("_ViewProjectionMat", _lightCamera.projectionMatrix * _lightCamera.worldToCameraMatrix);//我发现这个投影矩阵式z-[-w,w]的，原来这个矩阵并不是mvp中的m： http://docs.unity3d.com/ScriptReference/Camera-projectionMatrix.html
            //maybe Dx
            _mat.SetMatrix("_ViewProjectionMat", GL.GetGPUProjectionMatrix(_lightCamera.projectionMatrix, true) * _lightCamera.worldToCameraMatrix);
            _mat.SetTexture("_DepthMap", _lightCamera.targetTexture);
            _mat.SetFloat("_NearClip", _lightCamera.nearClipPlane);
            _mat.SetFloat("_FarClip", _lightCamera.farClipPlane);
        }
    }

}
