using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public class ActorEventReceiver : BaseEventReceiver
{
    public ActorEventReceiver()
    {
        MappingReceiveEvent(eActorEventCode.RequestInteractStart, OnReplyInteractStart);
        MappingReceiveEvent(eActorEventCode.NotifyInteractStart, OnNotifyInteractStart);
        MappingReceiveEvent(eActorEventCode.RequestInteractEnd, OnReplyInteractEndt);
        MappingReceiveEvent(eActorEventCode.NotifyInteractEnd, OnReplyInteractEnd);
        MappingReceiveEvent(eActorEventCode.RequestEnqueueAttackProperty, OnReplyEnqueueAttackProperty);
        MappingReceiveEvent(eActorEventCode.RequestUseSkill, OnReplyUseSkill);
    }
    protected bool OnReplyInteractStart(int playerID)
    {
        ulong casterID = GetUInt64();
        ulong targetID = GetUInt64();
        IActor interactActor = ActorManager.Instance.GetActor(casterID);
        if (interactActor == null)
            return false;

        InteractObject interactObject = ActorManager.Instance.GetActor<InteractObject>(targetID);
        if (interactObject == null)
            return false;

        if (interactObject.IsPossibleInteract(interactActor))
        {
            interactObject.Interact(interactActor);
            NetworkManager.Instance.ActorEventSender.NotifyInteractStart(true, casterID, targetID);
        }
        else
            NetworkManager.Instance.ActorEventSender.NotifyInteractStart(false, casterID, targetID);

        DebugUtility.Log($"OnReplyInteractStart - CasterID: {casterID} TargetID: {targetID}");

        return true;
    }
    protected bool OnNotifyInteractStart(int playerID)
    {
        bool isInteractSuccess = GetBoolean();
        ulong casterID = GetUInt64();
        ulong targetID = GetUInt64();
        IActor interactActor = ActorManager.Instance.GetActor(casterID);
        if (interactActor == null)
            return false;

        IInteractable interactTarget = ActorManager.Instance.GetActor(targetID) as IInteractable;
        if (interactTarget == null)
            return false;

        if(isInteractSuccess)
            interactTarget.Interact(interactActor);

        DebugUtility.Log($"OnNotifyInteractStart - CasterID: {casterID} TargetID: {targetID}");

        return true;
    }
    protected bool OnReplyInteractEndt(int playerID)
    {
        ulong worldID = GetUInt64();
        IInteractable interactTarget = ActorManager.Instance.GetActor(worldID) as IInteractable;
        if (interactTarget == null)
            return false;

        interactTarget.InteractExit();
        NetworkManager.Instance.ActorEventSender.NotifyInteractEnd(worldID);
        DebugUtility.Log($"OnReplyInteractEndt - WorldID: {worldID}");

        return true;
    }
    protected bool OnReplyInteractEnd(int playerID)
    {
        ulong worldID = GetUInt64();
        IInteractable interactTarget = ActorManager.Instance.GetActor(worldID) as IInteractable;
        if (interactTarget == null)
            return false;

        interactTarget.InteractExit();
        DebugUtility.Log($"OnReplyInteractEnd - WorldID: {worldID}");

        return true;
    }
    protected bool OnReplyEnqueueAttackProperty(int playerID)
    {
        AttackProperty attackProperty = GetAttackProperty();
        ActorManager.Instance.EnqueueAttackProperty(attackProperty);
        DebugUtility.Log($"OnReplyEnqueueAttackProperty - CasterID: {attackProperty.CasterID} TargetID: {attackProperty.TargetID} Damage: {attackProperty.Damage}");

        return true;
    }
    protected bool OnReplyUseSkill(int playerID)
    {
        int skillIndex = GetInt32();
        ulong worldID = GetUInt64();
        Vector2 direction = GetVector2();
        Data.SkillData skillData = DataManager.Instance.SkillTable[skillIndex];
        if (skillData == null) return false;
        IActor actor = ActorManager.Instance.GetActor(worldID);
        if (actor == null) return false;
        skillData.Skill.StartSequence(actor, direction);
        DebugUtility.Log($"OnReplyUseSkill - SkillIndex: {skillIndex} WorldID: {worldID} Direction: {direction}");

        return true;
    }
}
