using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public enum eStageEditorType
{
    None,
    MonsterEditor,
    ObjectEditor,
}
#if UNITY_EDITOR
[ExecuteInEditMode]
public class StageRuntimeEditor : MonoBehaviour
{
    Transform _pivot;
    public Vector2Int Offset;
    public eStageEditorType StageEditorType;

    #region MonsterEditor
    public SpawnData Data;
    SpawnHandler _spawnHandler;
    public void SetSpawnHandler(SpawnHandler spawnHandler)
    {
        _spawnHandler = spawnHandler;
    }
    #endregion
    #region ObjectEditor
    Transform _objectPivot;
    public int CurrentSpawnIndex;
    StaticActor _currentSpawnObject;
    bool IsOverlapObject() => _currentSpawnObject != null ? GridManager.Instance.IsOverlapObject(_currentSpawnObject) : false;
    public void SetObject(Data.ObjectData objectData)
    {
        if(objectData == null)
        {
            if(_currentSpawnObject != null)
            {
                DestroyImmediate(_currentSpawnObject.gameObject);
                _currentSpawnObject = null;
                CurrentSpawnIndex = 0;
            }
            return;
        }

        if (CurrentSpawnIndex == objectData.Index)
            return;

        SceneView.lastActiveSceneView.showGrid = false;
        CurrentSpawnIndex = objectData.Index;
        // string prefabPath = $"Assets/Resources/{objectData.ResourcePath}.prefab";
        _currentSpawnObject = PrefabUtility.InstantiatePrefab(Resources.Load< StaticActor>(objectData.ResourcePath), _pivot.Find("ObjectPivot")) as StaticActor;
        _currentSpawnObject.Index = objectData.Index;
        _currentSpawnObject.CurrentHP = objectData.HP;
        Debug.Log(_currentSpawnObject.Index);
    }
    #endregion


    public void Initialize()
    {
        _pivot = GameObject.Find("ScenePivot").transform; 
        Offset = new Vector2Int(Mathf.RoundToInt(_pivot.position.x), Mathf.RoundToInt(_pivot.position.y));
        _objectPivot = _pivot.Find("ObjectPivot");
        SceneView.duringSceneGui += OnSceneViewGUI;

        GridManager.Instance.Initialize();
        for (int i = 0; i < _objectPivot.childCount; ++i)
        {
            ITileSetable tileSetable = _objectPivot.GetChild(i).GetComponent<ITileSetable>();
            if (tileSetable != null)
                GridManager.Instance.ModifyTileState(tileSetable);
        }
        Transform housePivot = _pivot.Find("HousePivot");
        if (housePivot)
        {
            for (int i = 0; i < housePivot.childCount; ++i)
            {
                ITileSetable tileSetable = housePivot.GetChild(i).GetComponent<ITileSetable>();
                if (tileSetable != null)
                    GridManager.Instance.ModifyTileState(tileSetable);
            }
        }
    }
    void OnSceneViewGUI(SceneView view)
    {
        switch (StageEditorType)
        {
            case eStageEditorType.ObjectEditor:
                if (_currentSpawnObject != null)
                {
                    Event e = Event.current;
                    int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
                    Vector2Int tilePosition = (Vector2Int)GridManager.Instance.GetTilePosition;
                    switch (e.type)
                    {
                        case EventType.MouseMove:
                            _currentSpawnObject.TilePosition = tilePosition;
                            _currentSpawnObject.transform.position = (Vector3Int)tilePosition;
                            break;
                        case EventType.MouseDown:
                            switch (e.button)
                            {
                                case 0:
                                    // LeftDown
                                    if(IsOverlapObject())
                                        return;

                                    _currentSpawnObject.transform.position = (Vector3Int)tilePosition;
                                    GridManager.Instance.ModifyTileState(_currentSpawnObject);

                                    CurrentSpawnIndex = 0;
                                    Selection.activeObject = _currentSpawnObject;
                                    _currentSpawnObject = null;

                                    EditorUtility.SetDirty(this);
                                    break;
                                case 1:
                                    // RightDown
                                    CurrentSpawnIndex = 0;
                                    DestroyImmediate(_currentSpawnObject.gameObject);
                                    _currentSpawnObject = null; 
                                    break;
                            }
                            GUIUtility.hotControl = controlID;
                            Event.current.Use();
                            break;
                    }
                }
                break;
        }
    }
    private void OnDestroy()
    {
        DestroyImmediate(GridManager.Instance.gameObject);
        SceneView.duringSceneGui -= OnSceneViewGUI;
        if (_currentSpawnObject != null)
            DestroyImmediate(_currentSpawnObject.gameObject);
    }
    public void OnDrawGizmos()
    {
        switch (StageEditorType)
        {
            case eStageEditorType.MonsterEditor:
                {
                    if (_spawnHandler != null)
                    {
                        for (int i = 0; i < _spawnHandler.SpawnDataList.Count; ++i)
                        {
                            if (_spawnHandler.SpawnDataList[i] == Data) Gizmos.color = Color.black;
                            else Gizmos.color = new Color(0f, 0f, 0f, 1f);

                            Gizmos.DrawCube(_spawnHandler.Offset + _spawnHandler.SpawnDataList[i].SpawnRect.position, _spawnHandler.SpawnDataList[i].SpawnRect.size);
                        }
                    }
                }
                break;
            case eStageEditorType.ObjectEditor:
                {
                    if (_currentSpawnObject != null)
                    {
                        if (_currentSpawnObject.GetTileBlockCoordList != null)
                        {
                            for (int i = 0; i < _currentSpawnObject.GetTileBlockCoordList.Count; ++i)
                            {
                                if (GridManager.Instance.IsOverlapObject(_currentSpawnObject.GetTileBlockCoordList[i] + _currentSpawnObject.TilePosition))
                                    Gizmos.color = new Color(1f, 0f, 0f, 0.3f);
                                else
                                    Gizmos.color = new Color(0f, 1f, 0f, 0.3f);

                                Vector3 tilePosition = new Vector3
                                {
                                    x = _currentSpawnObject.TilePosition.x + 0.5f,
                                    y = _currentSpawnObject.TilePosition.y + 0.5f,
                                    z = 0,
                                };
                                Gizmos.DrawCube(tilePosition + (Vector3Int)_currentSpawnObject.GetTileBlockCoordList[i], Vector2.one);
                            }
                        }
                        else
                        {
                            Vector3 tilePosition = new Vector3
                            {
                                x = _currentSpawnObject.transform.position.x + 0.5f,
                                y = _currentSpawnObject.transform.position.y + 0.5f,
                                z = 0,
                            };
                            Gizmos.DrawCube(tilePosition, Vector2.one);
                        }
                    }
                    Gizmos.color = new Color(1f, 1f, 1f, 1f);
                    for (int i = -50; i <= 50; ++i)
                        Gizmos.DrawLine(transform.position + new Vector3(i, -50), transform.position + new Vector3(i, 50));
                    for (int i = -50; i <= 50; ++i)
                        Gizmos.DrawLine(transform.position + new Vector3(-50, i), transform.position + new Vector3(50, i));
                }
                break;
        }
    }
}
#endif