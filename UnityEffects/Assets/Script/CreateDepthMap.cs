using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
        _lightCamera.backgroundColor = Color.white;
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

    void Update()
    {//根据主相机的视锥来确定灯光相机的设置
        _lightCamera.transform.position = _mainCamera.transform.position;
        //1、	求光view矩阵  用world to camera
        Matrix4x4 lgihtw2v = _lightCamera.worldToCameraMatrix;
        //2、	求视锥8顶点 （主相机空间中） n平面（aspect * y, tan(r/2)* n,n）  f平面（aspect*y, tan(r/2) * f, f）
        float r = (_mainCamera.fieldOfView / 180f) * Mathf.PI;
        //n平面
        Vector4 nLeftUp = new Vector4(-_mainCamera.aspect * Mathf.Tan(r / 2) * _mainCamera.nearClipPlane, Mathf.Tan(r / 2) * _mainCamera.nearClipPlane, _mainCamera.nearClipPlane, 1);
        Vector4 nRightUp = new Vector4(_mainCamera.aspect * Mathf.Tan(r / 2) * _mainCamera.nearClipPlane, Mathf.Tan(r / 2) * _mainCamera.nearClipPlane, _mainCamera.nearClipPlane, 1);
        Vector4 nLeftDonw = new Vector4(-_mainCamera.aspect * Mathf.Tan(r / 2) * _mainCamera.nearClipPlane, -Mathf.Tan(r / 2) * _mainCamera.nearClipPlane, _mainCamera.nearClipPlane, 1);
        Vector4 nRightDonw = new Vector4(_mainCamera.aspect * Mathf.Tan(r / 2) * _mainCamera.nearClipPlane, -Mathf.Tan(r / 2) * _mainCamera.nearClipPlane, _mainCamera.nearClipPlane, 1);

        //f平面
        Vector4 fLeftUp = new Vector4(-_mainCamera.aspect * Mathf.Tan(r / 2) * _mainCamera.farClipPlane, Mathf.Tan(r / 2) * _mainCamera.farClipPlane, _mainCamera.farClipPlane, 1);
        Vector4 fRightUp = new Vector4(_mainCamera.aspect * Mathf.Tan(r / 2) * _mainCamera.farClipPlane, Mathf.Tan(r / 2) * _mainCamera.farClipPlane, _mainCamera.farClipPlane, 1);
        Vector4 fLeftDonw = new Vector4(-_mainCamera.aspect * Mathf.Tan(r / 2) * _mainCamera.farClipPlane, -Mathf.Tan(r / 2) * _mainCamera.farClipPlane, _mainCamera.farClipPlane, 1);
        Vector4 fRightDonw = new Vector4(_mainCamera.aspect * Mathf.Tan(r / 2) * _mainCamera.farClipPlane, -Mathf.Tan(r / 2) * _mainCamera.farClipPlane, _mainCamera.farClipPlane, 1);

        Matrix4x4 mainv2w = _mainCamera.cameraToWorldMatrix;
        //3、	把顶点从主相机空间先转换到世界空间，再从世界空间变换到光view空间
        Vector4 vnLeftUp = lgihtw2v * mainv2w * nLeftUp;
        Vector4 vnRightUp = lgihtw2v * mainv2w * nRightUp;
        Vector4 vnLeftDonw = lgihtw2v * mainv2w * nLeftDonw;
        Vector4 vnRightDonw = lgihtw2v * mainv2w * nRightDonw;
        //
        Vector4 vfLeftUp = lgihtw2v * mainv2w * fLeftUp;
        Vector4 vfRightUp = lgihtw2v * mainv2w * fRightUp;
        Vector4 vfLeftDonw = lgihtw2v * mainv2w * fLeftDonw;
        Vector4 vfRightDonw = lgihtw2v * mainv2w * fRightDonw;

        _vList.Clear();
        _vList.Add(vnLeftUp);
        _vList.Add(vnRightUp);
        _vList.Add(vnLeftDonw);
        _vList.Add(vnRightDonw);

        _vList.Add(vfLeftUp);
        _vList.Add(vfRightUp);
        _vList.Add(vfLeftDonw);
        _vList.Add(vfRightDonw);
        //4、	求包围盒 (由于光锥xy轴的对称性，这里求最大包围盒就好，不是严格意义的AABB)
        float maxX = -float.MaxValue;
        float maxY = -float.MaxValue;
        float maxZ = -float.MaxValue;
        float minZ = float.MaxValue;
        for (int i = 0; i < _vList.Count; i++)
        {
            Vector4 v = _vList[i];
            if (Mathf.Abs(v.x) > maxX)
            {
                maxX = Mathf.Abs(v.x);
            }
            if (Mathf.Abs(v.y) > maxY)
            {
                maxY = Mathf.Abs(v.y);
            }
            if (v.z > maxZ)
            {
                maxZ = v.z;
            }
            else if (v.z < minZ)
            {
                minZ = v.z;
            }
        }
        //4.5 优化，如果8个顶点在光锥view空间中的z<0,那么如果n=0，就可能出现应该被渲染depthmap的物体被光锥近裁面剪裁掉的情况，所以z < 0 的情况下要延光照负方向移动光源位置以避免这种情况
        if(minZ < 0)
        {
            _lightCamera.transform.position += -_lightCamera.transform.forward.normalized * Mathf.Abs(minZ);
            maxZ = maxZ - minZ;
        }

        //5、	根据包围盒确定投影矩阵 包围盒的最大z就是f，Camera.orthographicSize由y max决定 ，还要设置Camera.aspect
        _lightCamera.orthographic = true;
        _lightCamera.aspect = maxX / maxY;
        _lightCamera.orthographicSize = maxY;
        _lightCamera.nearClipPlane = 0.1f;
        _lightCamera.farClipPlane = Mathf.Abs(maxZ);
    }
}
