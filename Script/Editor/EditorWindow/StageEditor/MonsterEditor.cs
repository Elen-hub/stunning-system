using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;

public class MonsterEditor : BaseStageEditor
{
    static HashSet<int> _areaHash = new HashSet<int>();
    static SpawnHandler _spawnHandler;
    static string _spawnHandlerPath;
    string SceneName => SceneManager.GetActiveScene().name;

    static int _selectNumber = -1;
    public MonsterEditor(string sceneName) : base(sceneName)
    {
        _spawnHandlerPath = Application.dataPath + "/Resources/Database/SceneData/";
        if (!System.IO.Directory.Exists(_spawnHandlerPath))
            System.IO.Directory.CreateDirectory(_spawnHandlerPath);
    }
    public override bool StartEditor()
    {
        if (!LoadHandler())
            return false;

        _runtimeEditor.SetSpawnHandler(_spawnHandler);
        return true;
    }
    public override void OnChangedScene(Scene prev, Scene after)
    {
        _runtimeEditor.SetSpawnHandler(_spawnHandler);
        if (_isChanged)
            if (EditorUtility.DisplayDialog("Scene change was detected.", "Changed prevoius data exists. Are you sure you want to overwrite?", "Overwrite", "No"))
                SaveHandler();

        _isChanged = false;
        _spawnHandler = null;
        _areaHash.Clear();

        if (_spawnHandler == null)
            LoadHandler();
    }
    bool LoadHandler()
    {
        _spawnHandler = null;
        string jsonPath = $"{_spawnHandlerPath}{_sceneName}/MonsterSpawnData.json";
        if (File.Exists(jsonPath))
        {
            StreamReader jsonFile = File.OpenText(jsonPath);
            _spawnHandler = JsonConvert.DeserializeObject<SpawnHandler>(jsonFile.ReadToEnd(), ClientConst.SerializingSetting);
            for (int i = 0; i < _spawnHandler.SpawnDataList.Count; ++i)
                _areaHash.Add(_spawnHandler.SpawnDataList[i].AreaNumber);
        }
        else
        {
            if (EditorUtility.DisplayDialog("Data does not exist in this scene.", "Please select create or exit editor.", "Create", "Exit"))
            {
                _spawnHandler = new SpawnHandler();
                _spawnHandler.Offset = _runtimeEditor.Offset;
            }
            else
                return false;
        }
        return true;
    }
    void SaveHandler()
    {
        string scenePath = _spawnHandlerPath + SceneName;
        if (!System.IO.Directory.Exists(scenePath))
            System.IO.Directory.CreateDirectory(scenePath);

        string jsonPath = $"{scenePath}/MonsterSpawnData.json";
        System.IO.FileStream fs = new System.IO.FileStream(jsonPath, System.IO.FileMode.Create);
        byte[] byteArr = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_spawnHandler, Formatting.Indented, ClientConst.SerializingSetting));
        fs.Write(byteArr, 0, byteArr.Length);
        fs.Close();
        AssetDatabase.Refresh();
    }
    public override void OnGUI()
    {
        if (_spawnHandler == null)
            return;

        EditorGUILayout.BeginVertical();
        if (_isChanged) DrawSave();
        DrawTitle();
        DrawCreateAreaButton();
        EditorGUI.BeginChangeCheck();
        for (int i = 0; i < _spawnHandler.SpawnDataList.Count; ++i)
            DrawElement(_spawnHandler.SpawnDataList[i]);

        if (EditorGUI.EndChangeCheck())
            _isChanged = true;

        EditorGUILayout.EndVertical();
    }
    void DrawSave()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.richText = true;
        if (GUILayout.Button("<B>Save Data</B>", style))
        {
            if (EditorUtility.DisplayDialog("[MonsterEditor] Scene change was detected.", "Changed prevoius data exists. Are you sure you want to overwrite?", "Overwrite", "No"))
                SaveHandler();
        }
    }
    void DrawCreateAreaButton()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        style.richText = true;
        if (GUILayout.Button("<B>Create Area</B>", style))
        {
            for (int i = 0; i < _spawnHandler.SpawnDataList.Count; ++i)
            {
                if (!_areaHash.Contains(i))
                {
                    _areaHash.Add(i);
                    _spawnHandler.SpawnDataList.Add(new SpawnData() { AreaNumber = i, });
                    return;
                }
            }
            _areaHash.Add(_spawnHandler.SpawnDataList.Count);
            _spawnHandler.SpawnDataList.Add(new SpawnData() { AreaNumber = _spawnHandler.SpawnDataList.Count, });
        }
    }
    void DrawTitle()
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.fontSize = 15;
        style.richText = true;
        EditorGUILayout.LabelField($"<B>SelectScene: {_sceneName}</B>", style);
    }
    void DrawElement(SpawnData data)
    {
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("X", GUILayout.Width(20f)))
        {
            _spawnHandler.SpawnDataList.Remove(data);

            if(_selectNumber == data.AreaNumber)
                _selectNumber = -1;
            EditorGUILayout.EndHorizontal();
            return;
        }
        bool isFolder = EditorGUILayout.BeginFoldoutHeaderGroup(_selectNumber == data.AreaNumber, $"AreaNum: {data.AreaNumber} Monster: {data.MonsterIndex} Position: {data.SpawnRect.position}");
        EditorGUILayout.EndHorizontal();
        if (isFolder)
        {
            _selectNumber = data.AreaNumber;
            _runtimeEditor.Data = data;
            data.SpawnRect = EditorGUILayout.RectField("SpawnRect", data.SpawnRect);
            data.MonsterIndex = EditorGUILayout.IntField("MonsterIndex: ", data.MonsterIndex);
            data.SpawnTick = EditorGUILayout.FloatField("SpawnTick: ", data.SpawnTick);
            data.SpawnCount = EditorGUILayout.IntField("SpawnCount: ", data.SpawnCount);
            data.MaxCount = EditorGUILayout.IntField("MaxCount: ", data.MaxCount);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    public override void Destroy()
    {

    }
}
