using UnityEngine;
using System.Collections;

public class DebugLog : MonoBehaviour {

    private static float _uiSize = 1f;
    private static int _fontSize = 14;
    private static GUIStyle _logStyle;
    private static float _lastCalculateTime = 0;
    private static uint _tam;
    private static uint _trm;
    private static float _fps;
    //
    private static int _gms;
    private static int _sms;
    private static int _pc;

    private void Start()
    {
        _gms = SystemInfo.graphicsMemorySize;
        _sms = SystemInfo.systemMemorySize;
        _pc = SystemInfo.processorCount;
        if (Application.isMobilePlatform)
        {
            _uiSize = Screen.dpi / 295;
        }
        _fontSize = (int)(14 * _uiSize);
        _logStyle = new GUIStyle();
        _logStyle.normal.textColor = new Color(0.6f, 0, 0);
        _logStyle.fontSize = _fontSize;
    }
   private void Update()
    {
        if (Time.realtimeSinceStartup - _lastCalculateTime >= 0.5f)
        {
            _tam = Profiler.GetTotalAllocatedMemory() / 1024 / 1024;
            _trm = Profiler.GetTotalReservedMemory() / 1024 / 1024;
            _fps = 1 / Time.deltaTime;
            _lastCalculateTime = Time.realtimeSinceStartup;

        }
    }     
   private void OnGUI()
   {
       GUI.skin.textField.fontSize = _fontSize;
       GUI.skin.button.fontSize = _fontSize;
       GUI.TextField(new Rect(0, 0, 250 * _uiSize, 70 * _uiSize), "系统显存:" + _gms + " 系统内存:" + _sms + " 核心数:" + _pc + "\n总内存:" + _tam + " 总保留内存:" + _trm + "\nFPS: " + _fps.ToString("f2") + "\ndpi:" + Screen.dpi);
   }
}
