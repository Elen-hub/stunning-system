using System.Collections.Generic;
public class ObjectTable : TableBase
{
    Dictionary<int, Data.ObjectData> _dataDictionary = new Dictionary<int, Data.ObjectData>();
    public Dictionary<int, Data.ObjectData> GetDataDictionary => _dataDictionary;
    public Data.ObjectData this[int index] {
        get {
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
            Data.ObjectData data = new Data.ObjectData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
