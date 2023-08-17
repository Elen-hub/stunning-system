using System.Collections.Generic;

public class ItemStatTable : TableBase
{
    Dictionary<int, Data.ItemStatData> _dataDictionary = new Dictionary<int, Data.ItemStatData>();
    public Data.ItemStatData this[int index] {
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
            Data.ItemStatData data = new Data.ItemStatData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
