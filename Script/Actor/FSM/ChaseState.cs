using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : BaseState
{
    public ChaseState(FSMController controller, DynamicActor owner, int fsmConfigureIndex) : base(controller, owner, fsmConfigureIndex)
    {

    }
    public override void OnStateEnter()
    {
        _fsmController.MoveType = eMoveType.Run;
    }
    public override void OnStateStay(float deltaTime)
    {
        if (ChaseStateCheck())
            return;

        if(_fsmController.Target == null)
        {
            _fsmController.State = eFSMState.Idle;
            return;
        }
        //if (!_fsmController.Target.IsExist)
        //{
        //    _fsmController.State = eFSMState.Idle;
        //    _fsmController.Target = null;
        //    return;
        //}
        if (!_fsmController.Target.IsAlive)
        {
            IActor actor = _fsmController.SearchHandler.FindActor();
            if (actor == null)
                return;
            
            _fsmController.Target = actor;
        }
        if (_fsmController.UseMatchConditionSkill(_fsmController.Target))
            return;

        _fsmController.MovePosition(_fsmController.Target.Position);
    }
    public override void OnStateExit()
    {
        _fsmController.Agent.Stop();
    }
    bool ChaseStateCheck()
    {
        if (IsOverLimitRadius)
        {
            _fsmController.State = eFSMState.Comback;
            return true;
        }
        return false;
    }
}
