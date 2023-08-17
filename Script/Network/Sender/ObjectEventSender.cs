using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public enum eObjectEventCode
{
    RequestInstallObject,
    NotifyInstallObject,
    RequestStartCook,
    NotifyStartCook,
    RequestEndCook,
    NotifyStopCook,
    NotifyStartRefinement,
    NotifyStopRefinement,
    NotifyObjectHit,
    NotifyDestroyObject,
    NotifySpawnObject,
}
public class ObjectEventSender : BaseEventSender
{
    public void RequestInstallObject(int index, Vector2Int tilePosition, string ownerGUID)
    {
        int capacity = IntSize + Vector2IntSize + StringSize(ownerGUID.Length);
        GeneratePacketOption(eObjectEventCode.RequestInstallObject, capacity, ReceiverOption.Host);
        CopyBytes(index);
        CopyBytes(tilePosition);
        CopyBytes(ownerGUID);
    }
    public void NotifyInstallObject(StaticActor staticActor)
    {
        int capacity = staticActor.GetByteSize;
        GeneratePacketOption(eObjectEventCode.NotifyInstallObject, capacity, ReceiverOption.Other);
        staticActor.EnqueueByte();
    }
    public void NotifyDestroyObject(ulong objectKey)
    {
        int capacity = ULongSize;
        GeneratePacketOption(eObjectEventCode.NotifyDestroyObject, capacity, ReceiverOption.Other);
        CopyBytes(objectKey);
    }
    public void RequestStartCook(ulong worldID)
    {
        int capacity = ULongSize;
        GeneratePacketOption(eObjectEventCode.RequestStartCook, capacity, ReceiverOption.Host);
        CopyBytes(worldID);
    }
    public void NotifyStartCook(ulong worldID, int craftIndex, bool isTrue)
    {
        int capacity = ULongSize + IntSize + BooleanSize;
        GeneratePacketOption(eObjectEventCode.NotifyStartCook, capacity, ReceiverOption.Other);
        CopyBytes(worldID);
        CopyBytes(craftIndex);
        CopyBytes(isTrue);
    }
    public void RequestStopCook(ulong worldID)
    {
        int capacity = ULongSize;
        GeneratePacketOption(eObjectEventCode.RequestEndCook, capacity, ReceiverOption.Host);
        CopyBytes(worldID);
    }
    public void NotifyStopCook(ulong worldID, bool isTrue)
    {
        int capacity = ULongSize + BooleanSize;
        GeneratePacketOption(eObjectEventCode.NotifyStopCook, capacity, ReceiverOption.Other);
        CopyBytes(worldID);
        CopyBytes(isTrue);
    }
    public void NotifyStartRefinement(ulong worldID, int craftIndex)
    {
        int capacity = ULongSize + IntSize;
        GeneratePacketOption(eObjectEventCode.NotifyStartRefinement, capacity, ReceiverOption.Other);
        CopyBytes(worldID);
        CopyBytes(craftIndex);
    }
    public void NotifyStopRefinement(ulong worldID)
    {
        int capacity = ULongSize;
        GeneratePacketOption(eObjectEventCode.NotifyStopRefinement, capacity, ReceiverOption.Other);
        CopyBytes(worldID);
    }
    public void NotifyObjectHit(ulong worldID, eDamageType damageType, float hp)
    {
        int capacity = ULongSize + ShortSize + FloatSize;
        GeneratePacketOption(eObjectEventCode.NotifyObjectHit, capacity, ReceiverOption.Other);
        CopyBytes(worldID);
        CopyBytes((short)damageType);
        CopyBytes(hp);
    }
}
