using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusBuff : Buff
{
    public eStatusType StatusType;
    uint _key;
    protected override void OnEvent(bool isActive)
    {
        if(isActive)
        {
            if(Data.IsMul) _key = _actor.ActorStat.GetStatusContents(StatusType).AddMul(Data.Value);
            else _key = _actor.ActorStat.GetStatusContents(StatusType).AddSum(Data.Value);
        }
        else
        {
            if (Data.IsMul) _actor.ActorStat.GetStatusContents(StatusType).Remove(eStatusCalculateSign.Mul, _key);
            else _actor.ActorStat.GetStatusContents(StatusType).Remove(eStatusCalculateSign.Add, _key);
        }
    }
}
