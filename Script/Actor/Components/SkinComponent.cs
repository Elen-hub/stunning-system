using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkinComponent : BaseComponent
{
    protected int _targetLayer;
    public SkinComponent(IActor actor) : base(actor, eComponent.SkinComponent)
    {
        AddEventMethods(eComponentEvent.SetLayerOrder, OnReceiveEventSetLayerOrder);
    }
    protected void OnReceiveEventSetLayerOrder(params object[] messageArr)
    {
        Vector2 position = (Vector2)messageArr[0];
        _targetLayer = ClientMath.GetLayerOrder(position);
        OnSetLayerOrder();
    }
    protected abstract void OnSetLayerOrder();
}
