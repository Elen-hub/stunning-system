using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombackState : BaseState
{
    public CombackState(FSMController controller, DynamicActor owner, int fsmConfigureIndex) : base(controller, owner, fsmConfigureIndex)
    {

    }
    public override void OnStateEnter()
    {

    }
    public override void OnStateStay(float deltaTime)
    {
        Vector2 direction = (_owner.SpawnPosition - _owner.Position);
        float sqrDistance = direction.sqrMagnitude;
        if (sqrDistance < 1f)
        {
            _fsmController.State = eFSMState.Idle;
            return;
        }
        else
            _fsmController.MovePosition(_owner.SpawnPosition);
    }
    public override void OnStateExit()
    {

    }
}
