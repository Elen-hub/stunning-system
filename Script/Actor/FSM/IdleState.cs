using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : BaseState
{
    bool _isDetect;
    Vector2 _detectPosition;
    float _limitSqrMag;
    public IdleState(FSMController controller, DynamicActor owner, int fsmConfigureIndex) : base(controller, owner, fsmConfigureIndex)
    {
        _limitSqrMag = FSMConfigureData.DetectSensorRadius * 0.2f * FSMConfigureData.DetectSensorRadius * 0.2f;
    }
    public override void OnStateEnter()
    {
        _isDetect = false;
        _fsmController.MoveType = eMoveType.Walk;
    }
    public override void OnStateStay(float deltaTime)
    {
        if (_isDetect)
        {
            IActor actor = FindActorIntoFovView();
            if (actor != null)
            {
                _fsmController.Target = actor;
                _fsmController.State = eFSMState.Chase;
                RPCGenerator.Instance.RPC_SetEmoticon(_owner.WorldID, eEmoticonType.ExclamationMark, eAttachmentTarget.OverHead, true, 1.5f);
            }
            else
            {
                Vector2 direction = (_detectPosition - _owner.Position);
                if (direction.sqrMagnitude > _limitSqrMag)
                    _fsmController.MovePosition(_detectPosition);
                else
                    _isDetect = false;
            }
        }
        else
        {
            if (IsDetectSensor())
                _isDetect = true;
        }
    }
    public override void OnStateExit()
    {

    }
    bool IsDetectSensor()
    {
        _fsmController.SearchHandler.DisableCondition(eActorSearchAttribute.FOV);
        _fsmController.SearchHandler.AddConditionDistance(FSMConfigureData.DetectSensorRadius);
        IActor actor = _fsmController.SearchHandler.FindActor();
        if (actor == null)
            return false;

        _detectPosition = actor.Position;
        RPCGenerator.Instance.RPC_SetEmoticon(_owner.WorldID, eEmoticonType.QuestionMark, eAttachmentTarget.OverHead, true, 2f);
        return true;
    }
    IActor FindActorIntoFovView()
    {
        _fsmController.SearchHandler.AddInsideDot(120f);
        _fsmController.SearchHandler.AddConditionDistance(FSMConfigureData.ChaseRadius);
        return _fsmController.SearchHandler.FindActor();
    }
}
