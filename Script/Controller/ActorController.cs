using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

[System.Serializable]
public abstract class ActorController
{
    protected DynamicActor _owner;
    public DynamicActor Owner => _owner;
    protected AgentController _agentController;
    AIPath _agent;
    public eMoveType MoveType = eMoveType.Run;
    public virtual eFSMState State { get; set; }
    protected IActor _target;
    public IActor Target
    {
        get => _target;
        set => _target = value;
    }
    public bool IsBlockType(eActionBlockType type) => (_actionBlockType & type) != 0;
    eActionBlockType _actionBlockType;
    float _actionBlockTime;
    public void SetBlockState(eActionBlockType blockType, float time)
    {
        _actionBlockType = blockType;
        _actionBlockTime = time;
    }
    public ActorController(DynamicActor owner)
    {
        _owner = owner;
        _agent = owner.GetComponent<AIPath>();
    }
    public virtual void MovePosition(Vector2 direction)
    {
        float speed = 0f;
        switch (MoveType)
        {
            case eMoveType.Walk: 
                speed = _owner.ActorStat[eStatusType.WalkSpeed]; 
                break;
            case eMoveType.Run:
                speed = _owner.ActorStat[eStatusType.RunSpeed]; 
                break;
            case eMoveType.Sprint:
                speed = _owner.ActorStat[eStatusType.RunSpeed] * 2f;
                break;
        }
        Vector3 nextPos =(Vector2)_owner.transform.position + direction * speed * _owner.Runner.DeltaTime;
        _owner.transform.position = nextPos;
    }
    public void LookAtTarget()
    {
        if (_owner != null && _target != null && !IsBlockType(eActionBlockType.Direction)) 
            _owner.Direction = (_target.Position - _owner.Position).normalized;
    }
    public virtual void NetworkUpdate(float deltaTime)
    {
        _owner.Animator?.Animator.SetInteger(ClientConst.KEY_ANIMATION_MOVETYPE, (int)MoveType);
    }
    void OnUpdateBlockTime(float deltaTime)
    {
        if (_actionBlockType != eActionBlockType.None)
        {
            _actionBlockTime -= deltaTime;
            if (_actionBlockTime < 0f)
                _actionBlockType = eActionBlockType.None;
        }
    }
    public virtual void Update(float deltaTime)
    {
        OnUpdateBlockTime(deltaTime);
    }
    public virtual void Terminate()
    {
        if (_agentController != null)
            _agentController.Stop();
    }
}
