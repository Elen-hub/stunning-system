using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class SkillTable : TableBase
{
    Dictionary<int, Data.SkillData> _skillDic = new Dictionary<int, Data.SkillData>();
    public Data.SkillData this[int index] {
        get {
            if (_skillDic.ContainsKey(index))
                return _skillDic[index];

            return null;
        }
    }
    protected override void OnLoad()
    {
        LoadData(_tableName);
        System.Reflection.Assembly assembly = System.Reflection.Assembly.Load("Assembly-CSharp");
        Dictionary<string, string> jsonDataDic = new Dictionary<string, string>();
        TextAsset[] textAsset = Resources.LoadAll<TextAsset>(ClientConst.SkillDataJsonPath);
        for (int i = 0; i < textAsset.Length; ++i)
            jsonDataDic.Add(textAsset[i].name, textAsset[i].text);

        Dictionary<string, System.Type> typeDic = new Dictionary<string, System.Type>();
        foreach (var contents in _dataDic)
        {
            int index = int.Parse(contents.Value["Index"]);

            Data.SkillData data = new Data.SkillData(contents.Value);
            if(jsonDataDic.ContainsKey(data.SerializeJson))
            {
                BaseSkill skill = CreateNewSkill(assembly, typeDic, data.ClassAssembly, jsonDataDic[data.SerializeJson]);
                skill.Index = index;
                data.Skill = skill;
            }
            _skillDic.Add(index, data);
        }
    }
    BaseSkill CreateNewSkill(System.Reflection.Assembly assembly, Dictionary<string, System.Type> typeDic, string className, string jsonText)
    {
        if (!typeDic.ContainsKey(className)) 
        {
            System.Type type = assembly.GetType(className);
            typeDic.Add(className, type);
        }
        object skill = JsonConvert.DeserializeObject(jsonText, typeDic[className]); 
        return (BaseSkill)skill;
    }
}
