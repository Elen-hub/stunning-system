using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UStorageUI : BaseUI
{
    [SerializeField] protected Sprite _interactSprite;
    float _elapsedTime = 0f;
    Vector2 _openPosition;
    Vector2 _closePosition;

    List<ItemSlot> _itemSlotList;
    Inventory _inventory;
    public void SetInventory(Inventory inventory, ItemComponent itemComponent)
    {
        if (_inventory != null)
        {
            _inventory.OnSlotChangedCallback -= OnSlotChange;
        }
        _inventory = inventory;
        if (_inventory != null)
        {
            _inventory.OnSlotChangedCallback += OnSlotChange;
            UIManager.Instance.Get<UInventory>(eUIName.UInventory).SetSubInventory = inventory.InventoryID;
            UIManager.Instance.Open(eUIName.UInventory);
        }
    }
    void OnSlotChange(int number) => _itemSlotList[number].Redraw();
    protected override void InitReference()
    {
        _itemSlotList = new List<ItemSlot>(24);
        _openPosition = new Vector2(0f, -65f);
        _closePosition = new Vector2(0f, rectTransform.sizeDelta.y);
    }
    protected override void OnOpen()
    {
        UIManager.Instance.Open(eUIName.UInventory);

        DrawAllSlot();
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        SetInventory(null, null);
    }
    void DrawAllSlot()
    {
        if(_inventory == null)
        {
            for (int i = 0; i < _itemSlotList.Count; ++i)
                _itemSlotList[i].Disable();
        }
        else
        {
            int slotCount = 0;
            while (slotCount < _inventory.Capacity)
            {
                if (_itemSlotList.Count <= slotCount)
                {
                    ItemSlot instance = UIManager.Instance.GetItemSlot();
                    instance.transform.SetParent(transform);
                    instance.transform.localScale = Vector3.one;
                    _itemSlotList.Add(instance);
                }
                _itemSlotList[slotCount].SetReferenceRoot(_inventory.InventoryID, slotCount);
                // _itemSlotList[slotCount].SetItem(_itemComponent.GetDefaultBag.GetItem(slotCount));
                ++slotCount;
            }
            while (slotCount < _itemSlotList.Count)
            {
                _itemSlotList[slotCount].Disable();
                ++slotCount;
            }
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
    private void LateUpdate()
    {
        OnUpdateActivateAnimation();
    }
}
