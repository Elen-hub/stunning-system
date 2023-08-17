using System.Collections.Generic;

namespace Data
{
    public class StatData
    {
        public readonly int Index;
        public readonly string ResourcePath;
        readonly Dictionary<eStatusType, float> _statusDic;
        public Dictionary<eStatusType, StatusFloat> GenerateStat()
        {
            Dictionary<eStatusType, StatusFloat> status = new Dictionary<eStatusType, StatusFloat>(_statusDic.Count);
            foreach (var value in _statusDic)
                status.Add(value.Key, new StatusFloat(value.Value));

            return status;
        }
        public StatData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            _statusDic = new Dictionary<eStatusType, float>();
            foreach (var contents in dataPair)
                if (EnumConverter.StatusTypeConvert.ContainsKey(contents.Key))
                {
                    float parseValue = 0f;
                    float.TryParse(contents.Value, out parseValue);
                    _statusDic.Add(EnumConverter.StatusTypeConvert[contents.Key], parseValue);
                }
        }
    }
}