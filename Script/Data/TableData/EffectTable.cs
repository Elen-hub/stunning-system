using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectTable : TableBase
{
    Dictionary<int, Data.EffectData> _effectDataDic = new Dictionary<int, Data.EffectData>();
    Dictionary<string, Data.EffectData> _effectDataNameKeyDic = new Dictionary<string, Data.EffectData>();
    public Dictionary<string, Data.EffectData> GetNameKeyDatas => _effectDataNameKeyDic;
    public Data.EffectData this[int index] {
        get {
            if (_effectDataDic.ContainsKey(index))
                return _effectDataDic[index];

            return null;
        }
    }
    public Data.EffectData this[string key]
    {
        get
        {
            if (_effectDataNameKeyDic.ContainsKey(key))
                return _effectDataNameKeyDic[key];

            return null;
        }
    }
    protected override void OnLoad()
    {
        LoadData(_tableName);
        foreach (var contents in _dataDic)
        {
            Data.EffectData data = new Data.EffectData(contents.Value);
            _effectDataDic.Add(contents.Key, data);
            if (!string.IsNullOrEmpty(contents.Value["Key"]))
                _effectDataNameKeyDic.Add(contents.Value["Key"], data);
        }
    }
}
