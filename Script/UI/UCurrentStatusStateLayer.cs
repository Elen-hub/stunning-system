using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UCurrentStatusStateLayer : UBaseSubUI
{
    Dictionary<eStatusType, StatusState> _statusStateDictionary;
    protected override void InitReference()
    {
        _statusStateDictionary = new Dictionary<eStatusType, StatusState>();
        OnInitializeStatusState(eStatusType.HP);
        OnInitializeStatusState(eStatusType.Temperature);
        OnInitializeStatusState(eStatusType.Hunger);
        OnInitializeStatusState(eStatusType.Stress);
        OnInitializeStatusState(eStatusType.Sleepy);
    }
    void OnInitializeStatusState(eStatusType type)
    {
        _statusStateDictionary.Add(type, transform.Find(type.ToString() + "StatusState").GetComponent<StatusState>());
        _statusStateDictionary[type].Initialize(type);
    }
    public void Enable(Character character)
    {
        _statusStateDictionary[eStatusType.HP].Enable(character);
        _statusStateDictionary[eStatusType.Temperature].Enable(character);
        _statusStateDictionary[eStatusType.Hunger].Enable(character);
        _statusStateDictionary[eStatusType.Stress].Enable(character);
        _statusStateDictionary[eStatusType.Sleepy].Enable(character);
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        foreach (var element in _statusStateDictionary)
            element.Value.Disable();

        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        foreach (var element in _statusStateDictionary)
            element.Value.OnUpdate();
    }
}
