using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public readonly struct ActorStatData
{
    public static ActorStatData Null = new ActorStatData();
    readonly Dictionary<eStatusType, float> _statusDic;
    public Dictionary<eStatusType, StatusFloat> GetStatus
    {
        get
        {
            Dictionary<eStatusType, StatusFloat> status = new Dictionary<eStatusType, StatusFloat>(_statusDic.Count);
            foreach (var value in _statusDic)
                status.Add(value.Key, new StatusFloat(value.Value));

            return status;
        }
    }
    public ActorStatData(Dictionary<string, string> data)
    {
        _statusDic = new Dictionary<eStatusType, float>();
        foreach (var contents in data)
            if (EnumConverter.StatusTypeConvert.ContainsKey(contents.Key))
            {
                float parseValue = 0f;
                if (float.TryParse(contents.Value, out parseValue))
                    _statusDic.Add(EnumConverter.StatusTypeConvert[contents.Key], parseValue);
                else
                    _statusDic.Add(EnumConverter.StatusTypeConvert[contents.Key], 0f);
            }
    }
}
