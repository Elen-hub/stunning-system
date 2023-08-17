using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMagazineItem : UseItem, IReloadable
{
    Skill_AutometicReload _reloadSkill;
    public Skill_AutometicReload GetReloadSkill {
        get {
            if (_reloadSkill == null)
                _reloadSkill = GetSkill as Skill_AutometicReload;

            return _reloadSkill;
        }
    }
    public int BulletIndex => Index;
    protected int _reloadedCount;
    public int ReloadedCount
    {
        get => _reloadedCount;
        set {
            _reloadedCount = value;
            OnChangedBullet?.Invoke();
        }
    }
    public event System.Action OnChangedBullet;
    public override void OnEnterSlot(IComponent actor)
    {
        base.OnEnterSlot(actor);

        UIManager.Instance.Open<UBulletMagazineUI>(eUIName.UBulletMagazineUI).SetReloadable(this);
    }
    public override void OnExitSlot(IComponent actor)
    {
        base.OnExitSlot(actor);

        UIManager.Instance.Close(eUIName.UBulletMagazineUI);
    }
    public override string GetInformationText()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(base.GetInformationText());
        stringBuilder.Append("\n\n").Append(LocalizingManager.Instance.GetLocalizing(8008, 
            LocalizingManager.Instance.GetLocalizing(DataManager.Instance.ItemTable[GetReloadSkill._combatData.RequireItemIndex].NameKey),
            ReloadedCount,
            GetReloadSkill._combatData.MagCapacity
            ));

        return stringBuilder.ToString();
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
    public bool IsPossibleReload(IActor actor) => IsUseable(actor);
    public void Reload(IActor actor) => OnUseStart(actor);
}
