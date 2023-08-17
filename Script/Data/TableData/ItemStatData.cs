using System.Collections.Generic;

namespace Data
{
    public class ItemStatData
    {
        public readonly int Index;
        public Dictionary<eStatusType, float> Stat;
        public Dictionary<eStatusType, StatusFloat> GenerateStat()
        {
            Dictionary<eStatusType, StatusFloat> status = new Dictionary<eStatusType, StatusFloat>(Stat.Count);
            foreach (var value in Stat)
                status.Add(value.Key, new StatusFloat(value.Value));

            return status;
        }
        public ItemStatData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            Stat = new Dictionary<eStatusType, float>();
            foreach (var contents in dataPair)
                if (EnumConverter.StatusTypeConvert.ContainsKey(contents.Key))
                {
                    float parseValue = 0f;
                    float.TryParse(contents.Value, out parseValue);
                    Stat.Add(EnumConverter.StatusTypeConvert[contents.Key], parseValue);
                }
        }
    }
}