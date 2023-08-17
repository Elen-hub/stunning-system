using System;
using System.Collections.Generic;

namespace Data
{
    public class RewardData
    {
        public readonly int Index;
        public readonly string Name;
        public readonly float DropPeriodHP;
        public readonly DropTableStructure[] DropPeriodStructures;
        public readonly DropTableStructure[] DropStructures;
        public RewardData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
#if UNITY_EDITOR
            Name = dataPair["Name"];
#endif
            DropPeriodHP = float.Parse(dataPair["RewardPeriodHP"]);
            if (DropPeriodHP > 0)
            {
                if (!string.IsNullOrEmpty("RewardPeriodItemIndex"))
                {
                    int[] dropPeriodItemIndexArr = Array.ConvertAll(dataPair["RewardPeriodItemIndex"].Split('|'), v => int.Parse(v));
                    int[] dropPeriodItemCountArr = Array.ConvertAll(dataPair["RewardPeriodItemCount"].Split('|'), v => int.Parse(v));
                    float[] dropPeriodItemPercentage = Array.ConvertAll(dataPair["RewardPeriodItemPercentage"].Split('|'), v => float.Parse(v));
                    DropPeriodStructures = new DropTableStructure[dropPeriodItemCountArr.Length];
                    for (int i = 0; i < dropPeriodItemCountArr.Length; ++i)
                        DropPeriodStructures[i] = new DropTableStructure(dropPeriodItemIndexArr[i], dropPeriodItemCountArr[i], dropPeriodItemPercentage[i]);
                }
            }
            if (!string.IsNullOrEmpty("RewardItemIndex"))
            {
                int[] dropItemIndexArr = Array.ConvertAll(dataPair["RewardItemIndex"].Split('|'), v => int.Parse(v));
                int[] dropItemCountArr = Array.ConvertAll(dataPair["RewardItemCount"].Split('|'), v => int.Parse(v));
                float[] dropItemPercentage = Array.ConvertAll(dataPair["RewardItemPercent"].Split('|'), v => float.Parse(v));
                DropStructures = new DropTableStructure[dropItemIndexArr.Length];
                for (int i = 0; i < dropItemIndexArr.Length; ++i)
                    DropStructures[i] = new DropTableStructure(dropItemIndexArr[i], dropItemCountArr[i], dropItemPercentage[i]);
            }
        }
    }
}