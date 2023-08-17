using System.Collections.Generic;
public class PatternTable : TableBase
{
    Dictionary<int, Data.PatternData> _patternDic = new Dictionary<int, Data.PatternData>();
    public Data.PatternData this[int index] {
        get {
            if (_patternDic.ContainsKey(index))
                return _patternDic[index];

            return null;
        }
    }
    protected override void OnLoad()
    {
        LoadData(_tableName);
        foreach (var contents in _dataDic)
        {
            int index = int.Parse(contents.Value["Index"]);
            Data.PatternData data = new Data.PatternData(index, contents.Value);
            _patternDic.Add(index, data);
        }
    }
}
