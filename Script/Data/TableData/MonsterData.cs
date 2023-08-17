using System.Collections.Generic;

namespace Data
{
    public class MonsterData
    {
        public readonly int Index;
        public readonly string ResourcePath;
        public readonly int NameKey;
        public readonly int StatTableIndex;
        public readonly int RewardIndex;
        public readonly int FSMConfigureIndex;
        public readonly int Exp;
        public MonsterData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            ResourcePath = dataPair["ResourcePath"];
            NameKey = int.Parse(dataPair["NameKey"]);
            StatTableIndex = int.Parse(dataPair["StatTableIndex"]);
            FSMConfigureIndex = int.Parse(dataPair["FSMConfigureIndex"]);
            Exp = int.Parse(dataPair["Exp"]);
            if (!string.IsNullOrEmpty(dataPair["RewardIndex"]))
                RewardIndex = int.Parse(dataPair["RewardIndex"]);
        }
    }
}