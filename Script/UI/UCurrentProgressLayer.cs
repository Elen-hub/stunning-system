using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UCurrentProgressLayer : UBaseSubUI
{
    RectScaleProgress _hpProgress;
    RectScaleProgress _staminaProgress;
    Character _character;
    protected override void InitReference()
    {
        _hpProgress = transform.Find("HPBar").GetComponent<RectScaleProgress>();
        _hpProgress.Initialize();

        _staminaProgress = transform.Find("StaminaBar").GetComponent<RectScaleProgress>();
        _staminaProgress.Initialize();
    }
    public void Enable(Character character)
    {
        if(character == null)
        {
            Disable();
            return;
        }
        _character = character;

        _hpProgress.GetCurrentAmount = GetCurrentHP;
        _hpProgress.GetMaxAmount = GetMaxHP;
        _staminaProgress.GetCurrentAmount = GetCurrentSP;
        _staminaProgress.GetMaxAmount = GetMaxSP;

        gameObject.SetActive(true);

        float GetCurrentHP() => _character.CurrentHP;
        float GetMaxHP() => _character.ActorStat[eStatusType.HP];

        float GetCurrentSP() => _character.CurrentSP;
        float GetMaxSP() => _character.ActorStat[eStatusType.SP];
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
}
