using System.Collections.Generic;

namespace Data
{
    public class RecipeData
    {
        public readonly int Index;
        public readonly int Count;
        public readonly float CraftTime;
        public readonly Dictionary<int, int> MaterialDictionary;
        public RecipeData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            Count = int.Parse(dataPair["Count"]);
            CraftTime = float.Parse(dataPair["CraftTime"]);
            MaterialDictionary = new Dictionary<int, int>();
            int count = 0;
            string materialColumn = "Material0";
            while (dataPair.ContainsKey(materialColumn))
            {
                if (string.IsNullOrEmpty(dataPair[materialColumn]))
                    break;

                int[] pairs = System.Array.ConvertAll(dataPair[materialColumn].Split('-'), v => int.Parse(v));
                MaterialDictionary.Add(pairs[0], pairs[1]);
                materialColumn = materialColumn.Replace(count.ToString(), (++count).ToString());
            }
        }
    }
}