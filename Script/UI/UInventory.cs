using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UInventory : BaseUI
{
    InventoryInputHandler _inventoryInputHandler;
    List<ItemSlot> _itemSlotList;
    Transform _root;
    ItemComponent _itemComponent;

    float _elapsedTime = 0f;
    Vector2 _openPosition;

    ulong _subInventoryIndex;
    public ulong SubInventoryIndex => _subInventoryIndex;
    ItemComponent SetItemComponent {
        set {
            if (_itemComponent != null)
            {
                if (_itemComponent.GetDefaultBag != null)
                    _itemComponent.GetDefaultBag.OnSlotChangedCallback -= OnDefaultBagChanged;
                if(_itemComponent.GetExpansionBag != null)
                    _itemComponent.GetExpansionBag.OnSlotChangedCallback -= OnExpansionBagChanged;
            }
            _itemComponent = value;
            if (_itemComponent != null)
            {
                if (_itemComponent.GetDefaultBag != null)
                    _itemComponent.GetDefaultBag.OnSlotChangedCallback += OnDefaultBagChanged;
                if (_itemComponent.GetExpansionBag != null)
                    _itemComponent.GetExpansionBag.OnSlotChangedCallback += OnExpansionBagChanged;

                _inventoryInputHandler.ItemComponent = value;
            }
        }
    }
    public ulong SetSubInventory {
        set {
            (_inputHandler as InventoryInputHandler).SubInventory = value;
            _subInventoryIndex = value;
        }
    }
    protected override void InitReference()
    {
        _root = transform.Find("Scroll View/Viewport/Content").transform;
        _itemSlotList = new List<ItemSlot>(24);

        _openPosition = new Vector2(0f, rectTransform.sizeDelta.y);
    }
    protected override void InitInput()
    {
        _inventoryInputHandler = new InventoryInputHandler();
        _inputHandler = _inventoryInputHandler;
    }
    protected override void OnOpen()
    {
        SetItemComponent = PlayerManager.Instance.Me.Character.GetComponent<ItemComponent>(eComponent.ItemComponent);
        DrawAllSlot();
        gameObject.SetActive(true);
    }
    void OnDefaultBagChanged(int number) => _itemSlotList[number].Redraw();
    void OnExpansionBagChanged(int number) => _itemSlotList[number + ClientConst.DefaultBagCapacity].Redraw();
    void DrawAllSlot()
    {
        if (_itemComponent == null)
        {
            for (int i = 0; i < _itemSlotList.Count; ++i)
                _itemSlotList[i].Disable();
        }
        else
        {
            int slotCount = 0;
            if (_itemComponent.GetDefaultBag != null)
            {
                while (slotCount < _itemComponent.GetDefaultBag.Capacity)
                {
                    if (_itemSlotList.Count <= slotCount)
                    {
                        ItemSlot instance = UIManager.Instance.GetItemSlot();
                        instance.transform.SetParent(_root);
                        instance.transform.localScale = Vector3.one;
                        _itemSlotList.Add(instance);
                    }
                    _itemSlotList[slotCount].SetReferenceRoot(_itemComponent.GetDefaultBag.InventoryID, slotCount);
                    // _itemSlotList[slotCount].SetItem(_itemComponent.GetDefaultBag.GetItem(slotCount));
                    ++slotCount;
                }
            }
            else
            {
                if (_itemComponent.GetExpansionBag != null)
                {
                    int tempCount = slotCount;
                    while (slotCount < _itemComponent.GetExpansionBag.Capacity)
                    {
                        if (_itemSlotList.Count <= slotCount)
                        {
                            ItemSlot instance = UIManager.Instance.GetItemSlot();
                            instance.transform.SetParent(_root);
                            _itemSlotList.Add(instance);
                        }
                        _itemSlotList[slotCount].SetReferenceRoot(_itemComponent.GetExpansionBag.InventoryID, slotCount - tempCount);
                        // _itemSlotList[slotCount].SetItem(_itemComponent.GetExpansionBag.GetItem(slotCount - tempCount));
                        ++slotCount;
                    }
                }
                while(slotCount < _itemSlotList.Count)
                {
                    _itemSlotList[slotCount].Disable();
                    ++slotCount;
                }
            }
        }
    }
    protected override void OnClose()
    {
        if (_subInventoryIndex != 0)
            SetSubInventory = 0;
        else
            UIManager.Instance.Close(eUIName.UEquipmentUI);

        SetItemComponent = null;
    }
    void OnUpdateActivateAnimation()
    {
        if (IsActive)
        {
            if (_elapsedTime < 1f)
            {
                const float speed = 5f;
                _elapsedTime += TimeManager.DeltaTime * speed;
                Vector2 anchoredPosition = Vector2.Lerp(Vector3.zero, _openPosition, _elapsedTime);
                rectTransform.anchoredPosition = anchoredPosition;
                if (UIManager.Instance.IsOpen(eUIName.UQuickSlot))
                    UIManager.Instance.Get(eUIName.UQuickSlot).rectTransform.anchoredPosition = anchoredPosition;
            }
        }
        else
        {
            if (_elapsedTime > 0f)
            {
                const float speed = 4f;
                _elapsedTime -= TimeManager.DeltaTime * speed;
                Vector2 anchoredPosition = Vector2.Lerp(Vector3.zero, _openPosition, _elapsedTime);
                rectTransform.anchoredPosition = anchoredPosition;
                if (UIManager.Instance.IsOpen(eUIName.UQuickSlot))
                    UIManager.Instance.Get(eUIName.UQuickSlot).rectTransform.anchoredPosition = anchoredPosition;
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
