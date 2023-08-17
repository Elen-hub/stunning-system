using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState
{
    const float _limitRadiusSqr = 1600f;
    Vector2 _spawnPosition;
    protected DynamicActor _owner;
    protected FSMController _fsmController; 
    float _stopDistance = 2f * 2f;
    public float StopDistance => _stopDistance;
    protected bool IsOverLimitRadius => (_owner.Position - _spawnPosition).sqrMagnitude > _limitRadiusSqr;
    protected Data.FSMConfigureData FSMConfigureData => DataManager.Instance.FSMConfigureTable[_fsmConfigureIndex];
    int _fsmConfigureIndex;
    public BaseState(FSMController controller, DynamicActor owner, int fsmConfigureIndex)
    {
        _owner = owner;
        _fsmController = controller;
        _fsmConfigureIndex = fsmConfigureIndex;
    }
    public abstract void OnStateEnter();
    public abstract void OnStateStay(float deltaTime);
    public abstract void OnStateExit();
}