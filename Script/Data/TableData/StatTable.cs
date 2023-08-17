using System.Collections.Generic;

public class StatTable : TableBase
{
    Dictionary<int, Data.StatData> _dataDictionary = new Dictionary<int, Data.StatData>();
    public Data.StatData this[int index] {
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
            Data.StatData data = new Data.StatData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
