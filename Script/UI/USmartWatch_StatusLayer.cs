using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class USmartWatch_StatusLayer : USmartWatchSubUI
{
    Character _character;
    Image _hungerProgress;
    Image _sleepProgress;
    Image _stressProgress;
    Image _tempProgress;

    protected override void InitReference()
    {
        base.InitReference();

        _hungerProgress = transform.Find("Hunger/Img_Progress").GetComponent<Image>();
        _tempProgress = transform.Find("Temp/Img_Progress").GetComponent<Image>();
        _sleepProgress = transform.Find("Sleep/Img_Progress").GetComponent<Image>();
        _stressProgress = transform.Find("Stress/Img_Progress").GetComponent<Image>();
    }
    public override void Enable()
    {
        gameObject.SetActive(true);
    }
    public override void Disable()
    {
        _character = null;
        gameObject.SetActive(false);
    }
    void OnUpdateHunger() => _hungerProgress.fillAmount = _character.CurrentHunger / _character.ActorStat[eStatusType.Hunger];
    void OnUpdateSleep() => _sleepProgress.fillAmount = 1f - _character.CurrentSleepy / _character.ActorStat[eStatusType.Sleepy];
    void OnUpdateStress() => _stressProgress.fillAmount = _character.CurrentStress / _character.ActorStat[eStatusType.Stress];
    void OnUpdateTemp() => _tempProgress.fillAmount = _character.CurrentTemperature / (_character.ActorStat[eStatusType.Temperature] * 2);
    private void LateUpdate()
    {
        if(_character == null)
        {
            Player player = PlayerManager.Instance.Me;
            if (player == null)
                return;

            DynamicActor dynamicActor = player.Character;
            if (dynamicActor != null)
                _character = dynamicActor as Character;

            return;
        }

        OnUpdateHunger();
        OnUpdateSleep();
        OnUpdateStress();
        OnUpdateTemp();
    }
}
