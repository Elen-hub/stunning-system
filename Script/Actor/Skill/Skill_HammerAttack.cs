using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Skill_HammerAttack : BaseSkill
{
    #region Data definitions
    public struct CombatData
    {
        public float Damage;
        public float MinRadius;
        public float MaxRadius;
        public float Angle;
    }
    #endregion
    [JsonRequired]
    protected CombatData _combatData;
    public override bool IsUseable(IActor caster)
    {
        if (caster.Controller.IsBlockType(eActionBlockType.Action))
            return false;

        return base.IsUseable(caster);
    }
    protected override IEnumerator IEStartSequenceProcess(IActor caster, Vector2 direction)
    {
        caster.Controller.LookAtTarget();
        // _monster.Animator.SetTrigger("AttackTrigger");
        caster.Controller.SetBlockState(eActionBlockType.Action | eActionBlockType.Move | eActionBlockType.Direction, _defaultData.DurationTime);
        caster.Controller.State = eFSMState.Battle;
        yield return CoroutineUtility.Wait(_defaultData.ReadyTime);
        OnAttack(caster);
        caster.Controller.State = eFSMState.Chase;
    }
    void OnAttack(IActor caster)
    {
        OverlapCicleArc(caster.Position, _combatData.MaxRadius, 1 << LayerMask.NameToLayer("Hitbox"));
        // OverlapRectangle(caster.Position + caster.Direction * _defaultData.OverlapExtent.y * 0.5f, _defaultData.OverlapExtent, ClientMath.GetAngleFromAxis(caster.Direction), 0f, 1 << LayerMask.NameToLayer("Hitbox"));
        while (_targetQueue.Count > 0)
        {
            TargetProperty targetProperty = _targetQueue.Dequeue();
            if (caster.GetAllyType(targetProperty.Actor.AllyNumber) == eAllyType.Ally)
                continue;

            NetworkManager.Instance.ActorEventSender.RequestEnqueueAttackProperty(eDamageType.Default, caster.WorldID, targetProperty.Actor.WorldID, false, _combatData.Damage);
            SpawnEffect(targetProperty.Actor, targetProperty.Actor.Position, targetProperty.Actor.Position - caster.Position, 2f);
        }
    }
#if UNITY_EDITOR
    public override void OnGUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList)
    {
        base.OnGUIEditor(maxWidth, ref height, ref errorList);

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Damage:");
        _combatData.Damage = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.Damage);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "MinRadius:");
        _combatData.MinRadius = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.MinRadius);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "MaxRadius:");
        _combatData.MaxRadius = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.MaxRadius);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Angle:");
        _combatData.Angle = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.Angle);
        height += 20f;
    }
    public override void SetDefaultData()
    {
        base.SetDefaultData();

        _combatData = new CombatData();
    }
#endif
}
