using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CreateDepthMap : MonoBehaviour 
{
    public Material material;
    public Shader depthMapShader;
    private Camera _mainCamera;
    private Camera _lightCamera;
    private List<Vector4> _vList = new List<Vector4>();
	void Start () 
    {
        _lightCamera = GetComponent<Camera>();
        _lightCamera.depthTextureMode = DepthTextureMode.Depth;
        _lightCamera.clearFlags = CameraClearFlags.SolidColor;
        _lightCamera.backgroundColor = Color.white;
        _lightCamera.SetReplacementShader(depthMapShader, "RenderType");
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
        //1、	求光view矩阵  用world to camera
        Matrix4x4 viewM = _lightCamera.worldToCameraMatrix;
        //2、	求视锥8顶点  n平面（aspect * y, tan(r/2)* n,n）  f平面（aspect*y, tan(r/2) * f, f）
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

        //3、	把顶点变换到光view空间
        Vector4 vnLeftUp = viewM * nLeftUp;
        Vector4 vnRightUp = viewM * nRightUp;
        Vector4 vnLeftDonw = viewM * nLeftDonw;
        Vector4 vnRightDonw = viewM * nRightDonw;
        //
        Vector4 vfLeftUp = viewM * fLeftUp;
        Vector4 vfRightUp = viewM * fRightUp;
        Vector4 vfLeftDonw = viewM * fLeftDonw;
        Vector4 vfRightDonw = viewM * fRightDonw;

        _vList.Clear();
        _vList.Add(vnLeftUp);
        _vList.Add(vnRightUp);
        _vList.Add(vnLeftDonw);
        _vList.Add(vnRightDonw);

        _vList.Add(vfLeftUp);
        _vList.Add(vfRightUp);
        _vList.Add(vfLeftDonw);
        _vList.Add(vfRightDonw);
        //4、	求包围盒 (由于光锥的对称性，这里求最大包围盒就好，不是严格意义的AABB)
        float maxX = 0;
        float maxY = 0;
        float maxZ = 0;

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
            if (Mathf.Abs(v.z) > maxZ)
            {
                maxZ = Mathf.Abs(v.z);
            }
        }
        //5、	根据aabb确定投影矩阵 包围盒的最大z就是f，Camera.orthographicSize由y max决定 ，还要设置Camera.aspect
        _lightCamera.orthographic = true;
        _lightCamera.aspect = maxX / maxY;
        _lightCamera.orthographicSize = maxY;
        _lightCamera.nearClipPlane = 0.1f;
        _lightCamera.farClipPlane = maxZ;
    }
    //void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination, material);
    //}
}
