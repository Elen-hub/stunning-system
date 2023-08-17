using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using Cysharp.Threading.Tasks;
public class ObjectEventReceiver : BaseEventReceiver
{
    public ObjectEventReceiver()
    {
        MappingReceiveEvent(eObjectEventCode.RequestInstallObject, OnReplyInstallObject);
        MappingReceiveEvent(eObjectEventCode.NotifyInstallObject, OnNotifyInstallObject);
        MappingReceiveEvent(eObjectEventCode.RequestStartCook, OnReplyStartCook);
        MappingReceiveEvent(eObjectEventCode.NotifyStartCook, OnNotifyStartCook);
        MappingReceiveEvent(eObjectEventCode.RequestEndCook, OnReplyEndCook);
        MappingReceiveEvent(eObjectEventCode.NotifyStopCook, OnNotifyStopCook);
        MappingReceiveEvent(eObjectEventCode.NotifyStartRefinement, OnNotifyStartRefinement);
        MappingReceiveEvent(eObjectEventCode.NotifyStopRefinement, OnNotifyStopRefinement);
        MappingReceiveEvent(eObjectEventCode.NotifyObjectHit, OnNotifyObjectHit);
        MappingReceiveEvent(eObjectEventCode.NotifyDestroyObject, OnNotifyDestroyObject);
        MappingReceiveEvent(eObjectEventCode.NotifySpawnObject, OnNotifySpawnObject);
    }
    protected bool OnReplyInstallObject(int playerID)
    {
        int index = GetInt32();
        Vector2Int tilePosition = GetVector2Int();
        string ownerGUID = GetString();
        if (GridManager.Instance.IsOverlapObject(tilePosition, DataManager.Instance.ObjectTable[index].TileList))
        {
            Debug.LogWarning("Overlap");
            // 아이템 되돌려줌
        }
        else
        {
            ActorManager.Instance.SpawnServerObject(index, tilePosition, ownerGUID).ContinueWith(result=>
            {
                result.CurrentHP = DataManager.Instance.ObjectTable[index].HP;
                NetworkManager.Instance.ObjectEventSender.NotifyInstallObject(result);
            });
        }
        DebugUtility.Log($"OnReplyInstallObject - ObjectIndex: {index} WorldPosition: {tilePosition} OwnerID: {ownerGUID}");

        return true;
    }
    protected bool OnNotifyInstallObject(int playerID)
    {
        int index = GetInt32();
        ActorManager.Instance.SpawnClientObject(index, this);
        DebugUtility.Log($"OnNotifyInstallObject - ObjectIndex: {index}");

        return true;
    }
    protected bool OnNotifyDestroyObject(int playerID)
    {
        ulong objectKey = GetUInt64();
        IActor actor = ActorManager.Instance.GetActor(objectKey);
        if (actor == null)
            return false;

        actor.Destroy();
        return true;
    }
    protected bool OnNotifySpawnObject(int playerID)
    {
        int index = GetInt32();
        ActorManager.Instance.SpawnClientObject(index, this);
        return true;
    }
    protected bool OnReplyStartCook(int playerID)
    {
        ulong worldID = GetUInt64();
        CookObject cookObject = ActorManager.Instance.GetActor<CookObject>(worldID);
        if (cookObject == null)
            return false;

        bool isPossibleCraft = cookObject.IsPossibleCraft(true);
        if (isPossibleCraft)
            cookObject.StartCraft();

        NetworkManager.Instance.ObjectEventSender.NotifyStartCook(worldID, cookObject.GetMakeIndex, isPossibleCraft);
        DebugUtility.Log($"OnRequestStartCook - ObjectID: {worldID}");

        return true;
    }
    bool OnNotifyStartCook(int playerID)
    {
        ulong worldID = GetUInt64();
        int craftIndex = GetInt32();
        bool isTrue = GetBoolean();
        if (isTrue)
        {
            CookObject cookObject = ActorManager.Instance.GetActor<CookObject>(worldID);
            if (cookObject == null)
                return false;

            cookObject.StartCraft(craftIndex);
        }
        DebugUtility.Log($"OnNotifyStartCook - ObjectID: {worldID} CraftIndex: {craftIndex}");

        return true;
    }
    bool OnReplyEndCook(int playerID)
    {
        ulong worldID = GetUInt64();
        CookObject cookObject = ActorManager.Instance.GetActor<CookObject>(worldID);
        if (cookObject == null)
            return false;

        bool isPossibleCraft = cookObject.IsPossibleCraft(false);
        if (isPossibleCraft)
            cookObject.StopCraft();

        NetworkManager.Instance.ObjectEventSender.NotifyStopCook(worldID, isPossibleCraft);
        DebugUtility.Log($"OnRequestEndCook - ObjectID: {worldID}");

        return true;
    }
    bool OnNotifyStopCook(int playerID)
    {
        ulong worldID = GetUInt64();
        bool isTrue = GetBoolean();
        if (isTrue)
        {
            CookObject cookObject = ActorManager.Instance.GetActor<CookObject>(worldID);
            if (cookObject == null)
                return false;

            cookObject.StopCraft();
        }
        DebugUtility.Log($"OnNotifyStopCook - ObjectID: {worldID}");

        return true;
    }
    bool OnNotifyStartRefinement(int playerID)
    {
        ulong worldID = GetUInt64();
        int craftIndex = GetInt32();
        RefinementObject refinementObject = ActorManager.Instance.GetActor<RefinementObject>(worldID);
        if (refinementObject == null)
            return false;

        refinementObject.StartCraft(craftIndex);
        DebugUtility.Log($"OnNotifyStartRefinement - ObjectID: {worldID} CraftIndex: {craftIndex}");

        return true;
    }
    bool OnNotifyStopRefinement(int playerID)
    {
        ulong worldID = GetUInt64();
        RefinementObject refinementObject = ActorManager.Instance.GetActor<RefinementObject>(worldID);
        if (refinementObject == null)
            return false;

        refinementObject.StopCraft();
        DebugUtility.Log($"OnNotifyStopRefinement - ObjectID: {worldID}");

        return true;
    }
    bool OnNotifyObjectHit(int playerID)
    {
        ulong worldID = GetUInt64();
        eDamageType damageType = (eDamageType)GetInt16();
        float targetHP = GetFloat();
        StaticActor actor = ActorManager.Instance.GetActor<StaticActor>(worldID);
        if (actor == null)
            return false;

        actor.Hit(damageType, targetHP);
        DebugUtility.Log($"OnNotifyObjectHit - ObjectID: {worldID}");

        return true;
    }
}
