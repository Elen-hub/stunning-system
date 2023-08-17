using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

public class SleepObject : InteractObject
{
    Character _character;
    public override bool IsPossibleInteract(IActor caster)
    {
        Character character = caster as Character;
        if(character.CurrentSleepy < 40f)
        {
#if !UNITY_SERVER
            UIManager.Instance.FieldUI.SetFieldText(caster, 9001);
#endif
            return false;
        }
        return base.IsPossibleInteract(caster);
    }
    protected override void OnInteractStart(IActor caster)
    {
        base.OnInteractStart(caster);

        Debug.Log("Start");
        _character = caster as Character;
        if (_character != null)
        {
            _character.SetSleep(true);
#if UNITY_SERVER
            RPCGenerator.Instance.RPC_SetEmoticon(_character.WorldID, eEmoticonType.SleepMark, eAttachmentTarget.OverHead, true, 100f);
#endif
        }
    }
    protected override void OnInteractEnd()
    {
        base.OnInteractEnd();

        Debug.Log("Exit");
        _character.SetSleep(false);
        _character = null;
    }
    protected override void Update()
    {
        base.Update();

#if UNITY_SERVER
        if (_character != null)
        {
            _character.CurrentSleepy -= TimeManager.DeltaTime * 5f;
            if(_character.CurrentSleepy <= 0f)
            {
                InteractExit();
                NetworkManager.Instance.ActorEventSender.NotifyInteractEnd(WorldID);
            }
        }
#endif
    }
}
