using System.Collections.Generic;

namespace Data
{
    public class StatusWeightData
    {
        public readonly eStatusType StatType;
        public readonly float Value;
        public StatusWeightData(eStatusType type, float value)
        {
            StatType = type;
            Value = value;
        }
    }
}