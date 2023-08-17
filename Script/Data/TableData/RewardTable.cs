using System.Collections.Generic;
public class RewardTable : TableBase
{
    Dictionary<int, Data.RewardData> _dataDictionary = new Dictionary<int, Data.RewardData>();
    public Dictionary<int, Data.RewardData> GetDataDictionary => _dataDictionary;
    public Data.RewardData this[int index] {
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
            Data.RewardData data = new Data.RewardData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
