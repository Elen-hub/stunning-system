using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class BaseEffect : MonoBehaviour
{
    [SerializeField] protected ParticleSystem[] _particleArr;
    //ParticleSystem.MainModule[] _mainModuleArr;
    public Vector2 Position => transform.position;
    IActor _trackingActor;
    eAttachmentTarget _trackingTarget;
    #region Default Varialbe
    [SerializeField] protected ulong _worldID;
    public ulong GetWorldID => _worldID;
    protected Data.EffectData Data => DataManager.Instance.EffectTable[_index];
    int _index;
    public int GetIndex => _index;
    protected float _elapsedTime;
    protected float _targetTime;
    public float GetTargetTime => _targetTime;
    #endregion
    #region Effect Method
    public virtual BaseEffect Initialize(int index)
    {
        _index = index;
        GameObject[] _childEffect;
        if (transform.childCount != 1)
        {
            _childEffect = new GameObject[transform.childCount];
            for (int i = 0; i < _childEffect.Length; ++i)
                _childEffect[i] = transform.GetChild(i).gameObject;
        }
        //if(_particleArr != null)
        //{
        //    _mainModuleArr = new ParticleSystem.MainModule[_particleArr.Length];
        //    for (int i = 0; i < _particleArr.Length; ++i)
        //        _mainModuleArr[i] = _particleArr[i].main;
        //}
        gameObject.SetActive(false);
        return this;
    }
    // 기본 이펙트 출력은 owner가 null이어도 상관없다.
    public virtual void Enabled(ulong worldID, IActor trackingActor, eAttachmentTarget trackingTarget, float durationTime)
    {
        _worldID = worldID;
        _trackingActor = trackingActor;
        _trackingTarget = trackingTarget;
        _elapsedTime = 0;

        _targetTime = durationTime;
        if(_trackingActor.Attachment != null)
        {
            AttachmentElement attachmentElement = _trackingActor.Attachment.GetAttachmentElement(trackingTarget);
            if (attachmentElement != null)
            {
                transform.SetParent(attachmentElement.Transform);
                transform.localPosition = Vector3.zero;
            }
        }
        //if (_particle != null)
        //{
        //    ParticleSystem.MainModule module = _particle.main;
        //    module.simulationSpeed = _data.PlayRate;
        //}
        gameObject.SetActive(true);
    }
    public virtual void Enabled(ulong worldID, Vector3 position, Vector2 direction, float durationTime)
    {
        _worldID = worldID;
        transform.position = position;
        transform.eulerAngles = new Vector3(0, 0, ClientMath.GetAngleFromAxis(direction));
        _elapsedTime = 0;

        _targetTime = durationTime;
        //if (_particle != null)
        //{
        //    ParticleSystem.MainModule module = _particle.main;
        //    module.simulationSpeed = _data.PlayRate;
        //}
        gameObject.SetActive(true);
    }
    public virtual void Disabled()
    {
        if (_trackingActor != null)
        {
            transform.SetParent(EffectManager.Instance.transform);
            _trackingActor = null;
        }
        EffectManager.Instance.RegistObjectMemoryPool = this;
        gameObject.SetActive(false);
    }
    public void ResetTime()
    {
        _elapsedTime = 0f;
        for (int i = 0; i < _particleArr.Length; ++i)
            _particleArr[i].time = 0f;
    }
    public void Stop()
    {
        for (int i = 0; i < _particleArr.Length; ++i)
            _particleArr[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    #endregion
    #region Unity API
    protected virtual void Update()
    {
        if (_targetTime != 0)
        {
            _elapsedTime += TimeManager.DeltaTime;
            if (_targetTime < _elapsedTime)
            {
                if (_worldID != 0)
                    EffectManager.Instance.RemoveEffect(_worldID);
                else
                    Disabled();
            }
        }
    }
    #endregion
}