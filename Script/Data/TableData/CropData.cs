using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class CropData
    {
        public readonly int Index;
        public readonly int Level;
        public readonly int GrowthObjectIndex;
        public readonly float GrowthTime;
        public CropData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            Level = int.Parse(dataPair["Level"]);
            GrowthObjectIndex = int.Parse(dataPair["GrowthObjectIndex"]);
            GrowthTime = float.Parse(dataPair["GrowthTime"]);
        }
    }
}