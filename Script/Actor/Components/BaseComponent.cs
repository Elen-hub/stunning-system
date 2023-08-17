using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseComponent
{
    protected eComponent _componentType;
    public eComponent ComponentType => _componentType;
    protected bool _isActivate;
    public virtual bool Activate { get => _isActivate; set => _isActivate = value; }
    protected IActor _owner;
    public IActor Owner => _owner;
    public BaseComponent(IActor master, eComponent componentType)
    {
        _isActivate = true;
        _owner = master;
        _componentType = componentType;
        _owner.AddComponent(this);
    }
    #region Component Event message
    Dictionary<eComponentEvent, UnityAction<object[]>> _eventMethodPair = new Dictionary<eComponentEvent, UnityAction<object[]>>();
    protected void AddEventMethods(eComponentEvent componentEvent, UnityAction<object[]> action)
    {
        _eventMethodPair.Add(componentEvent, action);
        _owner.AddComponentEvent(componentEvent, this);
    }
    public void ReceiveComponentMessage(eComponentEvent eventType, params object[] messageArr)
    {
        if (_eventMethodPair == null) return;
        if (_eventMethodPair.ContainsKey(eventType))
            _eventMethodPair[eventType](messageArr);
    }
    #endregion
    public void NextFrame(float deltaTime)
    {
        if (_isActivate)
        {
            OnUpdate(deltaTime);
#if !UNITY_SERVER
            OnClientUpdate(deltaTime);
#endif
        }
    }
    public void Reset() => OnReset();
    public void Cleanup() => OnCleanup();
    protected virtual void OnReset() { }
    protected virtual void OnCleanup() { }
    public virtual void OnTerminate() { }
    protected virtual void OnUpdate(float deltaTime) { }
    protected virtual void OnClientUpdate(float deltaTime) { }
    public static implicit operator bool(BaseComponent a)
    {
        return a != null;
    }
}