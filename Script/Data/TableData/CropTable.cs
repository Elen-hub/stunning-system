using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropTable : TableBase
{
    Dictionary<int, Data.CropData> _dataDictionary = new Dictionary<int, Data.CropData>();
    public Data.CropData this[int index] {
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
            Data.CropData data = new Data.CropData(contents.Key, contents.Value); 
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
