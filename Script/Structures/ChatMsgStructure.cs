using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

[System.Flags]
public enum eMessageType : ushort
{
    Normal,
    System,
}
public struct MessageStructure : IPacket
{
    public eMessageType Type;
    public int PlayerID;
    public string Name;
    public string Message;
    public MessageStructure(eMessageType type, int playerID, string id, string message)
    {
        Type = type;
        PlayerID = playerID;
        Name = id;
        Message = message;
    }
    public bool IsValidMessage()
    {
        return true;
    }
    #region Network
    public int GetByteSize =>
        ReliableHelper.UShortSize +
        ReliableHelper.IntSize +
        ReliableHelper.StringSize(Name.Length) +
        ReliableHelper.StringSize(Message.Length);
    public void EnqueueByte()
    {
        BaseEventSender.CopyBytes((ushort)Type);
        BaseEventSender.CopyBytes(PlayerID);
        BaseEventSender.CopyBytes(Name);
        BaseEventSender.CopyBytes(Message);
    }
    #endregion
}
