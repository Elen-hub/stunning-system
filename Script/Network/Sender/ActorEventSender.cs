using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public enum eActorEventCode
{
    RequestInteractStart,
    NotifyInteractStart,
    RequestInteractEnd,
    NotifyInteractEnd,

    RequestEnqueueAttackProperty,
    NotifyEnqueueAttackProperty,

    NotifyHit,
    RequestUseSkill,
}
public class ActorEventSender : BaseEventSender
{
    public void RequestInteractStart(ulong casterID, ulong targetID)
    {
        int capacity = ULongSize + ULongSize;
        GeneratePacketOption(eActorEventCode.RequestInteractStart, capacity, ReceiverOption.Host);
        CopyBytes(casterID);
        CopyBytes(targetID);
    }
    public void NotifyInteractStart(bool isInteractSuccess, ulong casterID, ulong targetID)
    {
        int capacity = BooleanSize + ULongSize + ULongSize;
        GeneratePacketOption(eActorEventCode.NotifyInteractStart, capacity, ReceiverOption.Other);
        CopyBytes(isInteractSuccess);
        CopyBytes(casterID);
        CopyBytes(targetID);
    }
    public void RequestInteractEnd(ulong worldID)
    {
        int capacity = ULongSize;
        GeneratePacketOption(eActorEventCode.RequestInteractEnd, capacity, ReceiverOption.Host);
        CopyBytes(worldID);
    }
    public void NotifyInteractEnd(ulong worldID)
    {
        int capacity = ULongSize;
        GeneratePacketOption(eActorEventCode.NotifyInteractEnd, capacity, ReceiverOption.Other);
        CopyBytes(worldID);
    }
    public void RequestEnqueueAttackProperty(eDamageType damageType, ulong casterID, ulong targetID, bool isCritical, float damage)
    {
        int capacity = ShortSize + ULongSize + ULongSize + BooleanSize + FloatSize;
        GeneratePacketOption(eActorEventCode.RequestEnqueueAttackProperty, capacity, ReceiverOption.Host);
        CopyBytes((short)damageType);
        CopyBytes(casterID);
        CopyBytes(targetID);
        CopyBytes(isCritical);
        CopyBytes(damage);
    }
    public void RequestUseSkill(int skillIndex, ulong worldID, Vector2 direction)
    {
        int capacity = IntSize + ULongSize + Vector2Size;
        GeneratePacketOption(eActorEventCode.RequestUseSkill, capacity, ReceiverOption.Host);
        CopyBytes(skillIndex);
        CopyBytes(worldID);
        CopyBytes(direction);
    }
}
