using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public enum ePlayerEventCode
{
    RequestJoinPlayer,
    NotifyJoinPlayer,
    RequestSendMessage,
    NotifySendMessage,
}
public class PlayerEventSender : BaseEventSender
{
    public void RequestJoinPlayer(Player player)
    {
        int capacity = player.GetByteSize;
        GeneratePacketOption(ePlayerEventCode.RequestJoinPlayer, capacity, ReceiverOption.Host);
        player.EnqueueByte();
    }
    public void NotifyJoinPlayer(Player player)
    {
        int capacity = player.GetByteSize;
        GeneratePacketOption(ePlayerEventCode.NotifyJoinPlayer, capacity, ReceiverOption.Other);
        player.EnqueueByte();
    }
    public void RequestSendMessage(MessageStructure message)
    {
        int capacity = message.GetByteSize;
        GeneratePacketOption(ePlayerEventCode.RequestSendMessage, capacity, ReceiverOption.Host);
        message.EnqueueByte();
    }
    public void NotifySendMessage(MessageStructure message)
    {
        int capacity = message.GetByteSize;
        GeneratePacketOption(ePlayerEventCode.NotifySendMessage, capacity, ReceiverOption.Other);
        message.EnqueueByte();
    }
}
