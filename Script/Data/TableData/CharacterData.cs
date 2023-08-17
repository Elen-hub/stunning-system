using System.Collections.Generic;

namespace Data
{
    public class CharacterData
    {
        public readonly int Index;
        public readonly string ResourcePath;
        public readonly int StatTableIndex;
        public CharacterData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            ResourcePath = dataPair["ResourcePath"];
            StatTableIndex = int.Parse(dataPair["StatTableIndex"]);
        }
    }
}