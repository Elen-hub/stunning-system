using System.Collections.Generic;

public class CharacterTable : TableBase
{
    Dictionary<int, Data.CharacterData> _dataDictionary = new Dictionary<int, Data.CharacterData>();
    public Data.CharacterData this[int index] {
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
            Data.CharacterData data = new Data.CharacterData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
