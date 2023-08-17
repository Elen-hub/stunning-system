using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstallItem : UseItem
{
    public Data.ObjectData ObjectData => DataManager.Instance.ObjectTable[_installKey];
    Skill_InstallObject _installSkill;
    public Skill_InstallObject GetInstallSkill {
        get {
            if (_installSkill == null)
                _installSkill = GetSkill as Skill_InstallObject;

            return _installSkill;
        }
    }
    int _installKey;
    public override string GetInformationText()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(base.GetInformationText());

        return stringBuilder.ToString();
    }
    public override void OnEnterSlot(IComponent actor)
    {
        base.OnEnterSlot(actor);

        if(_installKey == 0)
        {
            Skill_InstallObject installSkill = DataManager.Instance.SkillTable[ItemData.ReferenceIndex].Skill as Skill_InstallObject;
            _installKey = installSkill._installData.ObjectIndex;
        }
        actor.SendComponentMessage(eComponentEvent.InstallMode, true, ObjectData.Index);
    }
    public override void OnExitSlot(IComponent actor)
    {
        base.OnExitSlot(actor);

        actor.SendComponentMessage(eComponentEvent.InstallMode, false, 0);
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
        --Count;
    }
}
