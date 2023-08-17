using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Data
{
    public class EffectData
    {
        public readonly int Index;
        public readonly string ResourcePath;
        public EffectData(Dictionary<string, string> dataPair)
        {
            Index = int.Parse(dataPair["Index"]);
            ResourcePath = dataPair["ResourcePath"];
        }
    }
}