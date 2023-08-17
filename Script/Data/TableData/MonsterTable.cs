using System.Collections.Generic;
public class MonsterTable : TableBase
{
    Dictionary<int, Data.MonsterData> _dataDictionary = new Dictionary<int, Data.MonsterData>();
    public Data.MonsterData this[int index] {
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
            Data.MonsterData data = new Data.MonsterData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
