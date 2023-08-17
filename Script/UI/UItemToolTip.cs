using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UItemToolTip : BaseToolTipUI
{
    ItemSlot _itemSlot;
    EquipSlot _equipSlot;
    TextMeshProUGUI _typeText;
    TextMeshProUGUI _priceText;
    protected override void InitReference()
    {
        base.InitReference();

        _typeText = transform.Find("Text_Type").GetComponent<TextMeshProUGUI>();
        _priceText = transform.Find("Text_Price").GetComponent<TextMeshProUGUI>();
    }
    public void Enable(ItemSlot itemSlot)
    {
        _itemSlot = itemSlot;
        OnProcessText(_itemSlot.GetItem);
        OnProcessScale();
        OnProcessPosition(itemSlot.RectTransform.position);
        gameObject.SetActive(true);
    }
    public void Enable(EquipSlot itemSlot)
    {
        _equipSlot = itemSlot;
        OnProcessText(_equipSlot.GetItem);
        OnProcessScale();
        OnProcessPosition(itemSlot.RectTransform.position);
        gameObject.SetActive(true);
    }
    public override void Disable()
    {
        gameObject.SetActive(false);
    }
    void OnProcessText(Item item)
    {
        _nameText.text = LocalizingManager.Instance.GetLocalizing(item.ItemData.NameKey);
        _informationText.text = item.GetInformationText();
        _typeText.text = item.ItemData.Type.ToString();
        _priceText.text = LocalizingManager.Instance.GetLocalizing(1001, item.ItemData.Price);
    }
}
