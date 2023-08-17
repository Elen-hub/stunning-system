using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentComponent : BaseComponent
{
    Dictionary<eEquipSlotType, EquipItem> _equipSlotDictionray;
    public event System.Action<eEquipSlotType, EquipItem> OnChangedSlotEvent;
    public EquipmentComponent(IActor actor) : base(actor, eComponent.EquipmentComponent)
    {
        _equipSlotDictionray = new Dictionary<eEquipSlotType, EquipItem>((int)eEquipSlotType.End);
        AddEventMethods(eComponentEvent.SetMainCharacter, OnReceiveEventSetMainCharacter);
    }
    void OnReceiveEventSetMainCharacter(params object[] messageArr)
    {
        OnChangedSlotEvent += OnSendPacketEquip;
    }
    void OnSendPacketEquip(eEquipSlotType slotType, EquipItem item)
    {
        if (item != null) NetworkManager.Instance.ItemEventSender.RequestEquipItem(Owner.WorldID, slotType, item);
        else NetworkManager.Instance.ItemEventSender.RequestUnequipItem(Owner.WorldID, slotType);
    }
    public bool IsContainsItem(eEquipSlotType type) => _equipSlotDictionray.ContainsKey(type);
    public EquipItem GetItem(eEquipSlotType slotType) => _equipSlotDictionray.ContainsKey(slotType) ? _equipSlotDictionray[slotType] : null;
    public EquipItem PushItem(eEquipSlotType slotType, EquipItem item)
    {
        if(_equipSlotDictionray.ContainsKey(slotType))
        {
            EquipItem temp = _equipSlotDictionray[slotType];
            temp.OnUnEquipAction(_owner);
            _equipSlotDictionray[slotType] = item;
            item.OnEquipAction(_owner);
            if (OnChangedSlotEvent != null)
                OnChangedSlotEvent(slotType, item);
            return temp;
        }
        else
        {
            _equipSlotDictionray.Add(slotType, item);
            item.OnEquipAction(_owner);
            if (OnChangedSlotEvent != null)
                OnChangedSlotEvent(slotType, item);
            return null;
        }
    }
    public EquipItem PopSlot(eEquipSlotType slotType)
    {
        EquipItem temp = _equipSlotDictionray[slotType];
        _equipSlotDictionray.Remove(slotType);
        if (OnChangedSlotEvent != null)
            OnChangedSlotEvent(slotType, null);

        temp.OnUnEquipAction(_owner);
        return temp;
    }
}
