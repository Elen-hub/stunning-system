using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using System;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine.Tilemaps;

public class ObjectEditor : BaseStageEditor
{
    static string _objectSpawnDataPath;
    string SceneName => SceneManager.GetActiveScene().name;

    eStaticActorType _category;
    Transform _objectToolPivot;
    Transform _houseToolPivot;
    Dictionary<eStaticActorType, List<Data.ObjectData>> _objectDataDic;
    int _selectIndex = 0;
    public int SelectIndex {
        set {
            if (_selectIndex != 0)
                _runtimeEditor.SetObject(null);

            _selectIndex = value;
        }
    }
    public ObjectEditor(string sceneName) : base(sceneName)
    {
        _objectSpawnDataPath = Application.dataPath + "/Resources/Database/SceneData/";
        if (!System.IO.Directory.Exists(_objectSpawnDataPath))
            System.IO.Directory.CreateDirectory(_objectSpawnDataPath);

        if (_objectDataDic == null)
        {
            _objectDataDic = new Dictionary<eStaticActorType, List<Data.ObjectData>>();
            foreach (var data in DataManager.Instance.ObjectTable.GetDataDictionary)
            {
                if (!_objectDataDic.ContainsKey(data.Value.StaticActorType))
                    _objectDataDic.Add(data.Value.StaticActorType, new List<Data.ObjectData>());

                _objectDataDic[data.Value.StaticActorType].Add(data.Value);
            }
        }
    }
    void SpawnedObjectPivot()
    {
        GameObject scenePivot = GameObject.Find("ScenePivot");
        if(scenePivot != null)
        {
            _objectToolPivot = scenePivot.transform.Find("ObjectPivot");
            if(_objectToolPivot == null)
            {
                GameObject obj = new GameObject("ObjectPivot", typeof(Transform));
                _objectToolPivot = obj.transform;
                _objectToolPivot.SetParent(scenePivot.transform);
                _objectToolPivot.transform.localPosition = Vector3.zero;
                _objectToolPivot.transform.localEulerAngles = new Vector3(90f, 0, 0);
            }
        }
    }
    void SpawnedHousePivot()
    {
        GameObject scenePivot = GameObject.Find("ScenePivot");
        if (scenePivot != null)
        {
            _houseToolPivot = scenePivot.transform.Find("HousePivot");
            if (_houseToolPivot == null)
            {
                GameObject obj = new GameObject("HousePivot", typeof(Transform));
                _houseToolPivot = obj.transform;
                _houseToolPivot.SetParent(scenePivot.transform);
                _houseToolPivot.transform.localPosition = Vector3.zero;
                _houseToolPivot.transform.localEulerAngles = new Vector3(90f, 0, 0);
            }
        }
    }
    public override bool StartEditor()
    {
        SpawnedObjectPivot();
        SpawnedHousePivot();
        return true;
    }
    public override void OnChangedScene(Scene prev, Scene after)
    {
        _isChanged = false;
        SpawnedObjectPivot();
        SpawnedHousePivot();
    }
    public override void OnGUI()
    {
        if (GUILayout.Button("오브젝트/집 저장"))
            SaveObject();

        SceneView.lastActiveSceneView.showGrid = true;
        EditorGUILayout.BeginVertical();
        DrawElement(eStaticActorType.Normal);
        DrawElement(eStaticActorType.Plant);
        DrawElement(eStaticActorType.CookObject);
        DrawElement(eStaticActorType.Storage);
        DrawElement(eStaticActorType.Sleep);
        EditorGUILayout.EndVertical();
    }
    void SaveObject()
    {
        string scenePath = _objectSpawnDataPath + SceneName;
        if (!System.IO.Directory.Exists(scenePath))
            System.IO.Directory.CreateDirectory(scenePath);

        if(_objectToolPivot != null)
        {
            string jsonPath = $"{scenePath}/ObjectSpawnData.bytes";
            System.IO.FileStream fs = new System.IO.FileStream(jsonPath, System.IO.FileMode.Create);
            for (int i = 0; i < _objectToolPivot.childCount; ++i)
            {
                StaticActor staticActor = _objectToolPivot.GetChild(i).GetComponent<StaticActor>();
                if (staticActor != null)
                {
                    byte[] byteArr = staticActor.GetSaveByte();
                    fs.Write(byteArr, 0, byteArr.Length);
                }
            }
            fs.Close();
        }
        if (_houseToolPivot != null)
        {
            string jsonPath = $"{scenePath}/HouseSpawnData.bytes";
            System.IO.FileStream fs = new System.IO.FileStream(jsonPath, System.IO.FileMode.Create);
            for (int i = 0; i < _houseToolPivot.childCount; ++i)
            {
                House house = _houseToolPivot.GetChild(i).GetComponent<House>();
                if (house != null)
                {
                    byte[]  byteArr = BitConverter.GetBytes(house.Index);
                    fs.Write(byteArr, 0, byteArr.Length);
                    byteArr = BitConverter.GetBytes(house.transform.position.x);
                    fs.Write(byteArr, 0, byteArr.Length);
                    byteArr = BitConverter.GetBytes(house.transform.position.y);
                    fs.Write(byteArr, 0, byteArr.Length);
                }
            }
            fs.Close();
        }
        AssetDatabase.Refresh();
    }
    void DrawElement(eStaticActorType category)
    {
        bool isFolder = EditorGUILayout.BeginFoldoutHeaderGroup(_category == category, $"Category:: {category}");
        if(isFolder)
        {
            _category = category;
            foreach (var element in _objectDataDic[category])
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                string title = element.Index == _selectIndex ? $"<color=red>Index: {element.Index} {element.Name}</color>" : $"Index: {element.Index} {element.Name}";
                if (GUILayout.Toggle(element.Index == _selectIndex, title))
                {
                    if (element.Index != _selectIndex)
                    {
                        SelectIndex = element.Index;
                        _runtimeEditor.SetObject(element);
                    }
                    if (_runtimeEditor.CurrentSpawnIndex == 0) 
                        _selectIndex = 0;
                }
                GUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
    public override void Destroy()
    {
        _selectIndex = 0;
        GameObject.DestroyImmediate(ResourceManager.Instance.gameObject);
    }
}
