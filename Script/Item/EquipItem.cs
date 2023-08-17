using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipItem : Item
{
    protected Data.ItemStatData StatData => DataManager.Instance.ItemStatTable[ItemData.ReferenceIndex];
    Dictionary<eStatusType, uint> _statusType;
    public override string GetInformationText()
    {
        if (StatData.Stat != null)
        {
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(base.GetInformationText()).Append("\n");
            foreach (var element in StatData.Stat)
            {
                if(element.Value != 0)
                    stringBuilder.Append("\n").Append(element.Key.ToString()).Append(": ").Append(element.Value);
            }
            return stringBuilder.ToString();
        }
        
        return base.GetInformationText();
    }
    public void OnEquipAction(IActor owner)
    {
        _statusType = new Dictionary<eStatusType, uint>(StatData.Stat.Count);
        foreach (var element in StatData.Stat)
        {
            StatusFloat statusFloat = owner.ActorStat.GetStatusContents(element.Key);
            uint key = statusFloat.AddSum(element.Value);
            _statusType.Add(element.Key, key);
        }
    }
    public void OnUnEquipAction(IActor owner)
    {
        foreach (var element in _statusType)
        {
            StatusFloat statusFloat = owner.ActorStat.GetStatusContents(element.Key);
            statusFloat.Remove(eStatusCalculateSign.Add, element.Value);
        }
        _statusType = null;
    }
}
