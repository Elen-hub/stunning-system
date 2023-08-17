using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class InteractComponent : BaseComponent
{
    static float _interactColliderRadius = 0.5f;
    LayerMask _interactLayer = 1 << LayerMask.NameToLayer("Interact");
    Collider2D[] _colliderArray = new Collider2D[10];

    HashSet<Collider2D> _interactHashSet = new HashSet<Collider2D>();
    List<InteractOverlapStructure> _interactObjectList = new List<InteractOverlapStructure>(10);
    IInteractable _interactObject;
    IInteractable _currentInteractObject;
    public InteractComponent(DynamicActor actor) : base(actor, eComponent.InteractComponent)
    {
        AddEventMethods(eComponentEvent.InteractInput, OnReceiveEventInteractInput);
        AddEventMethods(eComponentEvent.Interact, OnReceiveEventInteract);
        AddEventMethods(eComponentEvent.InteractEnd, OnReceiveEventInteractEnd);
    }
    void OnReceiveEventInteractInput(params object[] messageArr)
    {
        if (_currentInteractObject == null)
        {
            if (_interactObject != null)
            {
                if(_interactObject.IsPossibleInteract(_owner))
                    NetworkManager.Instance.ActorEventSender.RequestInteractStart(_owner.WorldID, _interactObject.WorldID);
            }
        }
        else
            NetworkManager.Instance.ActorEventSender.RequestInteractEnd(_currentInteractObject.WorldID);
    }
    void OnReceiveEventInteract(params object[] messageArr)
    {
        _currentInteractObject = (IInteractable)messageArr[0];
    }
    void OnReceiveEventInteractEnd(params object[] messageArr)
    {
        _currentInteractObject = null;
    }
    void CheckOverlapEnter(int size, Collider2D[] colArr)
    {
        for (int i = 0; i < size; ++i)
        {
            if (_interactHashSet.Contains(colArr[i]))
                continue;

            InteractCollision obj = colArr[i].GetComponent<InteractCollision>();
            if (obj == null)
                continue;

            // if (obj.Owner.IsPossibleInteract(_owner))
                RegistInteractList(new InteractOverlapStructure(colArr[i], obj.Owner));
        }
    }
    void RegistInteractList(in InteractOverlapStructure handler)
    {
        _interactHashSet.Add(handler.Collider);
        _interactObjectList.Add(handler);
    }
    void RemoveInteractList(int number)
    {
        // 삭제할 오브젝트가 가장 가까운 오브젝트였다면
        if (_interactObjectList[number].InteractObject == _interactObject)
        {
            _interactObject.NotifyRemoveProximate();
            _interactObject = null;
        }
        if(_interactObjectList[number].InteractObject == _currentInteractObject)
            NetworkManager.Instance.ActorEventSender.RequestInteractEnd(_currentInteractObject.WorldID);

        _interactHashSet.Remove(_interactObjectList[number].Collider);
        _interactObjectList.RemoveAt(number);
    }
    void SetProximateObject(IInteractable interactObject)
    {
        if (_interactObject == interactObject)
            return;

        if (_interactObject != null)
            _interactObject.NotifyRemoveProximate();

        _interactObject = interactObject;
        _interactObject.NotifyRegistProximate();
    }
    void CheckInteractList(int size, Collider2D[] colArr)
    {
        IInteractable interactObj = null;
        Vector3 pos = _owner.Position;
        Vector3 interactObjectColliderCenter = Vector3.zero;
        for (int i = _interactObjectList.Count - 1; i >= 0; --i)
        {
            if (!_interactObjectList[i].Collider.enabled)
            {
                RemoveInteractList(i);
                continue;
            }
            bool isFind = false;
            for (int j = 0; j < size; ++j)
            {
                if (_interactObjectList[i].Collider == colArr[j])
                {
                    isFind = true;
                    break;
                }
            }
            if (!isFind)
            {
                RemoveInteractList(i);
                continue;
            }
            if (_interactObjectList[i].InteractObject == _interactObject)
                interactObjectColliderCenter = _interactObjectList[i].Collider.bounds.center;

        }
        float nearDistance = float.MaxValue;
        if (_interactObject != null)
            nearDistance = (interactObjectColliderCenter - pos).sqrMagnitude;

        for (int i = 0; i < _interactObjectList.Count; ++i)
        {
            Vector3 colliderCenterPos = _interactObjectList[i].Collider.bounds.center;
            float distance = (pos - colliderCenterPos).sqrMagnitude;
            if (nearDistance > distance)
            {
                interactObj = _interactObjectList[i].InteractObject;
                nearDistance = distance;
            }
        }
        if (interactObj != null)
            SetProximateObject(interactObj);
    }
    protected override void OnClientUpdate(float deltaTime)
    {
        if (!_isActivate)
            return;

        int size = Physics2D.OverlapCircleNonAlloc(_owner.Position, _interactColliderRadius, _colliderArray, _interactLayer);
        CheckOverlapEnter(size, _colliderArray);
        CheckInteractList(size, _colliderArray);
    }
}
