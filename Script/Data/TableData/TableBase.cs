using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TableBase
{
    protected string _tableName;
    public string SetTableName { set => _tableName = value; }
    protected Dictionary<int, Dictionary<string, string>> _dataDic;
    public void Reload()
    {
        OnLoad();
    }
    protected abstract void OnLoad();
    protected void LoadData(string name)
    {
        TextAsset textAsset =  Resources.Load<TextAsset>("Database/Table/" + name);
        string[] arr = textAsset.ToString().Split('\r');
        _dataDic = new Dictionary<int, Dictionary<string, string>>(arr.Length - 1);
        string[] keys = arr[0].Split(',');

        for (int i = 1; i < arr.Length; ++i)
        {
            string[] values = arr[i].Replace("\n", "").Split(',');
            int idx = int.Parse(values[0]);
            _dataDic.Add(idx, new Dictionary<string, string>());
            for (int j = 0; j < keys.Length; ++j)
                _dataDic[idx].Add(keys[j], values[j]);
        }
    }
}
