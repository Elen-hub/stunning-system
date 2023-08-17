using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
public abstract class ActorStat
{
    protected Dictionary<eStatusType, StatusFloat> _statusDic;
    public float this[eStatusType type] {
        get {
            if (!_statusDic.ContainsKey(type))
                return 0f;

            return StatusCalculator.Calculate(this, type);
        }
    }
    public StatusFloat GetStatusContents(eStatusType type)
    {
        return _statusDic.ContainsKey(type) ? _statusDic[type] : null;
    }
}
