using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : TSingletonMono<EffectManager>
{
    ulong _currentWorldID;
    Dictionary<ulong, BaseEffect> _effectDic = new Dictionary<ulong, BaseEffect>();
    Dictionary<int, ObjectMemoryPool<BaseEffect>> _ObjectMemoryPool = new Dictionary<int, ObjectMemoryPool<BaseEffect>>();
    Dictionary<eSkinEffectType, int> _skinEffectDictionary = new Dictionary<eSkinEffectType, int>() {
        { eSkinEffectType.Skin, 1001 },
        { eSkinEffectType.Wall, 1002 },
    };
    public BaseEffect RegistObjectMemoryPool
    {
        set {
            _ObjectMemoryPool[value.GetIndex].Register(value);
            if (_effectDic.ContainsKey(value.GetWorldID))
                _effectDic.Remove(value.GetWorldID);
        }
    }
    protected override void OnInitialize()
    {
        if (Application.isPlaying)
            _currentWorldID = (ulong)NetworkManager.Instance.Runner.LocalPlayer.PlayerId * 10000000000000000000L;
    }
    public BaseEffect SpawnEffect(int index, IActor actor, eAttachmentTarget attachmentTarget, float durationTime)
    {
        BaseEffect effect = GetEffect(index);
        ++_currentWorldID;

        effect.Enabled(_currentWorldID, actor, attachmentTarget, durationTime);
        _effectDic.Add(_currentWorldID, effect);
        RPCGenerator.Instance.RPC_SpawnEffect(_currentWorldID, index, actor.WorldID, attachmentTarget, durationTime);
        return effect;
    }
    public BaseEffect SpawnEffect(ulong worldID, int index, IActor actor, eAttachmentTarget attachmentTarget, float durationTime)
    {
        BaseEffect effect = GetEffect(index);

        effect.Enabled(worldID, actor, attachmentTarget, durationTime);
        if (worldID != 0)
            _effectDic.Add(worldID, effect);
        return effect;
    }
    public BaseEffect SpawnEffect(int index, Vector2 position, Vector2 direction, float durationTime)
    {
        BaseEffect effect = GetEffect(index);
        ++_currentWorldID;

        effect.Enabled(_currentWorldID, position, direction, durationTime);
        _effectDic.Add(_currentWorldID, effect);
        RPCGenerator.Instance.RPC_SpawnEffect(_currentWorldID, index, position, direction, durationTime);
        return effect;
    }
    public BaseEffect SpawnEffect(ulong worldID, int index, Vector2 position, Vector2 direction, float durationTime)
    {
        BaseEffect effect = GetEffect(index);

        effect.Enabled(worldID, position, direction, durationTime);
        _effectDic.Add(worldID, effect);
        return effect;
    }
    public BaseEffect SpawnEffect(int index, Vector2 position, Vector2 direction, float durationTime, float speed, float maxDistance)
    {
        BaseEffect effect = GetEffect(index);
        ++_currentWorldID;

        effect.Enabled(_currentWorldID, position, direction, durationTime);
        effect.StartCoroutine(CoroutineUtility.IESetThrust(effect.transform, direction, speed, maxDistance));
        _effectDic.Add(_currentWorldID, effect);
        RPCGenerator.Instance.RPC_SpawnEffect(_currentWorldID, index, position, direction, durationTime, speed, maxDistance);
        return effect;
    }
    public BaseEffect SpawnEffect(ulong worldID, int index, Vector2 position, Vector2 direction, float durationTime, float speed, float maxDistance)
    {
        BaseEffect effect = GetEffect(index);
        effect.Enabled(worldID, position, direction, durationTime);
        _effectDic.Add(worldID, effect);
        effect.StartCoroutine(CoroutineUtility.IESetThrust(effect.transform, direction, speed, maxDistance));
        return effect;
    }
    public BaseEffect SpawnSkinEffect(eSkinEffectType skinType, Vector2 position, Vector2 direction, float durationTime)
    {
        BaseEffect effect = GetEffect(_skinEffectDictionary[skinType]);
        ++_currentWorldID;

        effect.Enabled(_currentWorldID, position, direction, durationTime);
        RPCGenerator.Instance.RPC_SpawnEffect(_currentWorldID, _skinEffectDictionary[skinType], position, direction, durationTime);
        return effect;
    }
    public void RemoveEffect(ulong worldID)
    {
        if (_effectDic.ContainsKey(worldID))
            _effectDic[worldID].Disabled();
    }
    BaseEffect GetEffect(int index)
    {
        if (!_ObjectMemoryPool.ContainsKey(index))
            _ObjectMemoryPool.Add(index, new ObjectMemoryPool<BaseEffect>(30));

        BaseEffect effect = _ObjectMemoryPool[index].GetItem();
        if (effect == null)
            effect = Instantiate(Resources.Load<BaseEffect>(DataManager.Instance.EffectTable[index].ResourcePath), transform).Initialize(index);

        return effect;
    }
}