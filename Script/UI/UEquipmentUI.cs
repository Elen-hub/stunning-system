using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UEquipmentUI : BaseUI
{
    Dictionary<eEquipSlotType, EquipSlot> _equipSlotDictionary;

    [SerializeField] float _elapsedTime = 0f;
    Vector2 _openPosition;
    Vector2 _closePosition;

    EquipmentComponent _equipmentComponent;

    StatusElement[] _statusElementArr;
    TextMeshProUGUI _nameText;
    protected override void InitReference()
    {
        _nameText = transform.Find("InformationLayer/Text_Name").GetComponent<TextMeshProUGUI>();
        _equipSlotDictionary = new Dictionary<eEquipSlotType, EquipSlot>((int)eEquipSlotType.End);
        _equipSlotDictionary.Add(eEquipSlotType.Hat, transform.Find("EquipmentLayer/EquipSlot_Hat").GetComponent<EquipSlot>());
        _equipSlotDictionary.Add(eEquipSlotType.Armor, transform.Find("EquipmentLayer/EquipSlot_Armor").GetComponent<EquipSlot>());
        _equipSlotDictionary.Add(eEquipSlotType.Shoes, transform.Find("EquipmentLayer/EquipSlot_Shoes").GetComponent<EquipSlot>());
        _equipSlotDictionary.Add(eEquipSlotType.Necklace, transform.Find("EquipmentLayer/EquipSlot_Necklace").GetComponent<EquipSlot>());
        _equipSlotDictionary.Add(eEquipSlotType.Ring1, transform.Find("EquipmentLayer/EquipSlot_Ring1").GetComponent<EquipSlot>());
        _equipSlotDictionary.Add(eEquipSlotType.Ring2, transform.Find("EquipmentLayer/EquipSlot_Ring2").GetComponent<EquipSlot>());
        _statusElementArr = transform.Find("InformationLayer/Scroll View/Viewport/Content").GetComponentsInChildren<StatusElement>(true);
        for (int i = 0; i < _statusElementArr.Length; ++i)
            _statusElementArr[i].Initialize();

        _closePosition = new Vector2(0f, rectTransform.sizeDelta.y);
        _openPosition = new Vector2(0f, -65f);
    }
    protected override void OnOpen()
    {
        _nameText.text = PlayerManager.Instance.Me.Character.Name;
        _equipmentComponent = PlayerManager.Instance.Me.Character.GetComponent<EquipmentComponent>(eComponent.EquipmentComponent);
        _equipmentComponent.OnChangedSlotEvent += OnDrawSlot;
        for (int i = 0; i < _statusElementArr.Length; ++i)
            _statusElementArr[i].Enable(PlayerManager.Instance.Me.Character.ActorStat);
        
        foreach (var element in _equipSlotDictionary)
            element.Value.Enable(_equipmentComponent);

        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        _equipmentComponent.OnChangedSlotEvent -= OnDrawSlot;
        _equipmentComponent = null;
        for (int i = 0; i < _statusElementArr.Length; ++i)
            _statusElementArr[i].Disable();
    }
    void OnDrawSlot(eEquipSlotType slotType, Item item)
    {
        _equipSlotDictionary[slotType].Redraw();
    }
    void OnUpdateActivateAnimation()
    {
        if (IsActive)
        {
            if (_elapsedTime < 1f)
            {
                const float speed = 5f;
                _elapsedTime += TimeManager.DeltaTime * speed;
                Vector2 anchoredPosition = Vector2.Lerp(_closePosition, _openPosition, _elapsedTime);
                rectTransform.anchoredPosition = anchoredPosition;
            }
        }
        else
        {
            if (_elapsedTime > 0f)
            {
                const float speed = 4f;
                _elapsedTime -= TimeManager.DeltaTime * speed;
                Vector2 anchoredPosition = Vector2.Lerp(_closePosition, _openPosition, _elapsedTime);
                rectTransform.anchoredPosition = anchoredPosition;
            }
            else
                gameObject.SetActive(false);
        }
    }
    private void LateUpdate()
    {
        OnUpdateActivateAnimation();
    }
}
