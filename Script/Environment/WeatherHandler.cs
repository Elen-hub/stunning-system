using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

[System.Serializable]
public class WeatherHandler : Network.IPacket
{
    public static WeatherHandler Null = new WeatherHandler() { IsNull = true };
    public bool IsNull;
    public eWeatherType WeahterType;
    public float StartTime;
    public float DurationTime;
    public float Intencity;

    #region Network
    public int GetByteSize => IsNull ? ReliableHelper.BooleanSize : ReliableHelper.BooleanSize + ReliableHelper.ShortSize + ReliableHelper.FloatSize + ReliableHelper.FloatSize + ReliableHelper.FloatSize;
    public void EnqueueByte()
    {
        if (IsNull)
        {
            BaseEventSender.CopyBytes(true);
        }
        else
        {
            BaseEventSender.CopyBytes(false);
            BaseEventSender.CopyBytes((short)WeahterType);
            BaseEventSender.CopyBytes(StartTime);
            BaseEventSender.CopyBytes(DurationTime);
            BaseEventSender.CopyBytes(Intencity);
        }
    }
    #endregion
}
