using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class ItemSlot : Button
{
    ulong _inventoryID;
    public ulong InventoryID => _inventoryID;
    int _slotArrayPosition;
    public int SlotArrayPosition => _slotArrayPosition;

    RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;
    Image _iconImage;
    public Item GetItem => ItemManager.Instance.GetInventory(_inventoryID).GetItem(_slotArrayPosition);
    Image _selectImage;
    TextMeshProUGUI _countText;

    int _currentCount;

    public bool IsPossiblePush = true;
    public void SelectState(bool isSelect) => _selectImage.enabled = isSelect;
    public ItemSlot Initialize()
    {
        _rectTransform = transform as RectTransform;
        _iconImage = transform.Find("Img_Icon").GetComponent<Image>();
        _selectImage = transform.Find("SelectCornor").GetComponent<Image>();
        _countText = transform.Find("Text_Count").GetComponent<TextMeshProUGUI>();
        return this;
    }
    public void SetReferenceRoot(ulong inventoryID, int arrayPosition)
    {
        _inventoryID = inventoryID;
        _slotArrayPosition = arrayPosition;
        Redraw();
    }
    public void Redraw()
    {
        OnDrawIcon();
        OnDrawText();
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    void OnDrawIcon()
    {
        if (GetItem != null)
        {
            _iconImage.sprite = GetItem.ItemData.Icon;
            _iconImage.enabled = true;
        }
        else
        {
            _iconImage.enabled = false;
        }
    }
    void OnDrawText()
    {
        if(GetItem == null)
        {
            _countText.enabled = false;
            return;
        }
        if (GetItem.ItemData.MergeCount > 1)
        {
            _countText.enabled = true;
            if (_currentCount != GetItem.Count)
            {
                _currentCount = GetItem.Count;
                _countText.text = _currentCount.ToString();
            }
        }
        else
            _countText.enabled = false;
    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (GetItem != null)
            UIManager.Instance.Open<UToolTipUI>(eUIName.UToolTipUI).SetItemToolTip(this);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (UIManager.Instance.Get(eUIName.UToolTipUI))
            UIManager.Instance.Close(eUIName.UToolTipUI);
    }
}
