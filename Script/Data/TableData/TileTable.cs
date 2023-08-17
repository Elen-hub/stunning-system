using System.Collections.Generic;

public class TileTable : TableBase
{
    Dictionary<int, Data.TileData> _dataDictionary = new Dictionary<int, Data.TileData>();
    public Data.TileData this[int index] {
        get {
            if (_dataDictionary.ContainsKey(index))
                return _dataDictionary[index];

            return null;
        }
    }
#if UNITY_EDITOR
    Dictionary<string, int> _stringToKeyDictionary = new Dictionary<string, int>();
    public Data.TileData this[string name] {
        get {
            if (_stringToKeyDictionary.ContainsKey(name))
                return this[_stringToKeyDictionary[name]];

            return null;
        }
    }
#endif
    protected override void OnLoad()
    {
        LoadData(_tableName);
        foreach (var contents in _dataDic)
        {
            Data.TileData data = new Data.TileData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
#if UNITY_EDITOR
            _stringToKeyDictionary.Add(data.Tile.name, data.Index);
#endif
        }
        _dataDic = null;
    }
}
