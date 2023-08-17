using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class FSMConfigureData
    {
        public readonly int Index;
        public readonly int[] PatternKeys;
        public readonly float DetectSensorRadius;
        public readonly float ChaseRadius;
        public readonly float LimitRadius;
        public readonly float LeastDistance;
        public FSMConfigureData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            DetectSensorRadius = float.Parse(dataPair["DetectSensorRadius"]);
            ChaseRadius = float.Parse(dataPair["ChaseRadius"]);
            LimitRadius = float.Parse(dataPair["LimitRadius"]);
            if (string.IsNullOrEmpty(dataPair["PatternKeys"]) == false)
            {
                string[] splitArr = dataPair["PatternKeys"].Split('|');
                PatternKeys = new int[splitArr.Length];
                for (int i = 0; i < splitArr.Length; ++i)
                    PatternKeys[i] = int.Parse(splitArr[i]);

                if (PatternKeys[0] == 0)
                    PatternKeys = null;
            }
            LeastDistance = float.Parse(dataPair["LeastDistance"]);
        }
    }
}