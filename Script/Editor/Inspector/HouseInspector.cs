using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(House), true)]
[CanEditMultipleObjects]
public class HouseInspector : Editor
{
    House _target;
    private void OnEnable()
    {
        SceneView.duringSceneGui += SceneGUI;
    }
    private void OnDisable()
    {
        _target.EditorState = House.eEdtiorState.None; 
        _target.IsTileSetMode = false;
        SceneView.duringSceneGui -= SceneGUI;
    }
    public override void OnInspectorGUI()
    {
        if (_target == null)
            _target = target as House;

        base.OnInspectorGUI();

        GUILayout.Space(20);
        if (GUILayout.Button("House EditMode"))
            _target.IsEditMode = !_target.IsEditMode;

        GUILayout.Space(5);

        if (OnTileSetEditor())
            return;

        EditorGUI.BeginChangeCheck();

        if (EditorGUI.EndChangeCheck())
            serializedObject.ApplyModifiedProperties();
    }
    void OnUpdateTile()
    {
        if (_target == null)
            return;

        if (_target.EditorState != House.eEdtiorState.TileSet)
            return;

        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
        Vector2Int tilePosition = (Vector2Int)GridManager.Instance.GetTilePosition;
        switch (e.type)
        {
            case EventType.MouseMove:
                SceneView.RepaintAll();
                _target.TilePosition = tilePosition;
                break;
            case EventType.MouseDown:
                switch (e.button)
                {
                    case 0:
                        // LeftDown
                        _target.OnClickTileBlock(tilePosition);
                        UnityEditor.EditorUtility.SetDirty(target);
                        break;
                    case 1:
                        // RightDown
                        _target.EditorState = House.eEdtiorState.None;
                        _target.IsTileSetMode = false;
                        break;
                }
                GUIUtility.hotControl = controlID;
                Event.current.Use();
                break;
        }
    }
    bool OnTileSetEditor()
    {
        //if (PrefabUtility.GetPrefabType(target) != PrefabType.PrefabInstance)
        //    return false;

        if (GUILayout.Button("Set Tile block"))
        {
            if (_target.IsTileSetMode)
            {
                _target.EditorState = House.eEdtiorState.None;
                _target.IsTileSetMode = false;
            }
            else
            {
                _target.EditorState = House.eEdtiorState.TileSet;
                _target.IsTileSetMode = true;
            }
        }
        return false;
    }
    void SceneGUI(SceneView view)
    {
        OnUpdateTile();
    }
}
