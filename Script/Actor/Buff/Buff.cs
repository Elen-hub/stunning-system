using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    #region Instance
    static Dictionary<int, MemoryPool<Buff>> _memoryPool = new Dictionary<int, MemoryPool<Buff>>();
    public static Buff NewBuff(int index)
    {
        if (!_memoryPool.ContainsKey(index))
            _memoryPool.Add(index, new MemoryPool<Buff>(10));

        Buff buff = _memoryPool[index].GetItem();
        if (buff != null)
            return buff;

        Data.BuffData buffData = DataManager.Instance.BuffTable[index];
        if (buffData.IsStatusType)
        {
            StatusBuff statusBuff = new StatusBuff()
            {
                _index = index,
                StatusType = buffData.StatusType,
                _isInfinity = buffData.TargetTime <= 0f,
            };
            return statusBuff;
        }
        else
        {
            EventBuff eventBuff = new EventBuff()
            {
                _index = index,
                _isInfinity = buffData.TargetTime <= 0f,
            };
            return eventBuff;
        }
    }
    #endregion
    int _index;
    public int Index => _index;
    public Data.BuffData Data => DataManager.Instance.BuffTable[_index];
    public bool IsTracking => Data.Icon != null;
    uint _buffKey;
    public uint BuffKey => _buffKey;
    float _elapsedTime;
    public float ElapsedTime => _elapsedTime;
    bool _isInfinity;
    public float GetProgress => _isInfinity ? 0 : _elapsedTime / Data.TargetTime;
    protected IActor _actor;
    BaseEffect _trackingEffect;
    public event System.Action OnDisableCallback;
    public void Start(IActor actor, uint buffKey)
    {
        _buffKey = buffKey;
        _actor = actor;
        _elapsedTime = 0f;
        OnEvent(true);
        OnSpawnTrackingEffect();
    }
    void OnSpawnTrackingEffect()
    {
#if !UNITY_SERVER
        if (Data.EffectIndex != 0)
        {
            if (_trackingEffect != null)
                _trackingEffect.ResetTime();
            else
                _trackingEffect = EffectManager.Instance.SpawnEffect(0, Data.EffectIndex, _actor, Data.AttachmentTarget, Data.TargetTime - _elapsedTime);
        }
#endif
    }
    public bool IsEnd => _elapsedTime > Data.TargetTime;
    public void End()
    {
        OnEvent(false);
        _memoryPool[_index].Register(this);
#if !UNITY_SERVER
        OnEvent(false);
        if (_trackingEffect != null)
        {
            _trackingEffect.Disabled();
            _trackingEffect = null;
        }
        OnDisableCallback?.Invoke();
#endif
    }
    protected abstract void OnEvent(bool isActive);
    public void Update(float deltaTime)
    {
        OnUpdateDurationTime(deltaTime);
    }
    public virtual void ResetTime()
    {
        _elapsedTime = 0f;
        OnSpawnTrackingEffect();
    }
    protected void OnUpdateDurationTime(float deltaTime)
    {
        if (_isInfinity)
            return;

        _elapsedTime += deltaTime;
        if(IsEnd)
            End();
    }
}
