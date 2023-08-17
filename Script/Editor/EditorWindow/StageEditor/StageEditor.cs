using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;

public class StageEditor : EditorWindow
{
    static Scene _currentScene;
    static StageEditor _window;
    public static GUIStyle ButtonStyle;
    public static GUIStyle LabelStyle;

    public static eStageEditorType StageEditorType;
    static Dictionary<eStageEditorType, BaseStageEditor> _stageEditorDic;

    static TileSaveEditor _tileSaveEditor;
    public static StageRuntimeEditor RuntimeEditor;

    [MenuItem("Tools/StageEditor")]
    public static void Window()
    {
        if (GameObject.Find("ScenePivot") == null)
            return;

        if(_window == null)
        {
            StageEditorType = eStageEditorType.None;
            EnumConverter.Init();
            _window = CreateInstance<StageEditor>();
            _window.titleContent = new GUIContent("Stage Editor");
            _window.minSize = new Vector2(400f, 200f);
            _window.maxSize = new Vector2(400f, 600f);
            _currentScene = SceneManager.GetActiveScene();
            _tileSaveEditor = new TileSaveEditor();

            _stageEditorDic = new Dictionary<eStageEditorType, BaseStageEditor>();
            _stageEditorDic.Add(eStageEditorType.MonsterEditor, new MonsterEditor(_currentScene.name));
            _stageEditorDic.Add(eStageEditorType.ObjectEditor, new ObjectEditor(_currentScene.name));
            if (RuntimeEditor == null)
                SpawnRuntimeEditor();
        }
        if (_window != null)
            _window.Show();
    }
    private void OnEnable()
    {
        EditorSceneManager.activeSceneChangedInEditMode -= OnChangedScene;
        EditorSceneManager.activeSceneChangedInEditMode += OnChangedScene;
    }
    private void OnDisable()
    {
        EditorSceneManager.activeSceneChangedInEditMode -= OnChangedScene;
    }
    public void OnGUI()
    {
        GUI.skin.button.richText = true;
        GUI.skin.label.richText = true;
        GUI.skin.toggle.richText = true;

        EditorGUILayout.BeginVertical(); 
        _tileSaveEditor.OnGUI();

        EditorGUILayout.Space(2.5f);

        if (GUILayout.Button("πÃ¥œ∏  ¿˙¿Â"))
            OnExportMinimapBlock();
        
        EditorGUILayout.Space(5f);
        eStageEditorType type = (eStageEditorType)EditorGUILayout.EnumPopup(StageEditorType);
        if(StageEditorType != type)
        {
            if(type != eStageEditorType.None)
            {
                if (_stageEditorDic[type].StartEditor()) 
                {
                    StageEditorType = type;
                    RuntimeEditor.StageEditorType = type;
                }
            }
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space(10);
        if (StageEditorType != eStageEditorType.None)
            _stageEditorDic[StageEditorType].OnGUI();
    }
    public void OnChangedScene(Scene prev, Scene after)
    {
        if (prev != after)
        {
            if(_window == null)
            {
                return;
            }

            if (GameObject.Find("ScenePivot") == null)
            {
                _window.Close();
                return;
            }

            if (RuntimeEditor == null)
                SpawnRuntimeEditor();  

            _currentScene = after;
            foreach(var editor in _stageEditorDic) 
                editor.Value.OnChangedScene(prev, after);
        }
    }
    void OnExportMinimapBlock()
    {
        GameObject scenePivot = GameObject.Find("ScenePivot"); 
        if (scenePivot == null)
            return;

        string path = Application.dataPath + "/Resources/Database/SceneData/" + SceneManager.GetActiveScene().name;
        if (!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        ExportMapImageEditor.Export(scenePivot.transform.position, path + "/MinimapBlock.png");
    }
    static void SpawnRuntimeEditor()
    {
        GameObject scenePivot = GameObject.Find("ScenePivot");
        GameObject obj = new GameObject("RuntimeEditor", typeof(StageRuntimeEditor));
        RuntimeEditor = obj.GetComponent<StageRuntimeEditor>();
        RuntimeEditor.Initialize();
        obj.transform.position = scenePivot.transform.position;
    }
    private void OnDestroy()
    {
        EditorSceneManager.activeSceneChangedInEditMode -= OnChangedScene;
        foreach (var editor in _stageEditorDic)
            editor.Value.Destroy();

        _stageEditorDic.Clear();
        if (RuntimeEditor != null)
            DestroyImmediate(RuntimeEditor.gameObject);
    }
}