using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticGunItem : UseItem, IReloadable
{
    Skill_Gun _gunSkill;
    public Skill_Gun GetGunSkill {
        get {
            if (_gunSkill == null)
                _gunSkill = GetSkill as Skill_Gun;

            return _gunSkill;
        }
    }
    public int BulletIndex => _bulletMag.Index;
    BulletMagazineItem _bulletMag;
    public BulletMagazineItem BulletMagazineItem => _bulletMag;
    public override string GetInformationText()
    {
        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(base.GetInformationText());
        if (_bulletMag != null)
        {
            stringBuilder.Append("\n\n").Append(LocalizingManager.Instance.GetLocalizing(8007, LocalizingManager.Instance.GetLocalizing(DataManager.Instance.ItemTable[_bulletMag.Index].NameKey)));
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8008, LocalizingManager.Instance.GetLocalizing(DataManager.Instance.ItemTable[_bulletMag.GetReloadSkill._combatData.RequireItemIndex].NameKey), _bulletMag.ReloadedCount, _bulletMag.GetReloadSkill._combatData.MagCapacity));
        }
        else
        {
            stringBuilder.Append("\n\n").Append(LocalizingManager.Instance.GetLocalizing(8006, LocalizingManager.Instance.GetLocalizing(DataManager.Instance.ItemTable[GetGunSkill._reloadData.BulletMagazineIndex].NameKey)));
        }
        return stringBuilder.ToString();
    }
    public override void OnEnterSlot(IComponent actor)
    {
        base.OnEnterSlot(actor);

        if(_bulletMag != null)
            UIManager.Instance.Open<UBulletMagazineUI>(eUIName.UBulletMagazineUI).SetReloadable(_bulletMag);
    }
    public override void OnExitSlot(IComponent actor)
    {
        base.OnExitSlot(actor);

        if (_bulletMag != null)
            UIManager.Instance.Close(eUIName.UBulletMagazineUI);
    }
    public override bool IsUseable(IActor actor)
    {
        if (_bulletMag == null)
            return false;

        if (_bulletMag.ReloadedCount <= 0)
            return false;

        if (!base.IsUseable(actor))
            return false;

        return GetSkill.IsUseable(actor);
    }
    public override void OnUseStart(IActor actor)
    {
        _bulletMag.ReloadedCount -= 1;
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
    public bool IsPossibleReload(IActor actor)
    {
        if (actor.Controller.IsBlockType(eActionBlockType.Action))
            return false;

        ItemComponent itemComponent = actor.GetComponent<ItemComponent>(eComponent.ItemComponent);
        if (itemComponent == null)
            return false;

        if (itemComponent.IsContainsItem(GetGunSkill._reloadData.BulletMagazineIndex))
            return true;

        if (_bulletMag != null)
        {
            if (_bulletMag.IsPossibleReload(actor))
                return true;
        }

        UIManager.Instance.FieldUI.SetFieldText(actor, 9004);
        return false;
    }
    public void Reload(IActor actor)
    {
        actor.StartCoroutine(IEReload(actor));
    }
    IEnumerator IEReload(IActor actor)
    {
        UIManager.Instance.FieldUI.SetFieldText(actor, 9003);
        ItemComponent itemComponent = actor.GetComponent<ItemComponent>(eComponent.ItemComponent);
        if (itemComponent == null)
            yield break;

        bool isSuccess = true;
        actor.Controller.SetBlockState(eActionBlockType.Action, GetGunSkill._reloadData.ReloadTime); 
        float elapsedTime = 0f;
        while((elapsedTime += TimeManager.DeltaTime) < GetGunSkill._reloadData.ReloadTime)
        {
            yield return null;

            if (!actor.IsAlive)
            {
                isSuccess = false;
                break;
            }

            if (itemComponent.SelectItem != this)
            {
                UIManager.Instance.FieldUI.SetFieldText(actor, 9004);
                isSuccess = false;
                break;
            }
        }

        if(isSuccess)
        {
            if (itemComponent.IsContainsItem(GetGunSkill._reloadData.BulletMagazineIndex))
            {
                Item item = itemComponent.PopItem(GetGunSkill._reloadData.BulletMagazineIndex);
                if(item != null)
                {
                    BulletMagazineItem bulletMagazine = item as BulletMagazineItem;
                    if(_bulletMag != null)
                        itemComponent.PushItem(bulletMagazine);

                    _bulletMag = bulletMagazine;
                    UIManager.Instance.Open<UBulletMagazineUI>(eUIName.UBulletMagazineUI).SetReloadable(_bulletMag);
                }
                UIManager.Instance.FieldUI.SetFieldText(actor, 9005);
            }
            else
            {
                if (_bulletMag != null)
                {
                    if (_bulletMag.IsPossibleReload(actor))
                        _bulletMag.Reload(actor);
                }
                else
                {
                    UIManager.Instance.FieldUI.SetFieldText(actor, 9006);
                }
            }
        }
        else
        {
            actor.Controller.SetBlockState(eActionBlockType.Action, 0f);
        }
    }
}
