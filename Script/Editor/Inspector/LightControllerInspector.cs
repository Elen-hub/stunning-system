using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LightController))]
public class LightControllerInspector : Editor
{
    bool _isInit;
    LightController _controller;
    LightController Controller {
        get {
            if (_controller == null)
            {
                _controller = target as LightController;
                _controller.Initialize();
            }
            return _controller; 
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(20);
        if (GUILayout.Button("Initialize", GUILayout.MinWidth(60f), GUILayout.MaxWidth(60f)))
        {
            _isInit = true;
            Controller.Initialize();
        }
        GUILayout.Space(10f);
        if (!_isInit)
            return;

        eTimeTag timeTag = (eTimeTag)EditorGUILayout.EnumPopup(Controller.GetTimeTag);
        Controller.SetTimeTag(timeTag);
    }
}
