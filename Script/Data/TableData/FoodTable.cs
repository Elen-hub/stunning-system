using System.Collections.Generic;
public class FoodTable : TableBase
{
    Dictionary<int, Data.FoodData> _dataDictionary = new Dictionary<int, Data.FoodData>();
    public Data.FoodData this[int index] {
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
            Data.FoodData data = new Data.FoodData(contents.Key, contents.Value);
            _dataDictionary.Add(contents.Key, data);
        }
        _dataDic = null;
    }
}
