using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public enum ENavigationState
{
    Blank,
    Calculating,
    Running,
    Fail,
    Finish,
}
public class AgentController
{
    static Vector2 _maxVector = new Vector3(float.MaxValue, float.MaxValue);
    DynamicActor _owner;
    Seeker _seeker;
    AIPath _aiPath;
    float _stoppingDistance;
    public float SetStoppingDistance { set
        {
            _stoppingDistance = value * value;
            _aiPath.endReachedDistance = value;
        }
    }
    public AIPath Agent => _aiPath;
    public float SetSpeed { set => _aiPath.maxSpeed = value; }
    Vector2 _destination = _maxVector;
    public Vector3 Velocity => _aiPath.velocity;
    Vector2 _direction;
    public Vector2 Direction => _direction;
    public AgentController(DynamicActor owner)
    {
        _owner = owner;
        _seeker = _owner.GetComponent<Seeker>();
        _aiPath = _owner.GetComponent<AIPath>();
    }
    public bool SetDestination(Vector2 destination)
    {
        if (_destination == destination)
            return true;

        if ((_owner.Position - destination).sqrMagnitude <= _stoppingDistance)
        {
            Stop();
            return false;
        }

        if (_aiPath.isStopped)
            _aiPath.isStopped = false;

        _destination = destination;
        _aiPath.destination = destination;
        return true;
    }
    public void Stop()
    {
        _destination = _maxVector;
        if(_aiPath.hasPath)
            _aiPath.SetPath(null);

        _aiPath.isStopped = true;
    }
    public void MoveableUpdate()
    {
        if (_aiPath.isStopped)
            return;

        if ((_owner.Position - _destination).sqrMagnitude <= _stoppingDistance)
            Stop();
    }
}