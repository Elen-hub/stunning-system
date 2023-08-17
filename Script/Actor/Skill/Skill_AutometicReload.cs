using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Skill_AutometicReload : BaseSkill
{
    public struct CombatData
    {
        public int RequireItemIndex;
        public int MagCapacity;
        public float ReloadSpeed;
    }
    [JsonRequired]
    public CombatData _combatData;
    public override bool IsUseable(IActor caster)
    {
        if (caster.Controller.IsBlockType(eActionBlockType.Action))
            return false;

        ItemComponent itemComponent = caster.GetComponent<ItemComponent>(eComponent.ItemComponent);
        if (itemComponent == null)
            return false;

        BulletMagazineItem magzineItem = null;
        if (itemComponent.SelectItem.IsIncludeItemType(eItemType.Magazine))
        {
            magzineItem = itemComponent.SelectItem as BulletMagazineItem;
        }
        else
        {
            AutomaticGunItem gunItem = itemComponent.SelectItem as AutomaticGunItem;
            if (gunItem.BulletMagazineItem != null)
                magzineItem = gunItem.BulletMagazineItem;
        }
        if (magzineItem == null)
            return false;

        if (magzineItem.ReloadedCount >= _combatData.MagCapacity)
            return false;

        if (!itemComponent.IsContainsItem(_combatData.RequireItemIndex))
            return false;

        return base.IsUseable(caster);
    }
    protected override IEnumerator IEStartSequenceProcess(IActor caster, Vector2 direction)
    {
        // UIManager.Instance.FieldUI.SetFieldText(caster, )
        ItemComponent itemComponent = caster.GetComponent<ItemComponent>(eComponent.ItemComponent);
        Item matchItem = itemComponent.SelectItem;
        BulletMagazineItem magzineItem = null;
        if (itemComponent.SelectItem.IsIncludeItemType(eItemType.Magazine))
        {
            magzineItem = itemComponent.SelectItem as BulletMagazineItem;
        }
        else
        {
            AutomaticGunItem gunItem = itemComponent.SelectItem as AutomaticGunItem;
            if (gunItem.BulletMagazineItem != null)
                magzineItem = gunItem.BulletMagazineItem;
        }

        while (itemComponent.SelectItem == matchItem)
        {
            caster.Controller.SetBlockState(eActionBlockType.Action, _combatData.ReloadSpeed);
            yield return CoroutineUtility.Wait(_combatData.ReloadSpeed);
            if (!caster.IsAlive)
                break;

            if (itemComponent.SelectItem != matchItem)
            {
                UIManager.Instance.FieldUI.SetFieldText(caster, 9009);
                break;
            }
            if (magzineItem.ReloadedCount >= _combatData.MagCapacity)
            {
                UIManager.Instance.FieldUI.SetFieldText(caster, 9009);
                break;
            }
            if (!itemComponent.IsContainsItem(_combatData.RequireItemIndex))
            {
                UIManager.Instance.FieldUI.SetFieldText(caster, 9009);
                break;
            }
            Item item = itemComponent.PopItem(_combatData.RequireItemIndex, 1);
            if (item != null)
            {
                magzineItem.ReloadedCount += item.Count;
                UIManager.Instance.FieldUI.SetFieldText(caster, 9007, magzineItem.ReloadedCount, _combatData.MagCapacity);
            }
            if (magzineItem.ReloadedCount >= _combatData.MagCapacity)
            {
                UIManager.Instance.FieldUI.SetFieldText(caster, 9008);
                break;
            }
        }
        caster.Controller.SetBlockState(eActionBlockType.Action, 0f);
    }
#if UNITY_EDITOR
    public override void OnGUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList)
    {
        base.OnGUIEditor(maxWidth, ref height, ref errorList);

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "RequireItemIndex:");
        _combatData.RequireItemIndex = UnityEditor.EditorGUI.IntField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.RequireItemIndex);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "MagCapacity:");
        _combatData.MagCapacity = UnityEditor.EditorGUI.IntField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.MagCapacity);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "ReloadSpeed:");
        _combatData.ReloadSpeed = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.ReloadSpeed);
        height += 20f;
    }
    public override void SetDefaultData()
    {
        base.SetDefaultData();

        _combatData = new CombatData();
    }
#endif
}
