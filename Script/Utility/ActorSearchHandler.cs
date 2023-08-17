using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Flags]
public enum eActorSearchAttribute
{
    Ally = 1 << 0,
    Distance = 1 <<1,
    FSMState = 1 <<2,
    CullingWorldID = 1 << 3,
    FOV = 1 << 4,
}
public class ActorSearchHandler
{
    eActorSearchAttribute _attribute;
    eAllyType _allyType;

    IActor _owner;
    float _sqrDistance;
    float _fov;
    HashSet<ulong> _cullingIDHash = new HashSet<ulong>();
    Queue<IActor> _existActorQueue = new Queue<IActor>();
    public IActor SetActorEnqueue { set => _existActorQueue.Enqueue(value); }
    public ActorSearchHandler(IActor owner)
    {
        _owner = owner;
    }
    public bool IsExist => ActorManager.Instance.FindActor(this) != null;
    public IActor FindActor() => ActorManager.Instance.FindActor(this);
    public Queue<IActor> GetQueue(int count)
    {
        ActorManager.Instance.FindActor(count, this);
        return _existActorQueue;
    }
    public ActorSearchHandler AddConditionAllyType(eAllyType allyType)
    {
        if((_attribute & eActorSearchAttribute.Ally) == 0)
            _attribute |= eActorSearchAttribute.Ally;

        _allyType = allyType;
        return this;
    }
    public void DisableCondition(eActorSearchAttribute attribute)
    {
        if ((_attribute & attribute) == 0)
            return;

        _attribute ^= attribute;
        if ((attribute & eActorSearchAttribute.CullingWorldID) != 0)
            _cullingIDHash.Clear();
    }
    public ActorSearchHandler AddConditionDistance(float distance)
    {
        if ((_attribute & eActorSearchAttribute.Distance) == 0)
            _attribute |= eActorSearchAttribute.Distance;

        _sqrDistance = distance * distance;
        return this;
    }
    public ActorSearchHandler AddConditionCullingWorldID(uint worldID)
    {
        if ((_attribute & eActorSearchAttribute.CullingWorldID) == 0)
            _attribute |= eActorSearchAttribute.CullingWorldID;

        if (_cullingIDHash.Contains(worldID))
            _cullingIDHash.Add(worldID);

        return this;
    }
    public void AddInsideDot(float fov)
    {
        if ((_attribute & eActorSearchAttribute.FOV) == 0)
            _attribute |= eActorSearchAttribute.FOV;

        _fov = fov;
    }
    public bool CheckCondition(IActor actor)
    {
        if (!actor.IsAlive)
            return false;

        if((_attribute & eActorSearchAttribute.Ally) != 0)
        {
            if(_owner.GetAllyType(actor.AllyNumber) != _allyType)
                return false;
        }
        if ((_attribute & eActorSearchAttribute.Distance) != 0)
        {
            if ((_owner.Position - actor.Position).sqrMagnitude > _sqrDistance)
                return false;
        }
        if ((_attribute & eActorSearchAttribute.CullingWorldID) != 0)
            if (_cullingIDHash.Contains(actor.WorldID))
                return false;
        if((_attribute & eActorSearchAttribute.FOV) != 0)
        {
            float dotProduct = (1f - Vector3.Dot(_owner.Direction, actor.Position - _owner.Position)) * 180f;
            if (dotProduct > _fov)
                return false;
        }

        return true;
    }
}
