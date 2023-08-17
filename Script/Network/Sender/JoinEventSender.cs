using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public enum eJoinEventCode
{
    RequestPlayerData,
    NotifyPlayerData,
    RequestItemData,
    NotifyItemData,
    RequestInventoryData,
    NotifyInventoryData,
    RequestTileChunk,
    NotifyTileChunk,
    NotifyBuildNavmesh,
    RequestObjectChunk,
    NotifyObjectChunk,
}
public class JoinEventSender : BaseEventSender
{
    public void RequestPlayerData() => GeneratePacketOption(eJoinEventCode.RequestPlayerData, 0, ReceiverOption.Host);
    public void NotifyPlayerData(int requestID)
    {
        foreach(var element in PlayerManager.Instance.GetPlayerDictionary)
        {
            if (element.Key == requestID)
                continue;

            GeneratePacketOption(eJoinEventCode.NotifyPlayerData, element.Value.GetByteSize, requestID);
            element.Value.EnqueueByte();
        }
    }
    public void RequestItemData() => GeneratePacketOption(eJoinEventCode.RequestItemData, 0, ReceiverOption.Host);
    public void NotifyItemData(int playerID)
    {
        foreach(var element in ItemManager.Instance.GetItemDictionary)
        {
            GeneratePacketOption(eJoinEventCode.NotifyItemData, element.Value.GetByteSize, playerID);
            element.Value.EnqueueByte();
        }
    }
    public void RequestInventoryData() => GeneratePacketOption(eJoinEventCode.RequestInventoryData, 0, ReceiverOption.Host);
    public void NotifyInventoryData(int playerID)
    {
        foreach (var element in ItemManager.Instance.GetInventoryDictionary)
        {
            GeneratePacketOption(eJoinEventCode.NotifyInventoryData, element.Value.GetByteSize, playerID);
            element.Value.EnqueueByte();
        }
    }
    public void RequestTileChunk() => GeneratePacketOption(eJoinEventCode.RequestTileChunk, 0, ReceiverOption.Host);
    public void NotifyTileChunk(int playerID)
    {
        int capacity = Vector2IntSize + IntSize;
        foreach (var element in WorldManager.Instance.TileChunkSystem.GetOverrideTileDictionary)
        {
            if (!element.Value.IsModify)
                continue;

            if(element.Value.OverrideTileKey > 0)
            {
                GeneratePacketOption(eJoinEventCode.NotifyTileChunk, capacity, playerID);
                CopyBytes(element.Value.TilePosition);
                CopyBytes(element.Value.OverrideTileKey);
            }
        }
        GeneratePacketOption(eJoinEventCode.NotifyBuildNavmesh, 0, playerID);
    }
    public void RequestObjectChunk() => GeneratePacketOption(eJoinEventCode.RequestObjectChunk, 0, ReceiverOption.Host);
    public void NotifyObjectChunk(int playerID)
    {
        foreach(var element in ActorManager.Instance.GetSpawnedActors)
        {
            if(element.Value.IsAlive)
            {
                if(element.Value.ActorType == eActorType.StaticActor)
                {
                    IPacket packet = element.Value as IPacket;
                    if(packet != null)
                    {
                        GeneratePacketOption(eJoinEventCode.NotifyObjectChunk, packet.GetByteSize, playerID);
                        packet.EnqueueByte();
                    }
                }
            }
        }
    }
}
