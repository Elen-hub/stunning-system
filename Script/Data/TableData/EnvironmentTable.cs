using System.Collections.Generic;
public class EnvironmentTable : TableBase
{
    Dictionary<int, Data.EnvironmentData> _dataDictionary = new Dictionary<int, Data.EnvironmentData>();
    public Data.EnvironmentData this[int index] {
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
            Data.EnvironmentData data = new Data.EnvironmentData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
