using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CropItem : UseItem
{
    public Data.ObjectData ObjectData => DataManager.Instance.ObjectTable[_installKey];
    Skill_InstallCrop _installSkill;
    public Skill_InstallCrop GetInstallSkill
    {
        get {
            if (_installSkill == null)
                _installSkill = GetSkill as Skill_InstallCrop;

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

        GridManager.Instance.OverlapCondition += GetInstallSkill.IsPossibleInstall;
        actor.SendComponentMessage(eComponentEvent.InstallMode, true, ObjectData.Index);
    }
    public override void OnExitSlot(IComponent actor)
    {
        base.OnExitSlot(actor);

        GridManager.Instance.OverlapCondition -= GetInstallSkill.IsPossibleInstall;
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
