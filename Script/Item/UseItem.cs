using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseItem : Item
{
    public BaseSkill GetSkill => DataManager.Instance.SkillTable[ItemData.ReferenceIndex].Skill;
    public BaseSkill GetExSkill => DataManager.Instance.SkillTable[ItemData.ExReferenceIndex].Skill;
    public override string GetInformationText()
    {
        string infoText = GetSkill.GetInformationText();
        if (string.IsNullOrEmpty(infoText))
            return base.GetInformationText();

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(base.GetInformationText());
        stringBuilder.Append("\n\n").Append(infoText);

        return stringBuilder.ToString();
    }
    public virtual bool IsUseable(IActor actor)
    {
        return true;
    }
    public virtual void OnUseStart(IActor actor)
    {

    }
    public virtual bool IsSpecialUseable(IActor actor)
    {
        if (ItemData.ExReferenceIndex == 0)
            return false;

        return true;
    }
    public virtual void OnSpecialUseStart(IActor actor)
    {

    }
}
