using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffTable : TableBase
{
    Dictionary<int, Data.BuffData> _dataDictionary = new Dictionary<int, Data.BuffData>();
    public Data.BuffData this[int index] {
        get
        {
            if (_dataDictionary.ContainsKey(index))
                return _dataDictionary[index];

            return null;
        }
    }
    protected override void OnLoad()
    {
        LoadData(_tableName);
        foreach (var contents in _dataDic)
        {
            Data.BuffData data = new Data.BuffData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
