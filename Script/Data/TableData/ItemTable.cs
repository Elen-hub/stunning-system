using System.Collections.Generic;
public class ItemTable : TableBase
{
    Dictionary<int, Data.ItemData> _dataDictionary = new Dictionary<int, Data.ItemData>();
    public Dictionary<int, Data.ItemData> GetDataDictionary => _dataDictionary;
    public Data.ItemData this[int index] {
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
            Data.ItemData data = new Data.ItemData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
