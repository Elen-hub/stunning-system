using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;

[DisallowMultipleComponent]
[CustomEditor(typeof(StaticActor), true)]
public class StaticActorInspector : Editor
{
    StaticActor _targetStaticActor;
    int _selectionIndex;
    void OnEnable()
    {
        SceneView.duringSceneGui += SceneGUI;
    }
    public override void OnInspectorGUI()
    {
        if(_targetStaticActor == null)
            _targetStaticActor = target as StaticActor;

        base.OnInspectorGUI();

        GUILayout.Space(20);
        if (GUILayout.Button("StaticActor EditMode"))
            _targetStaticActor.IsEditMode = !_targetStaticActor.IsEditMode;

        if (!_targetStaticActor.IsEditMode)
            return;

        GUILayout.Space(5);
        if (target is InteractObject && IsPossibleAttachment())
            if (GUILayout.Button("Attach Interact"))
                OnAttachmentInteract();

        if (OnTileSetEditor())
            return;
    }
    bool IsPossibleAttachment()
    {
        StaticActor actor = target as StaticActor;
        for (int i = 0; i < actor.transform.childCount; ++i)
            if (actor.transform.GetChild(i).gameObject.layer == LayerMask.NameToLayer("Interact"))
                return false;

        return true;
    }
    void OnAttachmentInteract()
    {
        StaticActor actor = target as StaticActor;
        if(actor != null)
        {
            StaticActor clonePrefab = PrefabUtility.InstantiatePrefab(target) as StaticActor;
            GameObject gameObject = new GameObject("InteractCollider");
            gameObject.transform.SetParent(clonePrefab.transform);
            gameObject.transform.localPosition = Vector3.zero;

            InteractCollision col = gameObject.AddComponent<InteractCollision>();
            col.CreateCollider();
            col.Owner = clonePrefab.GetComponent<IInteractable>();

            PrefabUtility.SaveAsPrefabAssetAndConnect(clonePrefab.gameObject, PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(clonePrefab), InteractionMode.AutomatedAction);
            DestroyImmediate(clonePrefab.gameObject);
            //EditorUtility.SetDirty(target);
        }
    }
    bool OnTileSetEditor()
    {
        //if (PrefabUtility.GetPrefabType(target) != PrefabType.PrefabInstance)
        //    return false;

        if(GUILayout.Button("Set Tile block"))
        {
            if (_targetStaticActor.IsTileSetMode)
            {
                _targetStaticActor.EditorState = StaticActor.eEdtiorState.None;
                _targetStaticActor.IsTileSetMode = false;
            }
            else
            {
                _targetStaticActor.EditorState = StaticActor.eEdtiorState.TileSet;
                _targetStaticActor.IsTileSetMode = true;
            }
        }
        return false;
    }
    void OnUpdateTile()
    {
        if (_targetStaticActor == null)
            return;

        if (_targetStaticActor.EditorState != StaticActor.eEdtiorState.TileSet)
            return;

        Event e = Event.current;
        int controlID = GUIUtility.GetControlID(GetHashCode(), FocusType.Passive);
        Vector2Int tilePosition = (Vector2Int)GridManager.Instance.GetTilePosition;
        switch (e.type)
        {
            case EventType.MouseMove:
                SceneView.RepaintAll();
                _targetStaticActor.TilePosition = tilePosition;
                break;
            case EventType.MouseDown:
                switch (e.button)
                {
                    case 0:
                        // LeftDown
                        _targetStaticActor.OnClickTileBlock(tilePosition);
                        UnityEditor.EditorUtility.SetDirty(target);
                        break;
                    case 1:
                        // RightDown
                        _targetStaticActor.EditorState = StaticActor.eEdtiorState.None;
                        _targetStaticActor.IsTileSetMode = false;
                        break;
                }
                GUIUtility.hotControl = controlID;
                Event.current.Use();
                break;
        }
    }
    void SceneGUI(SceneView view)
    {
        OnUpdateTile();
    }
    private void OnDisable()
    {
        _targetStaticActor.EditorState = StaticActor.eEdtiorState.None;
        _targetStaticActor.IsTileSetMode = false;
        SceneView.duringSceneGui -= SceneGUI;
    }
}
