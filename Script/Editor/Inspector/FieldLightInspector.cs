using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;

[CustomEditor(typeof(FieldLight))]
[CanEditMultipleObjects]
public class FieldLightInspector : Editor
{
    FieldLight _fieldLight;
    Light2D _targetLight;
    FieldLight FieldLight
    {
        get
        {
            if (_fieldLight == null)
            {
                _fieldLight = target as FieldLight;
                _fieldLight.Initialize();
            }
            if (_targetLight == null)
                _targetLight = _fieldLight.GetComponent<Light2D>();
            return _fieldLight;
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (Application.isPlaying)
            return;

        EditorGUI.BeginChangeCheck();
        FieldLight.SetTime();
        if (EditorGUI.EndChangeCheck())
            EditorUtility.SetDirty(target);
    }
}
