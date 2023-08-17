using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : UseItem
{
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
        GetSkill.StartSequence(actor, actor.Direction);
    }
    public override bool IsSpecialUseable(IActor actor)
    {
        if (!base.IsSpecialUseable(actor))
            return false;

        return GetExSkill.IsUseable(actor);
    }
    public override void OnSpecialUseStart(IActor actor)
    {
        GetExSkill.StartSequence(actor, actor.Direction);
    }
}
