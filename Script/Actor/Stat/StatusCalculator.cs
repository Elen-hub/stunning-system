using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

public class StatusCalculator
{
    public delegate float DEL_GetStatus(ActorStat actorStat, eStatusType type);
    static Dictionary<eStatusType, DEL_GetStatus> _calculateMethod = new Dictionary<eStatusType, DEL_GetStatus>(52)
    {
        { eStatusType.HP, CalculateCustomToSum },
        { eStatusType.HPRecovery, CalculateCustomToSum },
        { eStatusType.SP, CalculateCustomToSum },
        { eStatusType.SPRecovery, CalculateCustomToSum },
        { eStatusType.MoveSpeed, CalculateCustomToMul },
        { eStatusType.WalkSpeed, CalculateCustomToMul },
        { eStatusType.RunSpeed, CalculateCustomToMul },
    };
    public static void Init(Dictionary<int, Dictionary<string, string>> dataDic)
    {
        foreach (var contents in dataDic)
        {
            int index = contents.Key;
            string statusName = contents.Value["StatusName"];
        }
    }
    public static float Calculate(ActorStat actorStat, eStatusType type)
    {
        if (_calculateMethod.ContainsKey(type)) return _calculateMethod[type](actorStat, type);
        else return actorStat.GetStatusContents(type).GetValue;
    }
    static float CalculateDefaultStat(ActorStat actorStat, eStatusType type)
    {
        return actorStat.GetStatusContents(type).GetValue;
    }
    static float CalculateCustomToSum(ActorStat actorStat, eStatusType type)
    {
        float value = actorStat.GetStatusContents(type).GetValue;
        StatusWeightData[] contents = DataManager.Instance.StatusWeightTable[type];
        if (contents == null)
            return value;

        for (int i = 0; i < contents.Length; ++i)
            value += actorStat[contents[i].StatType] * contents[i].Value;

        return value;
    }
    static float CalculateCustomToMul(ActorStat actorStat, eStatusType type)
    {
        float value = actorStat.GetStatusContents(type).GetValue;
        StatusWeightData[] contents = DataManager.Instance.StatusWeightTable[type];
        if (contents == null)
            return value;

        for (int i = 0; i < contents.Length; ++i)
            value *= actorStat[contents[i].StatType] * contents[i].Value;

        return value;
    }
    static float CalculateCustomToDiv(ActorStat actorStat, eStatusType type)
    {
        return 0;
    }
}