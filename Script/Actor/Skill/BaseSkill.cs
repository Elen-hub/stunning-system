using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public abstract class BaseSkill
{
    #region Data definitions
    public struct DefaultData
    {
        public eSkillInputType SkillInputType;
        public float ReadyTime;
        public float DurationTime;
    }
    public string GetInformationText()
    {
        if (RuntimePreference.Preference.Language != _localizingType || _informationText == null)
            OnProcessInformationText();

        return _informationText;
    }
    protected virtual void OnProcessInformationText()
    {
        _localizingType = RuntimePreference.Preference.Language;
    }
    protected string _informationText;
    eLanguage _localizingType;
    [JsonRequired]
    protected DefaultData _defaultData;
    [JsonIgnore]
    public DefaultData GetDefaultData => _defaultData;
    protected void SpawnEffect(IActor target, Vector2 position, Vector2 direction, float durationTime)
    {
        switch (target.ActorType)
        {
            case eActorType.Character:
            case eActorType.Monster:
                EffectManager.Instance.SpawnSkinEffect(eSkinEffectType.Skin, position, direction, durationTime);
                break;
        }
    }
#if UNITY_EDITOR
    public virtual void OnGUIEditor(float maxWidth, ref float height, ref List<SkillEditorException> errorList)
    {
        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "SkillInputType:");
        _defaultData.SkillInputType = (eSkillInputType)UnityEditor.EditorGUI.EnumPopup(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _defaultData.SkillInputType);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "DurationTime:");
        _defaultData.DurationTime = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _defaultData.DurationTime);
        height += 20f;

        UnityEditor.EditorGUI.LabelField(new Rect(0f, height, maxWidth * 2 / 3f, 20f), "ReadyTime:");
        _defaultData.ReadyTime = UnityEditor.EditorGUI.FloatField(new Rect(maxWidth * 2 / 3f, height, maxWidth * 1 / 3f, 20f), _defaultData.ReadyTime);
        height += 30f;
    }
#endif
    #endregion
    [JsonIgnore]
    public Data.SkillData Data => DataManager.Instance.SkillTable[Index];
    [JsonIgnore]
    public int Index;
    public System.Type AssemblyType;
    public virtual bool IsUseable(IActor caster)
    {
        return caster.IsAlive;
    }
    public virtual void StartSequence(IActor caster, Vector2 direction)
    {
        caster.StartCoroutine(IEStartSequenceProcess(caster, direction));
    }
    protected abstract IEnumerator IEStartSequenceProcess(IActor caster, Vector2 direction);
    #region Collider Find Methods
    public readonly struct TargetProperty
    {
        public readonly Collider2D HitCollider;
        public readonly IActor Actor;
        public TargetProperty(Collider2D collider, IActor actor)
        {
            HitCollider = collider;
            Actor = actor;
        }
    }
    protected Queue<TargetProperty> _targetQueue = new Queue<TargetProperty>(30);
    protected Collider2D[] _colliderArr = new Collider2D[30];
    protected void OverlapRectangle(Vector2 pivot, Vector2 halfExtent, float angle, float minRange, int layerMask)
    {
        _targetQueue.Clear();
        int count = Physics2D.OverlapBoxNonAlloc(pivot, halfExtent, angle, _colliderArr, layerMask);
        for (int i = 0; i < count; ++i)
        {
            IActor actor = _colliderArr[i].GetComponentInParent<IActor>();
            if (actor == null)
                continue;

            if (minRange != 0 && IsContainsCircle(pivot, minRange, true, _colliderArr[i]))
                continue;

            _targetQueue.Enqueue(new TargetProperty(_colliderArr[i], actor));
        }
    }
    protected void OverlapCicleArc(Vector2 pivot, float radius, int layerMask)
    {
        _targetQueue.Clear();
        int count = Physics2D.OverlapCircleNonAlloc(pivot, radius, _colliderArr, layerMask);
        for (int i = 0; i < count; ++i)
        {
            DynamicActor actor = _colliderArr[i].GetComponentInParent<DynamicActor>();
            if (actor == null)
                continue;

            _targetQueue.Enqueue(new TargetProperty(_colliderArr[i], actor));
        }
    }
    protected void OverlapCicleArc(Vector2 pivot, float minRadius, float maxRadius, int layerMask)
    {
        _targetQueue.Clear();
        int count = Physics2D.OverlapCircleNonAlloc(pivot, maxRadius, _colliderArr, layerMask);
        for (int i = 0; i < count; ++i)
        {
            DynamicActor actor = _colliderArr[i].GetComponentInParent<DynamicActor>();
            if (actor == null)
                continue;

            _targetQueue.Enqueue(new TargetProperty(_colliderArr[i], actor));
        }
    }
    bool IsContainsCircle(Vector2 pivot, float radius, bool isCircleCollider, Collider2D col)
    {
        Bounds bound = col.bounds;
        if (isCircleCollider)
        {
            float radiusMagnitude = Mathf.Abs(radius - bound.extents.x);
            if ((pivot - (Vector2)bound.center).sqrMagnitude >= radiusMagnitude * radiusMagnitude)
                return false;
        }
        else
        {
            float radSqrMag = radius * radius;
            for (int x = -1; x <= 1; x += 2)
            {
                for (int z = -1; z <= 1; z += 2)
                {
                    float ax = bound.center.x + bound.extents.x * x;
                    float az = bound.center.y + bound.extents.y * z;
                    if ((pivot - new Vector2(ax, az)).sqrMagnitude > radSqrMag)
                        return false;
                }
            }
        }
        return true;
    }
    #endregion
#if UNITY_EDITOR
    #region Editor
    public static BaseSkill CreateNewSkillOnlyEditor(System.Reflection.Assembly assembly, int index)
    {
        string skillName = DataManager.Instance.SkillTable[index].ClassAssembly;
        System.Type type = assembly.GetType(skillName);
        if (type != null)
        {
            BaseSkill skill = System.Activator.CreateInstance(type) as BaseSkill;
            skill.Index = index;
            skill.AssemblyType = type;
            return skill;
        }

        return null;
    }
    public virtual void SetDefaultData()
    {
        _defaultData = new DefaultData();
    }
    #endregion
#endif
}
