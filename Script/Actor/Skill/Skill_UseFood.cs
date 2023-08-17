using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Skill_UseFood : BaseSkill
{
    #region Data definitions
    public struct RecoveryData
    {
        public float HPRecoveryAmount;
        public float HPRecoveryRate;
        public float SPRecoveryAmount;
        public float SPRecoveryRate;
        public float HungerRecoveryAmount;
        public float HungerRecoveryRate;
        public float StressRecoveryAmount;
        public float StressRecoveryRate;
        public float SleepyRecoveryAmount;
        public float SleepyRecoveryRate;
    }
    #endregion
    [JsonRequired]
    protected RecoveryData _recoveryData;
    protected override void OnProcessInformationText()
    {
        base.OnProcessInformationText();

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        if (_recoveryData.HPRecoveryAmount != 0)
            stringBuilder.Append(LocalizingManager.Instance.GetLocalizing(8009, _recoveryData.HPRecoveryAmount));
        if (_recoveryData.HPRecoveryRate != 0)
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8010, _recoveryData.HPRecoveryRate));
        if (_recoveryData.SPRecoveryAmount != 0)
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8011, _recoveryData.SPRecoveryAmount));
        if (_recoveryData.SPRecoveryRate != 0)
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8012, _recoveryData.SPRecoveryRate));
        if (_recoveryData.HungerRecoveryAmount != 0)
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8013, _recoveryData.HungerRecoveryAmount));
        if (_recoveryData.HungerRecoveryRate != 0)
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8014, _recoveryData.HungerRecoveryRate));
        if (_recoveryData.StressRecoveryAmount != 0)
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8015, _recoveryData.StressRecoveryAmount));
        if (_recoveryData.StressRecoveryRate != 0)
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8016, _recoveryData.StressRecoveryRate));
        if (_recoveryData.SleepyRecoveryAmount != 0)
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8017, _recoveryData.SleepyRecoveryAmount));
        if (_recoveryData.SleepyRecoveryRate != 0)
            stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8018, _recoveryData.SleepyRecoveryRate));

        _informationText = stringBuilder.ToString();
    }
    public override bool IsUseable(IActor caster)
    {
        if (caster.Controller.IsBlockType(eActionBlockType.Action))
            return false;

        if (!base.IsUseable(caster))
            return false;

        if(_recoveryData.HPRecoveryAmount > 0f || _recoveryData.HPRecoveryRate >0f)
        {
            if (caster.ActorStat[eStatusType.HP] > caster.CurrentHP)
                return true;
        }
        Character character = caster as Character;
        if (character)
        {
            if (_recoveryData.SPRecoveryAmount > 0f || _recoveryData.SPRecoveryRate > 0f)
            {
                if (caster.ActorStat[eStatusType.SP] > character.CurrentSP)
                    return true;
            }
            if (_recoveryData.HungerRecoveryAmount > 0f || _recoveryData.HungerRecoveryRate > 0f)
            {
                if (caster.ActorStat[eStatusType.Hunger] > character.CurrentHunger)
                    return true;
            }
            if (_recoveryData.StressRecoveryAmount > 0f || _recoveryData.StressRecoveryRate > 0f)
            {
                if (caster.ActorStat[eStatusType.Stress] > character.CurrentStress)
                    return true;
            }
            if (_recoveryData.SleepyRecoveryAmount > 0f || _recoveryData.SleepyRecoveryRate > 0f)
            {
                if (caster.ActorStat[eStatusType.Sleepy] > character.CurrentSleepy)
                    return true;
            }
        }
        return false;
    }
    protected override IEnumerator IEStartSequenceProcess(IActor caster, Vector2 direction)
    {
        caster.Controller.SetBlockState(eActionBlockType.Action, _defaultData.ReadyTime);
        yield return CoroutineUtility.Wait(_defaultData.ReadyTime);

        // caster.Controller.LookAtTarget();
#if UNITY_SERVER
        if (_recoveryData.HPRecoveryAmount > 0f || _recoveryData.HPRecoveryRate > 0f)
            caster.CurrentHP = Mathf.Clamp(caster.CurrentHP + _recoveryData.HPRecoveryAmount + _recoveryData.HPRecoveryRate * caster.ActorStat[eStatusType.HP], 0f, caster.ActorStat[eStatusType.HP]);

        Character character = caster as Character;
        if (character)
        {
            if (_recoveryData.SPRecoveryAmount > 0f || _recoveryData.SPRecoveryRate > 0f)
                character.CurrentSP = Mathf.Clamp(character.CurrentSP + _recoveryData.SPRecoveryAmount + _recoveryData.SPRecoveryRate * caster.ActorStat[eStatusType.SP], 0f, caster.ActorStat[eStatusType.SP]);

            if (_recoveryData.HungerRecoveryAmount > 0f || _recoveryData.HungerRecoveryRate > 0f)
                character.CurrentHunger = Mathf.Clamp(character.CurrentHunger + _recoveryData.HungerRecoveryAmount + _recoveryData.HungerRecoveryRate * caster.ActorStat[eStatusType.Hunger], 0f, caster.ActorStat[eStatusType.Hunger]);

            if (_recoveryData.StressRecoveryAmount > 0f || _recoveryData.StressRecoveryRate > 0f)
                character.CurrentStress = Mathf.Clamp(character.CurrentStress + _recoveryData.StressRecoveryAmount + _recoveryData.StressRecoveryRate * caster.ActorStat[eStatusType.Stress], 0f, caster.ActorStat[eStatusType.Stress]);

            if (_recoveryData.SleepyRecoveryAmount > 0f || _recoveryData.SleepyRecoveryRate > 0f)
                character.CurrentSleepy = Mathf.Clamp(character.CurrentSleepy + _recoveryData.SleepyRecoveryAmount + _recoveryData.SleepyRecoveryRate * caster.ActorStat[eStatusType.Sleepy], 0f, caster.ActorStat[eStatusType.Sleepy]);
        }
#endif
        yield break;
    }
#if UNITY_EDITOR
    public override void OnGUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList)
    {
        base.OnGUIEditor(maxWidth, ref height, ref errorList);

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "HP Recovery Amount");
        _recoveryData.HPRecoveryAmount = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.HPRecoveryAmount);
        height += 20f;
        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "HP Recovery Rate");
        _recoveryData.HPRecoveryRate = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.HPRecoveryRate);
        height += 30f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "SP Recovery Amount");
        _recoveryData.SPRecoveryAmount = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.SPRecoveryAmount);
        height += 20f;
        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "SP Recovery Rate");
        _recoveryData.SPRecoveryRate = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.SPRecoveryRate);
        height += 30f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Hunger Recovery Amount");
        _recoveryData.HungerRecoveryAmount = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.HungerRecoveryAmount);
        height += 20f;
        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Hunger Recovery Rate");
        _recoveryData.HungerRecoveryRate = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.HungerRecoveryRate);
        height += 30f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Stress Recovery Amount");
        _recoveryData.StressRecoveryAmount = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.StressRecoveryAmount);
        height += 20f;
        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Stress Recovery Rate");
        _recoveryData.StressRecoveryRate = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.StressRecoveryRate);
        height += 30f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Sleepy Recovery Amount");
        _recoveryData.SleepyRecoveryAmount = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.SleepyRecoveryAmount);
        height += 20f;
        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Sleepy Recovery Rate");
        _recoveryData.SleepyRecoveryRate = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _recoveryData.SleepyRecoveryRate);
        height += 30f;
    }
    public override void SetDefaultData()
    {
        base.SetDefaultData();

        _recoveryData = new RecoveryData();
    }
#endif
}
