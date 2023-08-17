using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public class PlayerEventReceiver : BaseEventReceiver
{
    public static event System.Action<MessageStructure> MessageUIEvent;
    public PlayerEventReceiver()
    {
        MappingReceiveEvent(ePlayerEventCode.RequestJoinPlayer, OnReplyJoinPlayer);
        MappingReceiveEvent(ePlayerEventCode.NotifyJoinPlayer, OnNotifyJoinPlayer);
        MappingReceiveEvent(ePlayerEventCode.RequestSendMessage, OnReplySendMessage);
        MappingReceiveEvent(ePlayerEventCode.NotifySendMessage, OnNotifySendMessage);
    }
    protected bool OnReplyJoinPlayer(int playerID)
    {
        Player player = GetPlayer();
        player.PlayerID = playerID;
        bool isCached = PlayerManager.Instance.IsCachedPlayer(player.Guid);
        if (isCached)
        {
            PlayerManager.Instance.ActivateCachePlayer(playerID, player.Guid);
        }
        else
        {
            PlayerManager.Instance.JoinPlayer(player);
        }
        NetworkManager.Instance.PlayerEventSender.NotifyJoinPlayer(player);
        DebugUtility.Log($"OnReplyJoinPlayer - PlayerID: {playerID}");

        return true;
    }
    protected bool OnNotifyJoinPlayer(int playerID)
    {
        Player player = GetPlayer();
        if (player.PlayerID == Runner.LocalPlayer)
            return true;

        PlayerManager.Instance.JoinPlayer(player);
        DebugUtility.Log($"OnNotifyJoinPlayer - PlayerID: {playerID}");

        return true;
    }
    protected bool OnReplySendMessage(int playerID)
    {
        MessageStructure message = GetMessageStructure();
        NetworkManager.Instance.PlayerEventSender.NotifySendMessage(message);
        DebugUtility.Log($"OnReplySendMessage - PlayerID: {message.PlayerID}");

        return true;
    }
    protected bool OnNotifySendMessage(int playerID)
    {
        MessageStructure message = GetMessageStructure();
        MessageUIEvent(message);
        DebugUtility.Log($"OnNotifySendMessage - PlayerID: {message.PlayerID}");
        return true;
    }
}
