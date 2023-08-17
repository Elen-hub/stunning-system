using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipSlot : Button
{
    RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;
    public eEquipSlotType EquipSlotType;
    public eItemType AccessItemType;
    public EquipItem GetItem => _equipmentComponent?.GetItem(EquipSlotType);
    Image _defaultImage;
    Image _iconImage;
    Image _cornorImage;
    EquipmentComponent _equipmentComponent;
    protected override void Awake()
    {
        base.Awake();

        _rectTransform = transform as RectTransform;
        _iconImage = transform.Find("Img_Icon").GetComponent<Image>();
        _defaultImage = transform.Find("Img_Default").GetComponent<Image>();
        _cornorImage = transform.Find("Img_Cornor").GetComponent<Image>();
    }
    public void Enable(EquipmentComponent equipmentComponent)
    {
        _equipmentComponent = equipmentComponent;
        Redraw();
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        _equipmentComponent = null;
        gameObject.SetActive(false);
    }
    public void Redraw()
    {
        OnDrawIcon();
        OnDrawCornor();
    }
    void OnDrawIcon() 
    {
        if(_equipmentComponent.IsContainsItem(EquipSlotType))
        {
            _iconImage.sprite = _equipmentComponent.GetItem(EquipSlotType).ItemData.Icon;
            _iconImage.enabled = true;
            _defaultImage.enabled = false;
        }
        else
        {
            _iconImage.enabled = false;
            _defaultImage.enabled = true;
        }
    }
    void OnDrawCornor()
    {

    }
    public override void OnPointerEnter(PointerEventData eventData)
    {
        base.OnPointerEnter(eventData);

        if (_equipmentComponent.GetItem(EquipSlotType) != null)
            UIManager.Instance.Open<UToolTipUI>(eUIName.UToolTipUI).SetItemToolTip(this);
    }
    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);

        if (UIManager.Instance.Get(eUIName.UToolTipUI))
            UIManager.Instance.Close(eUIName.UToolTipUI);
    }
}
