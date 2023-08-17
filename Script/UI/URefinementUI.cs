using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class URefinementUI : BaseUI
{
    [SerializeField] int _makeIndex;

    float _elapsedTime = 0f;
    Vector2 _openPosition;
    Vector2 _closePosition;

    TextMeshProUGUI _nameText;
    RefinementObject _refinementObject;

    ItemSlot[] _itemSlotArr;

    TextMeshProUGUI _progressingText;
    RectScaleProgress _refinementProgress;

    int _currentSeconds;
    public void SetRefinementObject(RefinementObject refinementObject)
    {
        if (_refinementObject != null)
        {
            _refinementObject.Inventory.OnSlotChangedCallback -= OnSlotChange;
        }
        _refinementObject = refinementObject;
        if (_refinementObject != null)
        {
            _refinementObject.Inventory.OnSlotChangedCallback += OnSlotChange;
            UIManager.Instance.Get<UInventory>(eUIName.UInventory).SetSubInventory = _refinementObject.Inventory.InventoryID;
            UIManager.Instance.Open(eUIName.UInventory);
            OnUpdateProgressText();
        }
    }
    float GetCurrentAmount() => _refinementObject != null ? _refinementObject.ElapsedTime : 0f;
    float GetMaxAmount() => _refinementObject != null ? _refinementObject.TargetTime : 1f;
    void OnSlotChange(int number) => _itemSlotArr[number].Redraw();
    protected override void InitReference()
    {
        _closePosition = new Vector2(0f, rectTransform.sizeDelta.y);
        _openPosition = new Vector2(0f, -65f);

        _nameText = transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
        _refinementProgress = transform.Find("CraftingProgress").GetComponent<RectScaleProgress>();
        _refinementProgress.Initialize();
        _refinementProgress.GetCurrentAmount = GetCurrentAmount;
        _refinementProgress.GetMaxAmount = GetMaxAmount;
        _progressingText = transform.Find("CraftingProgress/DynamicCanvas/Text_Progress").GetComponent<TextMeshProUGUI>();

        Transform slotLayout = transform.Find("SlotLayout");
        _itemSlotArr = new ItemSlot[2];
        _itemSlotArr[0] = slotLayout.Find("ItemSlot").GetComponent<ItemSlot>();
        _itemSlotArr[1] = slotLayout.Find("ResultSlot").GetComponent<ItemSlot>();
        for (int i = 0; i < _itemSlotArr.Length; ++i)
            _itemSlotArr[i].Initialize();
    }
    protected override void OnOpen()
    {
        UIManager.Instance.Open(eUIName.UInventory);

        DrawAllSlot();
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        SetRefinementObject(null);
    }
    public override void Refresh()
    {
        if (_refinementObject != null)
            _nameText.text = LocalizingManager.Instance.GetLocalizing(DataManager.Instance.ObjectTable[_refinementObject.Index].NameKey);
    }
    void DrawAllSlot()
    {
        if (_refinementObject == null)
        {
            for (int i = 0; i < _itemSlotArr.Length; ++i)
                _itemSlotArr[i].Disable();
        }
        else
        {
            for (int i = 0; i < _refinementObject.Inventory.Capacity; ++i)
                _itemSlotArr[i].SetReferenceRoot(_refinementObject.Inventory.InventoryID, i);
        }
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
    void OnUpdateProgressText()
    {
        if (_refinementObject == null)
            return;

        if (!_refinementObject.IsRunning)
            return;

        int sec = Mathf.FloorToInt(GetMaxAmount() - GetCurrentAmount());
        if (sec < 0)
            sec = 0;

        if (_currentSeconds != sec)
        {
            _currentSeconds = sec;
            _progressingText.text = LocalizingManager.Instance.GetLocalizing(30, _currentSeconds);
        }
    }
    private void LateUpdate()
    {
        OnUpdateActivateAnimation();
        OnUpdateProgressText();
    }
}
