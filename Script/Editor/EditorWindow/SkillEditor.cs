using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Newtonsoft.Json;
public class SkillEditor : EditorWindow
{
    public static Dictionary<string, TextAsset> JsonDic = new Dictionary<string, TextAsset>();
    float _width = 380f;
    float _height = 0;
    int _xmlIndex = 0;
    int _selectIndex = 0;
    Data.SkillData _skillData;
    static SkillEditor _window;
    static BaseSkill _skill;
    static string _jsonName;

    static GUIStyle _style;

    static float _scrollbarValue;

    [MenuItem("Tools/SkillEditor")]
    public static void Window()
    {
        if (_window == null)
        {
            _style = new GUIStyle();
            _style.richText = true;
            _window = CreateInstance<SkillEditor>();
            _window.titleContent = new GUIContent("Skill Json Editor");
            _window.minSize = new Vector2(400f, 200f);
            _window.maxSize = new Vector2(400f, 600f);
            EnumConverter.Init();
            DataManager.Instance.Initialize();
            SkillTable table = DataManager.Instance.SkillTable; 
            OnLoadJson();
        }
        _window.Show();
    }
    static void OnLoadJson()
    {
        JsonDic.Clear();
        TextAsset[] textAssetList = Resources.LoadAll<TextAsset>("Database/Json/SkillData");
        for (int i = 0; i < textAssetList.Length; ++i)
            JsonDic.Add(textAssetList[i].name, textAssetList[i]);
    }
    void OnIndex()
    {
        GUI.Box(new Rect(0, _height, _width, 20f), "", EditorStyles.toolbar);
        // GUI.Box(new Rect(0, 15, 0, 15), "", EditorStyles.toolbar);
        float width = 0f;
        EditorGUI.LabelField(new Rect(width, _height, 100f, 20f), "XML Index"); 
        width += 100f;
        _selectIndex = EditorGUI.IntField(new Rect(width, _height, 135f, 20f), _selectIndex); 
        width += 200f;
        if (GUI.Button(new Rect(width, _height, _width - width, 20f), "Find"))
        {
            _jsonName = null; 
            _xmlIndex = _selectIndex;
            _skillData = DataManager.Instance.SkillTable[_xmlIndex];
            _skill = null;
        }
        _height += 20f;
    }
    void OnGUIStart()
    {
        _height = -_scrollbarValue;
    }
    void ShowBackGround()
    {
        GUILayout.Width(_width);
        GUILayout.Height(500f);
        OnIndex();
    }
    void OnReadXmlAttribute()
    {
        _height += 20f; 
        if (JsonDic.ContainsKey(_skillData.SerializeJson))
        {
            _skill = DataManager.Instance.SkillTable[_xmlIndex].Skill;
            _jsonName = _skillData.SerializeJson;
            EditorGUI.LabelField(new Rect(0f, _height, _width, 20f), "JsonName: " + _skillData.SerializeJson);
        }
        else if (GUI.Button(new Rect(0f, _height, _width, 20f), "Create Json")) 
        {
            System.Reflection.Assembly currAssembly = System.Reflection.Assembly.Load("Assembly-CSharp");
            _skill = BaseSkill.CreateNewSkillOnlyEditor(currAssembly, _xmlIndex);
            _skill.SetDefaultData();
            _jsonName = EditorGUI.TextField(new Rect(0f, _height, _width, 20f), _jsonName);
        }
        _height += 20f;
    }
    void OnEditProperty(ref List<SkillEditorException> errorList)
    {
        _height += 10f;

        EditorGUI.LabelField(new Rect(0f, _height, _width * 1 / 3f, 20f), "JsonName: ");
        _jsonName = EditorGUI.TextField(new Rect(_width * 1 / 3f, _height, _width * 2 / 3f, 20f), _jsonName);
        _height += 30f;
        _skill.OnGUIEditor(_width, ref _height, ref errorList);
    }
    bool OnErrorMessageProcess(ref List<SkillEditorException> errorList)
    {
        bool isPossibleSave = true;
        if (string.IsNullOrEmpty(_jsonName))
        {
            errorList.Add(new SkillEditorException("", SkillEditorException.eErrorLevel.Error, "JsonName이 입력되지 않았습니다"));
            isPossibleSave = false;
        }
        for (int i = 0; i < errorList.Count; ++i)
        {
            if (errorList[i].Error == SkillEditorException.eErrorLevel.Error)
                isPossibleSave = false;

            string colorCode = errorList[i].Error == SkillEditorException.eErrorLevel.Warrning ? "<color=yellow> [경고] " : "<color=red> [오류] ";
            EditorGUI.LabelField(new Rect(0f, _height, _width, 20f), colorCode + errorList[i].AttributeName + " : " + errorList[i].ErrorMessage + "</color>", _style);
            _height += 20f;
        }

        return isPossibleSave;
    }
    void OnSave(bool isForce)
    {
        string skillDataPath = Application.dataPath + "/Resources/Database/Json/SkillData/";
        if (!System.IO.Directory.Exists(skillDataPath))
            System.IO.Directory.CreateDirectory(skillDataPath);

        if (!isForce && 
            System.IO.File.Exists(skillDataPath + _jsonName + ".json"))
        {
            EditorGUI.LabelField(new Rect(0f, _height, _width, 20f), "<color=red> 중복되는 이름이 존재하여 덮어씁니다.</color>", _style);
            _height += 20f;
        }
        if (GUI.Button(new Rect(20f, _height, _width - 20f, 20f), "저장"))
        {
            System.IO.FileStream fs = new System.IO.FileStream(skillDataPath + _jsonName + ".json", System.IO.FileMode.Create);
            byte[] byteArr = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_skill, Formatting.Indented));
            fs.Write(byteArr, 0, byteArr.Length);
            fs.Close();
            _window.Close();
        }
    }
    private void OnGUI()
    {
        List<SkillEditorException> errorList = new List<SkillEditorException>();
        OnGUIStart();
        ShowBackGround();
        if (_skillData != null && DataManager.Instance.SkillTable[_xmlIndex] != null)
        {
            if (_skill == null)
                OnReadXmlAttribute();
            else
                OnEditProperty(ref errorList);
        }
        _height += 15f;
        if (OnErrorMessageProcess(ref errorList))
            OnSave(false);

        if (_window == null)
        {
            Window();
            return;
        }
        _scrollbarValue = GUI.VerticalScrollbar(new Rect(_width, 0, 20, _window.position.height), _scrollbarValue, 0, 0, _height + 50);
    }
}
