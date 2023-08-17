using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComponent 
{
    void AddComponent(BaseComponent component);
    void AddComponentEvent(eComponentEvent componentEvent, BaseComponent component);
    T GetComponent<T>(eComponent componentType) where T : BaseComponent;
    BaseComponent GetComponent(eComponent componentType);
    void SendComponentMessage(eComponentEvent eventType, params object[] messageArr);

}
