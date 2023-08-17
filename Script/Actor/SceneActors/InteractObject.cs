using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractObject : StaticActor, IInteractable
{
    InteractCollision _interactCollision;
    public eInteractState InteractState { get; set; }
    public Transform FollowInfoTarget { get; }
    public event System.Action OnDisableInfoEvent;
    protected IActor _interactCaster;
    int _colliderCount = 5;
    RaycastHit2D[] _raycastHitArr = new RaycastHit2D[10];
    public override void Initialize(int index)
    {
        base.Initialize(index);

        _interactCollision = transform.Find("InteractCollider").GetComponent<InteractCollision>();
    }
    public override void OnSpawnClient(BaseEventReceiver eventReceiver)
    {
        base.OnSpawnClient(eventReceiver);
    }
    public override void OnSpawnServer(ulong worldID)
    {
        base.OnSpawnServer(worldID);
    }
    protected override void OnStartInstallMode()
    {
        base.OnStartInstallMode();

        _interactCollision.Collider.enabled = false;
    }
    protected override void OnEndInstallMode()
    {
        base.OnEndInstallMode();

        _interactCollision.Collider.enabled = true;
    }
    public void NotifyRegistProximate()
    {
        SendComponentMessage(eComponentEvent.ActivateOutline, true);
    }
    public void NotifyRemoveProximate()
    {
        SendComponentMessage(eComponentEvent.ActivateOutline, false);
    }
    void OnDistanceUpdate()
    {
        if (_interactCaster == null)
            return;

        int count = Physics2D.RaycastNonAlloc(_interactCaster.Position,  Position - _interactCaster.Position, _raycastHitArr, 3f, LayerMask.GetMask("Interact"));
        for(int i = 0; i<count; ++i)
        {
            if (_raycastHitArr[i].collider == _interactCollision.Collider)
                return;
        }
        InteractExit();
        NetworkManager.Instance.ActorEventSender.NotifyInteractEnd(WorldID);
    }
    public virtual bool IsPossibleInteract(IActor caster)
    {
        if (InteractState != eInteractState.Standby)
        {
            UIManager.Instance.FieldUI.SetFieldText(caster, 9002);
            return false;
        }
        return IsAlive;
    }
    public void Interact(IActor caster)
    {
        _interactCaster = caster;
        OnInteractStart(caster);
        InteractState = eInteractState.Interacting;
#if !UNITY_SERVER
        if (caster.Equals(PlayerManager.Instance.Me.Character))
        {
            OnClientInteractStart(caster);
            caster.SendComponentMessage(eComponentEvent.Interact, this);
        }
        SendComponentMessage(eComponentEvent.Interact, eObjectRenderState.Interact);
#endif
    }
    protected virtual void OnInteractStart(IActor caster)
    {

    }
    protected virtual void OnInteractEnd()
    {
        InteractState = eInteractState.Standby;
    }
    protected virtual void OnClientInteractStart(IActor caster)
    {

    }
    public void InteractExit()
    {
        if (_interactCaster == null)
            return;

        OnInteractEnd();
#if !UNITY_SERVER

        if (_interactCaster.Equals(PlayerManager.Instance.Me.Character))
        {
            OnClientInteractEnd();
            _interactCaster.SendComponentMessage(eComponentEvent.InteractEnd);
        }
#endif
        SendComponentMessage(eComponentEvent.Interact, eObjectRenderState.Idle);
        _interactCaster = null;
    }
    protected virtual void OnClientInteractEnd()
    {
        
    }
    protected override void Update()
    {
        base.Update();

#if UNITY_SERVER
        OnDistanceUpdate();
#endif
    }
}
