using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
[CustomEditor(typeof(EquipSlot), true)]
[CanEditMultipleObjects]
public class EquipSlotInspector : ButtonEditor
{
    SerializedProperty _equipSlotTypeProperty;
    SerializedProperty _accessItemTypeProperty;
    protected override void OnEnable()
    {
        base.OnEnable();

        _equipSlotTypeProperty = serializedObject.FindProperty("EquipSlotType");
        _accessItemTypeProperty = serializedObject.FindProperty("AccessItemType");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(_equipSlotTypeProperty, true);
        EditorGUILayout.PropertyField(_accessItemTypeProperty, true);

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
}
