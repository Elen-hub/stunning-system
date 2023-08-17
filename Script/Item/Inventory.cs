using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

[System.Serializable]
public class Inventory : IPacket
{
    ulong _inventoryID;
    public ulong InventoryID => _inventoryID;
    public event System.Action<int> OnSlotChangedCallback;
    public event System.Action<int, Item, Item> OnSelectionChangedCallback;
    InventoryConstraint _globalConstraint;
    public InventoryConstraint SetGlobalConstraint { set => _globalConstraint = value; }
    Dictionary<int, InventoryConstraint> _slotInventoryConstraint;
    public void SetSlotConstraint(int slotNumber, InventoryConstraint constraint)
    {
        if (_slotInventoryConstraint == null)
            _slotInventoryConstraint = new Dictionary<int, InventoryConstraint>();

        if (_slotInventoryConstraint.ContainsKey(slotNumber)) _slotInventoryConstraint[slotNumber] = constraint;
        else _slotInventoryConstraint.Add(slotNumber, constraint);
    }
    [SerializeField] Item[] _itemArr;
    public int Capacity => _itemArr.Length;
    public void SlotChangedCallback(int number) => OnSlotChangedCallback?.Invoke(number);
    public Item GetItem(int invenNumber) => _itemArr[invenNumber];
    public Inventory(ulong inventoryID, int capacity)
    {
        _inventoryID = inventoryID;
        _itemArr = new Item[capacity];
    }
    public bool IsContainsItem(int index, int count)
    {
        int tempCount = count;
        for (int i = 0; i < _itemArr.Length; ++i)
        {
            if (tempCount <= 0)
                return true;

            if (_itemArr[i] == null)
                continue;

            if (_itemArr[i].Index != index)
                continue;

            if (_itemArr[i].Count > tempCount)
                return true;

            tempCount -= _itemArr[i].Count;
        }
        return false;
    }
    public bool IsContainsItem(eItemType itemType, int subType)
    {
        for (int i = 0; i < _itemArr.Length; ++i)
        {
            if (_itemArr[i] == null)
                continue;

            if (!_itemArr[i].IsIncludeItemType(itemType))
                continue;

            if (_itemArr[i].ItemData.SubType == subType)
                return true;
        }
        return false;
    }
    public bool IsContainsItem(int index) => IsContainsItem(index, 1);
    public bool IsPossiblePushItem(int slotIndex, Item item)
    {
        if (_globalConstraint != null)
        {
            if (!_globalConstraint.IsPushable(item))
                return false;
        }

        if (_slotInventoryConstraint != null)
        {
            if (_slotInventoryConstraint.ContainsKey(slotIndex))
                if (!_slotInventoryConstraint[slotIndex].IsPushable(item))
                    return false;
        }

        return true;
    }
    public bool IsPossiblePushItem(Item item)
    {
        if (_globalConstraint != null)
        {
            if (!_globalConstraint.IsPushable(item))
                return false;
        }

        for (int i = 0; i < _itemArr.Length; ++i)
        {
            if (_slotInventoryConstraint != null)
            {
                if (_slotInventoryConstraint.ContainsKey(i))
                    if (!_slotInventoryConstraint[i].IsPushable(item))
                        continue;
            }

            if (_itemArr[i] == null)
                return true;

            if(_itemArr[i].Index == item.Index)
                if (_itemArr[i].ItemData.MergeCount > _itemArr[i].Count)
                    return true;
        }
        return false;
    }
    public bool PushNotMergeItem(Item item)
    {
        if (_globalConstraint != null)
        {
            if (!_globalConstraint.IsPushable(item))
                return false;
        }

        for (int i = 0; i < _itemArr.Length; ++i)
        {
            if (_slotInventoryConstraint != null)
            {
                if (_slotInventoryConstraint.ContainsKey(i))
                    if (!_slotInventoryConstraint[i].IsPushable(item))
                        continue;
            }

            if (_itemArr[i] == null)
            {
                _itemArr[i] = item;
                if (OnSlotChangedCallback != null)
                    OnSlotChangedCallback(i);

                return true;
            }
        }
        return false;
    }
    public bool PushMergeItem(Item item, ref int nullSlot)
    {
        if (_globalConstraint != null)
        {
            if (!_globalConstraint.IsPushable(item))
                return false;
        }

        for (int i = 0; i < _itemArr.Length; ++i)
        {
            if (_slotInventoryConstraint != null)
            {
                if (_slotInventoryConstraint.ContainsKey(i))
                    if (!_slotInventoryConstraint[i].IsPushable(item))
                        continue;
            }

            if (_itemArr[i] == null)
            {
                if (nullSlot == -1)
                    nullSlot = i;

                continue;
            }

            if (_itemArr[i].Index == item.Index)
            {
                int marginCount = _itemArr[i].ItemData.MergeCount - _itemArr[i].Count;
                if (marginCount > item.Count)
                {
                    _itemArr[i].Count += item.Count;
                    if (OnSlotChangedCallback != null)
                        OnSlotChangedCallback(i);

                    return true;
                }
                else
                {
                    if (marginCount > 0)
                    {
                        _itemArr[i].Count += marginCount;
                        item.Count -= marginCount;
                        if (OnSlotChangedCallback != null)
                            OnSlotChangedCallback(i);
                    }
                }
            }
        }
        return false;
    }
    public bool PushItem(Item item)
    {
        if (_globalConstraint != null)
        {
            if (!_globalConstraint.IsPushable(item))
                return false;
        }

        int nullSlot = -1;
        if(item.ItemData.MergeCount > 1)
        {
            for (int i = 0; i < _itemArr.Length; ++i)
            {
                if (_slotInventoryConstraint != null)
                {
                    if (_slotInventoryConstraint.ContainsKey(i))
                        if (!_slotInventoryConstraint[i].IsPushable(item))
                            continue;
                }

                if (_itemArr[i] != null)
                {
                    if (_itemArr[i].Index == item.Index)
                    {
                        if (_itemArr[i].Count + item.Count > item.ItemData.MergeCount)
                        {
                            int count = item.ItemData.MergeCount - _itemArr[i].Count;
                            item.Count -= count;
                            _itemArr[i].Count = item.ItemData.MergeCount;
                            if (OnSlotChangedCallback != null)
                                OnSlotChangedCallback(i);
                        }
                        else
                        {
                            _itemArr[i].Count += item.Count;
                            if (OnSlotChangedCallback != null)
                                OnSlotChangedCallback(i);

                            return true;
                        }
                    }
                }
                else if (nullSlot == -1) 
                    nullSlot = i;
            }
        }
        else
        {
            for (int i = 0; i < _itemArr.Length; ++i)
            {
                if (_itemArr[i] == null)
                {
                    nullSlot = i;
                    break;
                }
            }
        }
        if (nullSlot != -1)
        {
            _itemArr[nullSlot] = item;
            if (OnSlotChangedCallback != null)
                OnSlotChangedCallback(nullSlot);

            return true;
        }

        return false;
    }
    /// <returns>해당 위치에 존재하였던 아이템</returns>
    public Item PushItemSlot(int invenNumber, Item item)
    {
        if (_globalConstraint != null)
        {
            if (!_globalConstraint.IsPushable(item))
                return item;
        }
        if (_slotInventoryConstraint != null)
        {
            if (_slotInventoryConstraint.ContainsKey(invenNumber))
                if (!_slotInventoryConstraint[invenNumber].IsPushable(item))
                    return item;
        }

        Item prevItem = null;
        Item currItem = null;
        if (_itemArr[invenNumber] != null)
        {
            if(_itemArr[invenNumber].Index == item.Index)
            {
                if(_itemArr[invenNumber].ItemData.MergeCount >= _itemArr[invenNumber].Count + item.Count)
                {
                    _itemArr[invenNumber].Count += item.Count;
                    if (OnSlotChangedCallback != null)
                        OnSlotChangedCallback(invenNumber);

                    return null;
                }
                else
                {
                    if(_itemArr[invenNumber].Count >= _itemArr[invenNumber].ItemData.MergeCount )
                    {
                        prevItem = _itemArr[invenNumber];
                        _itemArr[invenNumber] = item;
                        currItem = item;
                    }
                    else
                    {
                        int margin = _itemArr[invenNumber].ItemData.MergeCount - _itemArr[invenNumber].Count;
                        _itemArr[invenNumber].Count += margin;
                        item.Count -= margin;
                        if (OnSlotChangedCallback != null)
                            OnSlotChangedCallback(invenNumber);

                        return item;
                    }
                }
            }
            else
            {
                prevItem = _itemArr[invenNumber];
                _itemArr[invenNumber] = item;
            }
        }
        else
        {
            prevItem = _itemArr[invenNumber];
            _itemArr[invenNumber] = item;
        }
        if (OnSlotChangedCallback != null)
            OnSlotChangedCallback(invenNumber);
        if (OnSelectionChangedCallback != null)
            OnSelectionChangedCallback(invenNumber, prevItem, item);
        return prevItem;
    }
    public Item PopItemSlot(int invenNumber)
    {
        Item temp = _itemArr[invenNumber];
        if (temp != null)
        {
            _itemArr[invenNumber] = null;
            if (OnSlotChangedCallback != null)
                OnSlotChangedCallback(invenNumber);
            if (OnSelectionChangedCallback != null)
                OnSelectionChangedCallback(invenNumber, temp, null);
        }
        return temp;
    }
    public Item PopItemSlot(int invenNumber, int count)
    {
        if (count == 0)
            return PopItemSlot(invenNumber);

        Item popItem = _itemArr[invenNumber];
        if (popItem != null)
        {
            if (popItem.Count == count)
                return PopItemSlot(invenNumber);

            int newCount = popItem.Count - count;
            popItem.Count = count;
            Item temp = popItem.Clone();
            temp.Count = newCount;
            _itemArr[invenNumber] = temp;
            if (OnSlotChangedCallback != null)
                OnSlotChangedCallback(invenNumber);
            if (OnSelectionChangedCallback != null)
                OnSelectionChangedCallback(invenNumber, popItem, popItem);
        }
        return popItem;
    }
    public Item PopItem(int index, int count)
    {
        Item item = Item.NewItem(index, 0);
        for(int i = _itemArr.Length - 1; i >= 0; --i)
        {
            if (_itemArr[i] == null)
                continue;

            if (_itemArr[i].Index != index)
                continue;

            int requireCount = count - item.Count;
            if (_itemArr[i].Count > requireCount)
            {
                item.Count += requireCount;
                _itemArr[i].Count -= requireCount;
                if (OnSlotChangedCallback != null)
                    OnSlotChangedCallback(i);
            }
            else
            {
                item.Count += _itemArr[i].Count;
                _itemArr[i] = null;
                if (OnSlotChangedCallback != null)
                    OnSlotChangedCallback(i);
            }

            if (item.Count >= count)
                return item;
        }
        if (item.Count > 0)
            return item;

        return null;
    }
    public Item PopItem(int index)
    {
        for (int i = _itemArr.Length - 1; i >= 0; --i)
        {
            if (_itemArr[i] == null)
                continue;

            if (_itemArr[i].ItemData.Index != index)
                continue;

            Item temp = _itemArr[i];
            _itemArr[i] = null;
            OnSlotChangedCallback?.Invoke(i);
            return temp;
        }
        return null;
    }
    #region Network
    public int GetByteSize
    {
        get {
            int size = ReliableHelper.ULongSize + ReliableHelper.IntSize + ReliableHelper.IntSize;
            for (int i = 0; i < _itemArr.Length; ++i)
                if (_itemArr[i] != null)
                    size += ReliableHelper.IntSize + _itemArr[i].GetByteSize;

            return size;
        }
    }
    public void EnqueueByte()
    {
        BaseEventSender.CopyBytes(_inventoryID);
        if (_inventoryID != 0)
        {
            BaseEventSender.CopyBytes(_itemArr.Length);
            int count = 0;
            for (int i = 0; i < _itemArr.Length; ++i)
                if (_itemArr[i] != null)
                    ++count;
            BaseEventSender.CopyBytes(count);
            for (int i = 0; i < _itemArr.Length; ++i)
                if (_itemArr[i] != null)
                {
                    BaseEventSender.CopyBytes(i);
                    _itemArr[i].EnqueueByte();
                }
        }
    }
    #endregion
    #region Save & Load
    public static Inventory LoadInventory(SaveController saveController)
    {
        ulong inventoryID = saveController.GetUInt64();
        if (inventoryID == 0)
            return null;

        int capacity = saveController.GetInt32();
        Inventory inventory = new Inventory(inventoryID, capacity);
        int count = saveController.GetInt32();
        for(int i = 0; i < count; ++i)
        {
            int position = saveController.GetInt32();
            inventory._itemArr[position] = Item.LoadItem(saveController);
        }
        return inventory;
    }
    public void EnqueueByte(SaveController saveController)
    {
        saveController.CopyBytes(_inventoryID);
        if (_inventoryID != 0)
        {
            saveController.CopyBytes(_itemArr.Length);
            int count = 0;
            for (int i = 0; i < _itemArr.Length; ++i)
                if (_itemArr[i] != null)
                    ++count;
            saveController.CopyBytes(count);
            for (int i = 0; i < _itemArr.Length; ++i)
                if (_itemArr[i] != null)
                {
                    saveController.CopyBytes(i);
                    _itemArr[i].EnqueueByte(saveController);
                }
        }
    }
    #endregion
}
