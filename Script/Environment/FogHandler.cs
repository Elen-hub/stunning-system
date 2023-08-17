using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

public class FogHandler : IPacket
{
    public float StartTime;
    public float ElapsedTime;
    public float DurationTime;
    public float Intencity;
    public bool IsStart;
    #region Network
    public int GetByteSize => ReliableHelper.FloatSize + ReliableHelper.FloatSize + ReliableHelper.FloatSize + ReliableHelper.FloatSize;
    public void EnqueueByte()
    {
        BaseEventSender.CopyBytes(StartTime);
        BaseEventSender.CopyBytes(ElapsedTime);
        BaseEventSender.CopyBytes(DurationTime);
        BaseEventSender.CopyBytes(Intencity);
    }
    #endregion
}
