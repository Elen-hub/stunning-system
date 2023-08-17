using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class Skill_Gun : BaseSkill
{
    public struct CombatData
    {
        public float Damage;
        public float Range;
        public float Speed;
        public float HorizontalRecoil;
        public int PenetrateCount;
        public int BulletEffectIndex;
    }
    public struct ReloadData
    {
        public int BulletMagazineIndex;
        public float ReloadTime;
    }
    [JsonRequired]
    public CombatData _combatData;
    [JsonRequired]
    public ReloadData _reloadData;
    LayerMask _layerMask = LayerMask.GetMask("Hitbox", "House");
    protected override void OnProcessInformationText()
    {
        base.OnProcessInformationText();

        System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
        stringBuilder.Append(LocalizingManager.Instance.GetLocalizing(8002, _combatData.Damage));
        stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8003, _combatData.Range));
        stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8004, _combatData.Speed / 60f));
        stringBuilder.Append("\n").Append(LocalizingManager.Instance.GetLocalizing(8005, _combatData.PenetrateCount));
        _informationText = stringBuilder.ToString();
    }
    public override bool IsUseable(IActor caster)
    {
        if (caster.Controller.IsBlockType(eActionBlockType.Action))
            return false;

        return base.IsUseable(caster);
    }
    protected override IEnumerator IEStartSequenceProcess(IActor caster, Vector2 direction)
    {
        caster.Controller.SetBlockState(eActionBlockType.Action, _defaultData.DurationTime);
        // _monster.Animator.SetTrigger("AttackTrigger");
        yield return CoroutineUtility.Wait(_defaultData.ReadyTime);
        OnAttack(caster, direction);
    }
    void OnAttack(IActor caster, Vector2 direction)
    {
        float rad = Random.Range(-_combatData.HorizontalRecoil, _combatData.HorizontalRecoil) * Mathf.Deg2Rad;
        Vector2 rotationTransform = new Vector2() {
            x = Mathf.Cos(rad) * direction.x + Mathf.Sin(rad) * direction.y,
            y = -Mathf.Sin(rad) * direction.x + Mathf.Cos(rad) * direction.y,
        };

        BaseEffect baseEffect = EffectManager.Instance.SpawnEffect(_combatData.BulletEffectIndex, caster.Position, direction + rotationTransform, _combatData.Range / _combatData.Speed, _combatData.Speed, _combatData.Range);
        caster.StartCoroutine(IEBullet(caster, baseEffect));
    }
    IEnumerator IEBullet(IActor caster, BaseEffect effect)
    {
        const string wallTag = "Wall";
        HashSet<Collider2D> hitHashColliderArr = new HashSet<Collider2D>();
        RaycastHit2D[] hitResultArr = new RaycastHit2D[_combatData.PenetrateCount];
        int colliderCount = _combatData.PenetrateCount;
        Vector2 prevPosition = effect.transform.position;
        while (effect.isActiveAndEnabled)
        {
            yield return null;
            Vector2 currPosition = effect.transform.position;
            Vector2 direction = currPosition - prevPosition;
            float distance = direction.sqrMagnitude;
            int raycastCount = Physics2D.RaycastNonAlloc(prevPosition, direction, hitResultArr, distance, _layerMask);
            if(raycastCount > 0)
            {
                System.Array.Clear(hitResultArr, raycastCount, hitResultArr.Length - raycastCount);
                System.Array.Sort(hitResultArr, Comparer);
            }
            for(int i = 0; i < raycastCount; ++i)
            {
                if (hitHashColliderArr.Contains(hitResultArr[i].collider))
                    continue;

                hitHashColliderArr.Add(hitResultArr[i].collider);
                if (hitResultArr[i].collider.tag.Equals(wallTag))
                {
                    RPCGenerator.Instance.RPC_RemoveEffect(effect.GetWorldID);
                    // EffectManager.Instance.RemoveEffect(effect.GetWorldID);
                    EffectManager.Instance.SpawnSkinEffect(eSkinEffectType.Wall, hitResultArr[i].point, hitResultArr[i].normal, 2f);
                    yield break;
                }

                IActor actor = hitResultArr[i].collider.GetComponentInParent<IActor>();
                if (actor == null)
                    continue;

                if (actor.GetAllyType(caster.AllyNumber) == eAllyType.Ally)
                    continue;

                --colliderCount;
                SpawnEffect(actor, hitResultArr[i].point, -hitResultArr[i].normal, 2f);
                NetworkManager.Instance.ActorEventSender.RequestEnqueueAttackProperty(eDamageType.Default, caster.WorldID, actor.WorldID, false, _combatData.Damage);
                if (colliderCount <= 0)
                {
                    RPCGenerator.Instance.RPC_RemoveEffect(effect.GetWorldID);
                    break;
                }
            }
            prevPosition = currPosition;
        }
    }
    int Comparer(RaycastHit2D a, RaycastHit2D b)
    {
        if (a.collider == null)
            return 1;

        if (b.collider == null)
            return -1;

        if (a.distance > b.distance)
            return 1;
        else
            return -1;
    }
#if UNITY_EDITOR
    public override void OnGUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList)
    {
        base.OnGUIEditor(maxWidth, ref height, ref errorList);

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Damage:");
        _combatData.Damage = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.Damage);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Range:");
        _combatData.Range = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.Range);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "Speed:");
        _combatData.Speed = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.Speed);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "HorizontalRecoil:");
        _combatData.HorizontalRecoil = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.HorizontalRecoil);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "PenetrateCount:");
        _combatData.PenetrateCount = UnityEditor.EditorGUI.IntField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.PenetrateCount);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "BulletEffectIndex:");
        _combatData.BulletEffectIndex = UnityEditor.EditorGUI.IntField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _combatData.BulletEffectIndex);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "BulletMagazineIndex:");
        _reloadData.BulletMagazineIndex = UnityEditor.EditorGUI.IntField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _reloadData.BulletMagazineIndex);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "ReloadTime:");
        _reloadData.ReloadTime = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), "", _reloadData.ReloadTime);
        height += 20f;
    }
    public override void SetDefaultData()
    {
        base.SetDefaultData();

        _combatData = new CombatData();
        _reloadData = new ReloadData();
    }
#endif
}
