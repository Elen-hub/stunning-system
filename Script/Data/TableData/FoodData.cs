using System.Collections.Generic;

namespace Data
{
    public class FoodData
    {
        public readonly int Index;
        public FoodData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
        }
    }
}