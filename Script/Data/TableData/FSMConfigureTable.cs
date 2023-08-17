using System.Collections.Generic;
public class FSMConfigureTable : TableBase
{
    Dictionary<int, Data.FSMConfigureData> _dataDictionary = new Dictionary<int, Data.FSMConfigureData>();
    public Data.FSMConfigureData this[int index] {
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
            Data.FSMConfigureData data = new Data.FSMConfigureData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
