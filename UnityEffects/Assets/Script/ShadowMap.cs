using UnityEngine;
using System.Collections;

public class ShadowMap : MonoBehaviour
{

    private Material _mat;
    private Camera _lightCamera;
    // Use this for initialization
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

    // Update is called once per frame
    void Update()
    {

    }

    void OnWillRenderObject()
    {
        if (_mat != null && _lightCamera != null)
        {
            _mat.SetMatrix("_ViewProjectionMat", _lightCamera.projectionMatrix * _lightCamera.worldToCameraMatrix);//注意cg中用的是左乘，向量是列向量
            _mat.SetTexture("_DepthMap", _lightCamera.targetTexture);
        }
    }

}
