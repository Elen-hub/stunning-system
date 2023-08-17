using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using Cysharp.Threading.Tasks;

public class JoinEventReceiver : BaseEventReceiver
{

    public JoinEventReceiver()
    {
        MappingReceiveEvent(eJoinEventCode.RequestPlayerData, OnReplyPlayerData);
        MappingReceiveEvent(eJoinEventCode.NotifyPlayerData, OnNotifyPlayerData);
        MappingReceiveEvent(eJoinEventCode.RequestItemData, OnReplyItemData);
        MappingReceiveEvent(eJoinEventCode.NotifyItemData, OnNotifyItemData);
        MappingReceiveEvent(eJoinEventCode.RequestInventoryData, OnReplyInventoryData);
        MappingReceiveEvent(eJoinEventCode.NotifyInventoryData, OnNotifyInventoryData);
        MappingReceiveEvent(eJoinEventCode.RequestTileChunk, OnReplyTileChunk);
        MappingReceiveEvent(eJoinEventCode.NotifyTileChunk, OnNotifyTileChunk);
        MappingReceiveEvent(eJoinEventCode.NotifyBuildNavmesh, OnNotifyBuildNavmesh);
        MappingReceiveEvent(eJoinEventCode.RequestObjectChunk, OnReplyObjectChunk);
        MappingReceiveEvent(eJoinEventCode.NotifyObjectChunk, OnNotifyObjectChunk);
    }
    protected bool OnReplyPlayerData(int playerID)
    {
        NetworkManager.Instance.JoinEventSender.NotifyPlayerData(playerID);
        DebugUtility.Log($"OnReplyPlayerData - PlayerID: {playerID}");

        return true;
    }
    protected bool OnNotifyPlayerData(int playerID)
    {
        Player player = GetPlayer();
        PlayerManager.Instance.JoinPlayer(player);
        DebugUtility.Log($"OnNotifyPlayerData");

        return true;
    }
    protected bool OnReplyItemData(int playerID)
    {
        NetworkManager.Instance.JoinEventSender.NotifyItemData(playerID);
        DebugUtility.Log($"OnReplyItemData - PlayerID: {playerID}");

        return true;
    }
    protected bool OnNotifyItemData(int playerID)
    {
        Item item = GetItem();
        ulong worldID = GetUInt64();
        Vector2 worldPosition = GetVector2();
        ItemManager.Instance.SpawnItem(item, worldPosition, worldID);
        DebugUtility.Log($"OnNotifyItemData");

        return true;
    }
    protected bool OnReplyInventoryData(int playerID)
    {
        NetworkManager.Instance.JoinEventSender.NotifyInventoryData(playerID);
        DebugUtility.Log($"OnReplyInventoryData - PlayerID: {playerID}");

        return true;
    }
    protected bool OnNotifyInventoryData(int playerID)
    {
        ItemManager.Instance.AddInventory(GetInventory());
        DebugUtility.Log($"OnNotifyInventoryData");

        return true;
    }
    protected bool OnReplyTileChunk(int playerID)
    {
        NetworkManager.Instance.JoinEventSender.NotifyTileChunk(playerID);
        DebugUtility.Log($"OnReplyTileChunk - PlayerID: {playerID}");

        return true;
    }
    protected bool OnNotifyTileChunk(int playerID)
    {
        Vector2Int worldPosition = GetVector2Int();
        int overrideKey = GetInt32();
        WorldManager.Instance.TileChunkSystem.SetOverrideTile(worldPosition, overrideKey);
        DebugUtility.Log($"OnNotifyTileChunk");

        return true;
    }
    protected bool OnNotifyBuildNavmesh(int playerID)
    {
        WorldManager.Instance.TileChunkSystem.BuildNavmesh();
        DebugUtility.Log($"OnNotifyBuildNavmesh - PlayerID: {playerID}");

        return true;
    }
    protected bool OnReplyObjectChunk(int playerID)
    {
        NetworkManager.Instance.JoinEventSender.NotifyObjectChunk(playerID);
        DebugUtility.Log($"OnReplyObjectChunk - PlayerID: {playerID}");

        return true;
    }
    protected bool OnNotifyObjectChunk(int playerID)
    {
        int index = GetInt32();
        ActorManager.Instance.SpawnClientObject(index, this);
        DebugUtility.Log($"OnNotifyObjectChunk");

        return true;
    }
}
