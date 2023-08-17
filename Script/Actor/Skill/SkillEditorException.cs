using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillEditorException
{
    public enum eErrorLevel
    {
        Warrning,
        Error,
    }
    public readonly string AttributeName;
    public readonly eErrorLevel Error;
    public readonly string ErrorMessage;
    public SkillEditorException(string name, eErrorLevel level, string message)
    {
        AttributeName = name;
        Error = level;
        ErrorMessage = message;
    }
}
public abstract class SkillData
{
    float _maxWidth;
    public void GUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList)
    {
#if UNITY_EDITOR
        _maxWidth = maxWidth;
        float boxHeightStart = height;
        GUI.Box(new Rect(0, boxHeightStart, _maxWidth, 20f), GetType().Name, UnityEditor.EditorStyles.toolbar);
        height += 20f;
        OnGUIEditor(maxWidth, ref height, ref errorList);
        height += 10f;
#endif
    }
    public void GUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList, global::System.Type type)
    {
#if UNITY_EDITOR
        _maxWidth = maxWidth;
        float boxHeightStart = height;
        GUI.Box(new Rect(0, boxHeightStart, maxWidth, 20f), type.Name != null ? type.Name + "_" + GetType().Name : GetType().Name, UnityEditor.EditorStyles.toolbar);
        height += 20f;
        OnGUIEditor(maxWidth, ref height, ref errorList);
        height += 10f;
#endif
    }
    public void GUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList, int level)
    {
#if UNITY_EDITOR
        _maxWidth = maxWidth;
        float boxHeightStart = height;
        GUI.Box(new Rect(0, boxHeightStart, maxWidth, 20f), GetType().Name + " Level." + level, UnityEditor.EditorStyles.toolbar);
        // GUI.Label(new Rect(0f, height, maxWidth, 20f), "========" + this.GetType().Name + "========");
        height += 20f;
        OnGUIEditor(maxWidth, ref height, ref errorList);
        height += 10f;
#endif
    }
    public float Field(string parameterName, float parameter, ref float height)
    {
#if UNITY_EDITOR
        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, _maxWidth * 2 / 3f, 20f), parameterName + ":");
        parameter = UnityEditor.EditorGUI.FloatField(new Rect(_maxWidth * 2 / 3f, height, _maxWidth * 1 / 3f, 20f), parameter);
        height += 20f;
#endif
        return parameter;
    }
    public int Field(string parameterName, int parameter, ref float height)
    {
#if UNITY_EDITOR
        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, _maxWidth * 2 / 3f, 20f), parameterName + ":");
        parameter = UnityEditor.EditorGUI.IntField(new Rect(_maxWidth * 2 / 3f, height, _maxWidth * 1 / 3f, 20f), parameter);
        height += 20f;
#endif
        return parameter;
    }
    protected abstract void OnGUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList);
}
