using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UToolTipUI : BaseUI
{
    public enum eToolTipType
    {
         None,
         ItemToolTip,
         StatisStateToolTip,
         BuffToolTip,
    }
    Dictionary<eToolTipType, BaseToolTipUI> _baseToolTipDictionary;
    eToolTipType _toolTipType;
    protected override void InitReference()
    {
        _baseToolTipDictionary = new Dictionary<eToolTipType, BaseToolTipUI>();
        _baseToolTipDictionary.Add(eToolTipType.ItemToolTip, transform.Find("UItemToolTip").GetComponent<UItemToolTip>().Initialize());
        _baseToolTipDictionary.Add(eToolTipType.StatisStateToolTip, transform.Find("UStatusStateToolTip").GetComponent<UStatusStateToolTip>().Initialize());
        _baseToolTipDictionary.Add(eToolTipType.BuffToolTip, transform.Find("UBuffToolTip").GetComponent<UBuffToolTip>().Initialize());
    }
    public void SetItemToolTip(ItemSlot itemSlot)
    {
        if (_toolTipType != eToolTipType.None && _toolTipType != eToolTipType.ItemToolTip)
            _baseToolTipDictionary[_toolTipType].Disable();

        _toolTipType = eToolTipType.ItemToolTip;
        (_baseToolTipDictionary[eToolTipType.ItemToolTip] as UItemToolTip).Enable(itemSlot);
    }
    public void SetItemToolTip(EquipSlot itemSlot)
    {
        if (_toolTipType != eToolTipType.None && _toolTipType != eToolTipType.ItemToolTip)
            _baseToolTipDictionary[_toolTipType].Disable();

        _toolTipType = eToolTipType.ItemToolTip;
        (_baseToolTipDictionary[eToolTipType.ItemToolTip] as UItemToolTip).Enable(itemSlot);
    }
    public void SetStatusStateToolTip(Vector2 uiPosition, StatusState statusState)
    {
        if (_toolTipType != eToolTipType.None && _toolTipType != eToolTipType.StatisStateToolTip)
            _baseToolTipDictionary[_toolTipType].Disable();

        _toolTipType = eToolTipType.StatisStateToolTip;
        (_baseToolTipDictionary[eToolTipType.StatisStateToolTip] as UStatusStateToolTip).Enable(uiPosition, statusState);
    }
    public void SetBuffToolTip(Vector2 uiPosition, Buff buff)
    {
        if (_toolTipType != eToolTipType.None && _toolTipType != eToolTipType.BuffToolTip)
            _baseToolTipDictionary[_toolTipType].Disable();

        _toolTipType = eToolTipType.BuffToolTip;
        (_baseToolTipDictionary[eToolTipType.BuffToolTip] as UBuffToolTip).Enable(uiPosition, buff);
    }
    protected override void OnOpen()
    {
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        if(_toolTipType != eToolTipType.None)
            _baseToolTipDictionary[_toolTipType].Disable();

        _toolTipType = eToolTipType.None;
        gameObject.SetActive(false);
    }
}
