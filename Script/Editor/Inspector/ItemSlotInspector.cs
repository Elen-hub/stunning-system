using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;

[CustomEditor(typeof(ItemSlot), true)]
[CanEditMultipleObjects]
public class ItemSlotInspector : ButtonEditor
{
    SerializedProperty _isPossiblePushProperty;
    protected override void OnEnable()
    {
        _isPossiblePushProperty = serializedObject.FindProperty("IsPossiblePush");

        base.OnEnable();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_isPossiblePushProperty, true);
        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
