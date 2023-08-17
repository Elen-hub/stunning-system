using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

public class PlayerCharacterData : IPacket
{
    public string Name;
    public int Index;
    #region Netwokred
    public int GetByteSize => ReliableHelper.StringSize(Name.Length) + ReliableHelper.IntSize;
    public void EnqueueByte()
    {
        BaseEventSender.CopyBytes(Name);
        BaseEventSender.CopyBytes(Index);
    }
    #endregion
}