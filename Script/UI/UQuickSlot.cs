using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UQuickSlot : BaseUI
{
    RectTransform[] _quickSlotGrid;
    ItemSlot[][] _itemSlot;
    ItemComponent _itemComponent;
    Animator _animator;
    int _selectNumberHash;

    int _prevQuickSlotNumber;
    int _prevCurrentSelect;

    public ItemComponent SetItemComponent { 
        set {
            if (_itemComponent != null)
            {
                _itemComponent.OnQuickSlotSwapCallback -= OnSwapQuickslot;
                _itemComponent.GetQuickSlotInventory[0].OnSlotChangedCallback -= OnQuickSlotChanged0;
                _itemComponent.GetQuickSlotInventory[1].OnSlotChangedCallback -= OnQuickSlotChanged1;
                _itemComponent.OnSelectSlot -= OnSelectSlot;
            }
            _itemComponent = value;
            if (_itemComponent != null)
            {
                _itemComponent.OnQuickSlotSwapCallback += OnSwapQuickslot;
                _itemComponent.GetQuickSlotInventory[0].OnSlotChangedCallback += OnQuickSlotChanged0;
                _itemComponent.GetQuickSlotInventory[1].OnSlotChangedCallback += OnQuickSlotChanged1;
                _itemComponent.OnSelectSlot += OnSelectSlot;
                for (int i = 0; i < _itemComponent.GetQuickSlotInventory.Length; ++i)
                    for (int j = 0; j < _itemComponent.GetQuickSlotInventory[i].Capacity; ++j)
                        _itemSlot[i][j].SetReferenceRoot(_itemComponent.GetQuickSlotInventory[i].InventoryID, j);

                OnSelectSlot(_itemComponent.CurrentSelectQuickSlot, _itemComponent.CurrentSelectSlot);
            }
        }
    }
    protected override void InitReference()
    {
        _quickSlotGrid = new RectTransform[2];
        _quickSlotGrid[0] = transform.Find("QuickSlot2") as RectTransform;
        _quickSlotGrid[1] = transform.Find("QuickSlot1") as RectTransform;
        _itemSlot = new ItemSlot[_quickSlotGrid.Length][];
        for (int i = 0; i < _quickSlotGrid.Length; ++i)
        {
            _itemSlot[i] = _quickSlotGrid[i].GetComponentsInChildren<ItemSlot>(true);
            for (int j = 0; j < _itemSlot[i].Length; ++j)
                _itemSlot[i][j].Initialize();
        }
        _animator = GetComponent<Animator>();
        _selectNumberHash = Animator.StringToHash("SelectNumber");
    }
    protected override void OnOpen()
    {
        if (_itemComponent != null)
        {
            _itemComponent.OnQuickSlotSwapCallback += OnSwapQuickslot;
            _itemComponent.GetQuickSlotInventory[0].OnSlotChangedCallback += OnQuickSlotChanged0;
            _itemComponent.GetQuickSlotInventory[1].OnSlotChangedCallback += OnQuickSlotChanged1;
            _itemComponent.OnSelectSlot += OnSelectSlot;
        }
    }
    protected override void OnClose()
    {
        if (_itemComponent != null)
        {
            _itemComponent.OnQuickSlotSwapCallback -= OnSwapQuickslot;
            _itemComponent.GetQuickSlotInventory[0].OnSlotChangedCallback -= OnQuickSlotChanged0;
            _itemComponent.GetQuickSlotInventory[1].OnSlotChangedCallback -= OnQuickSlotChanged1;
            _itemComponent.OnSelectSlot -= OnSelectSlot;
        }
    }
    void OnSelectSlot(int quickSlotNumber, int selectSlotNumber)
    {
        _itemSlot[_prevQuickSlotNumber][_prevCurrentSelect].SelectState(false);
        _prevQuickSlotNumber = quickSlotNumber;
        _prevCurrentSelect = selectSlotNumber;
        _itemSlot[_prevQuickSlotNumber][_prevCurrentSelect].SelectState(true);
    }
    void OnSwapQuickslot(int quickslotNumber)
    {
        _animator.SetInteger(_selectNumberHash, quickslotNumber);
        if (quickslotNumber == 0)
        {
            _quickSlotGrid[0].SetAsLastSibling();
            _quickSlotGrid[1].SetAsFirstSibling();
        }
        else
        {
            _quickSlotGrid[1].SetAsLastSibling();
            _quickSlotGrid[0].SetAsFirstSibling();
        }
        OnSelectSlot(_itemComponent.CurrentSelectQuickSlot, _itemComponent.CurrentSelectSlot);
    }
    void OnQuickSlotChanged0(int slotNumber) => _itemSlot[0][slotNumber].Redraw();
    void OnQuickSlotChanged1(int slotNumber) => _itemSlot[1][slotNumber].Redraw();
    public void SetAnchoredPosition(Vector2 position)
    {
        rectTransform.anchoredPosition = position;
    }
}
