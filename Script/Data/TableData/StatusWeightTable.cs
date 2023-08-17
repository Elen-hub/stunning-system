using System.Collections.Generic;
using Data;

public class StatusWeightTable : TableBase
{
    Dictionary<eStatusType, StatusWeightData[]> _statusWeightDic;
    public StatusWeightData[] this[eStatusType type] {
        get {
            if (_statusWeightDic.ContainsKey(type))
                return _statusWeightDic[type];

            return null;
        }
    }
    protected override void OnLoad()
    {
        const string element = "VariableWeight";
        LoadData(_tableName);
        StatusCalculator.Init(_dataDic);
        _statusWeightDic = new Dictionary<eStatusType, StatusWeightData[]>();
        foreach (var contents in _dataDic)
        {
            List<StatusWeightData> statusWeightList = new List<StatusWeightData>();
            int elementCount = 0;
            while(true)
            {
                if (!contents.Value.ContainsKey(element + elementCount) || string.IsNullOrEmpty(contents.Value[element + elementCount]))
                    break;

                string[] pairs = contents.Value[element + elementCount].Split('-');
                // UnityEngine.Debug.Log($"  {pairs[0]} {pairs[1]}");
                eStatusType type = EnumConverter.StatusTypeConvert[pairs[0]];
                float value = float.Parse(pairs[1]);
                statusWeightList.Add(new StatusWeightData(type, value));
                ++elementCount;
            }
            _statusWeightDic.Add(EnumConverter.StatusTypeConvert[contents.Value["StatusName"]], statusWeightList.ToArray());
        }
    }
}