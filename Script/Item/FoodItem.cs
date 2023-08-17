using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodItem : UseItem
{
    public FoodItem()
    {

    }
    public override void OnEnterSlot(IComponent actor)
    {
        base.OnEnterSlot(actor);
    }
    public override void OnExitSlot(IComponent actor)
    {
        base.OnExitSlot(actor);
    }
    public override bool IsUseable(IActor actor)
    {
        if (!base.IsUseable(actor))
            return false;

        return GetSkill.IsUseable(actor);
    }
    public override void OnUseStart(IActor actor)
    {
        NetworkManager.Instance.ActorEventSender.RequestUseSkill(ItemData.ReferenceIndex, actor.WorldID, actor.Direction);
        GetSkill.StartSequence(actor, actor.Direction);
        --Count;
    }
}
