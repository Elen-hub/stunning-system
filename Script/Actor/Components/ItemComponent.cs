using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemComponent : BaseComponent
{
    Inventory _defaultBag;
    public Inventory GetDefaultBag => _defaultBag;
    Inventory _expansionBag;
    public Inventory GetExpansionBag => _expansionBag;
    Inventory[] _quickSlotInventory = new Inventory[2];
    public Inventory[] GetQuickSlotInventory => _quickSlotInventory;
    int _selectSlot = 0;
    int _selectQuickSlot = 0;
    public int CurrentSelectSlot => _selectSlot;
    public int CurrentSelectQuickSlot => _selectQuickSlot;
    public event System.Action<int, int> OnSelectSlot;
    public event System.Action<int> OnQuickSlotSwapCallback;

    static int _itemColliderLayerMask = LayerMask.GetMask("Item");
    Collider2D[] _colliderArr = new Collider2D[ClientConst.MaxPickupColliderCapacity];

    bool _isPossiblePickup = true;
    const float _pickupTargetTime = 0.5f;
    float _pickupWaitElapsedTime = 0f;

    int _selectIndex = 1;
    public Item SelectItem => _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot);

    bool _isLeftDown;
    bool _isRightDown;
    public ItemComponent(IActor actor) : base(actor, eComponent.ItemComponent)
    {
        AddEventMethods(eComponentEvent.SelectQuickSlot, OnReceiveEventSelectQuickSlot);
        AddEventMethods(eComponentEvent.MouseLeftDown, OnReceiveEventMouseLeftDown);
        AddEventMethods(eComponentEvent.MouseLeftUp, OnReceiveEventMouseLeftUp);
        AddEventMethods(eComponentEvent.MouseRightDown, OnReceiveEventMouseRightDown);
        AddEventMethods(eComponentEvent.MouseRightUp, OnReceiveEventMouseRightUp);
        AddEventMethods(eComponentEvent.PickupItem, OnReceiveEventPickupItem);
        AddEventMethods(eComponentEvent.Reload, OnReceiveEventReload);

        _defaultBag = ItemManager.Instance.InstanceInventory(ClientConst.DefaultBagCapacity);
        _quickSlotInventory = new Inventory[2] {
            ItemManager.Instance.InstanceInventory(8),
            ItemManager.Instance.InstanceInventory(8),
        };
        _quickSlotInventory[_selectQuickSlot].OnSelectionChangedCallback += OnChangedSelectionQuickSlot;
    }
    void OnChangedSelectionQuickSlot(int changedNumber, Item prevItem, Item currItem)
    {
        if (_selectSlot == changedNumber)
        {
            if (prevItem != null) prevItem.OnExitSlot(_owner);
            if (currItem != null) currItem.OnEnterSlot(_owner);
        }
    }
    public void SwapQuickSlot()
    {
        if (_quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot) != null)
            _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot).OnExitSlot(_owner);

        _quickSlotInventory[_selectQuickSlot].OnSelectionChangedCallback -= OnChangedSelectionQuickSlot;
        ++_selectQuickSlot;
        if (_selectQuickSlot >= _quickSlotInventory.Length)
            _selectQuickSlot = 0;

        OnQuickSlotSwapCallback(_selectQuickSlot);
        _quickSlotInventory[_selectQuickSlot].OnSelectionChangedCallback += OnChangedSelectionQuickSlot;
        if (_quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot) != null)
            _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot).OnEnterSlot(_owner);
    }
    public bool IsContainsItem(int itemIndex)
    {
        if (_defaultBag.IsContainsItem(itemIndex))
            return true;

        if (_expansionBag != null)
        {
            if (_expansionBag.IsContainsItem(itemIndex))
                return true;
        }

        for (int i = 0; i < _quickSlotInventory.Length; ++i)
            if (_quickSlotInventory[i].IsContainsItem(itemIndex))
                return true;

        return false;
    }
    public bool IsContainsItem(eItemType type, int subType) 
    {
        if (_defaultBag.IsContainsItem(type, subType))
            return true;

        if (_expansionBag != null)
        {
            if (_expansionBag.IsContainsItem(type, subType))
                return true;
        }

        for (int i = 0; i < _quickSlotInventory.Length; ++i)
            if (_quickSlotInventory[i].IsContainsItem(type, subType))
                return true;

        return false;
    }
    public bool IsPossiblePushItem(Item item)
    {
        if (_defaultBag.IsPossiblePushItem(item))
            return true;

        if (_expansionBag != null)
        {
            if (_expansionBag.IsPossiblePushItem(item))
                return true;
        }

        for (int i = 0; i < _quickSlotInventory.Length; ++i)
            if (_quickSlotInventory[i].IsPossiblePushItem(item))
                return true;

        return false;
    }
    public Item PushItem(Item item)
    {
        if (item.ItemData.MergeCount > 1)
        {
            int nullSlot = -1;
            ulong inventoryID = 0;
            int firstCount = item.Count;
            if (OnPushInventory(_defaultBag, item, ref nullSlot, ref inventoryID))
                return null;

            if (_expansionBag != null)
            {
                if (OnPushInventory(_expansionBag, item, ref nullSlot, ref inventoryID))
                    return null;
            }

            for (int i = 0; i < _quickSlotInventory.Length; ++i)
                if (OnPushInventory(_quickSlotInventory[i], item, ref nullSlot, ref inventoryID))
                    return null;

            if (item.Count > 0 && inventoryID != 0)
            {
                if (_defaultBag != null && _defaultBag.InventoryID == inventoryID)
                    _defaultBag.PushItemSlot(nullSlot, item);

                if (_expansionBag != null && _expansionBag.InventoryID == inventoryID)
                    _expansionBag.PushItemSlot(nullSlot, item);

                for (int i = 0; i < _quickSlotInventory.Length; ++i)
                    if (_quickSlotInventory[i] != null && _quickSlotInventory[i].InventoryID == inventoryID)
                    {
                        _quickSlotInventory[i].PushItemSlot(nullSlot, item);
                        break;
                    }

                return null;
            }
        }
        else
        {
            if (_defaultBag != null)
                if (_defaultBag.PushNotMergeItem(item))
                    return null;

            if (_expansionBag != null)
                if (_expansionBag.PushNotMergeItem(item))
                    return null;

            for (int i = 0; i < _quickSlotInventory.Length; ++i)
            {
                if (_quickSlotInventory[i].PushNotMergeItem(item))
                    return null;
            }
        }

        return item;
    }
    public Item PopItem(int index, int count)
    {
        Item item = Item.NewItem(index, 0);
        if (_expansionBag != null)
            item += _expansionBag.PopItem(index, count - item.Count);

        if(item.Count < count)
        {
            item += _defaultBag.PopItem(index, count);
            if (item.Count < count)
            {
                for (int i = 0; i < _quickSlotInventory.Length; ++i)
                {
                    item += _quickSlotInventory[i].PopItem(index, count - item.Count);
                    if (item.Count >= count)
                        return item;
                }
            }
        }
        if (item.Count > 0)
            return item;

        return null;
    }
    public Item PopItem(int index)
    {
        if (_expansionBag != null)
        {
            Item item = _expansionBag.PopItem(index);
            if (item != null)
                return item;
        }
        if(_defaultBag != null)
        {
            Item item = _defaultBag.PopItem(index);
            if (item != null)
                return item;
        }
        for (int i = 0; i < _quickSlotInventory.Length; ++i)
        {
            Item item = _quickSlotInventory[i].PopItem(index);
            if (item != null)
                return item;
        }
        return null;
    }
    /// <summary>
    /// 가질 수 있는 아이템인지 체크.
    /// </summary>
    /// <returns>가질 수 있는 양. 가지지 못할경우 0</returns>
    bool IsPossiblePickupItem(Item item)
    {
        if (_defaultBag.IsPossiblePushItem(item))
            return true;

        if (_expansionBag != null)
        {
            if (_expansionBag.IsPossiblePushItem(item))
                return true;
        }

        for (int i = 0; i < _quickSlotInventory.Length; ++i)
            if (_quickSlotInventory[i].IsPossiblePushItem(item))
                return true;

        return false;
    }
    public void PickupItem()
    {
        int collisionCount = Physics2D.OverlapCircleNonAlloc(_owner.Position, ClientConst.PickupItemOverlapRadius, _colliderArr, _itemColliderLayerMask);
        for(int i = 0; i<collisionCount; ++i)
        {
            ItemObject itemObject = _colliderArr[i].GetComponent<ItemObject>();
            if(itemObject != null)
            {
                if (IsPossiblePickupItem(itemObject.GetItem))
                {
                    _pickupWaitElapsedTime = 0f;
                    _isPossiblePickup = false;
                    NetworkManager.Instance.ItemEventSender.RequestPickupCheck(itemObject.WorldID);
                }
            }
        }
    }
    void OnReceiveEventPickupItem(object[] messageArr)
    {
        ulong itemID = (ulong)messageArr[0];
        ItemObject itemObject = ItemManager.Instance.GetItem(itemID);
        PushInventory(itemObject);
    }
    void OnReceiveEventReload(object[] messageArr)
    {
        if (SelectItem == null) return;
        IReloadable reloadable = SelectItem as IReloadable;
        if(reloadable != null)
        {
            if (reloadable.IsPossibleReload(_owner))
                reloadable.Reload(_owner);
        }
    }
    bool OnPushInventory(Inventory inventory, Item item, ref int nullSlot, ref ulong inventoryID)
    {
        if (inventory == null)
            return false;

        if(inventory.PushMergeItem(item, ref nullSlot))
        {
            return true;
        }
        if (inventoryID == 0 && nullSlot != -1)
            inventoryID = inventory.InventoryID;

        return false;
    }
    void PushInventory(ItemObject itemObject)
    {
        if (itemObject.GetItem.ItemData.MergeCount > 1)
        {
            int nullSlot = -1;
            ulong inventoryID = 0;
            int firstCount = itemObject.GetItem.Count;
            if(OnPushInventory(_defaultBag, itemObject.GetItem, ref nullSlot, ref inventoryID))
            {
                NetworkManager.Instance.ItemEventSender.RequestPickupItem(_owner.WorldID, itemObject.WorldID, true);
                itemObject.Pickup(_owner.transform);
                return;
            }
            if (_expansionBag != null)
            {
                if (OnPushInventory(_expansionBag, itemObject.GetItem, ref nullSlot, ref inventoryID))
                {
                    NetworkManager.Instance.ItemEventSender.RequestPickupItem(_owner.WorldID, itemObject.WorldID, true);
                    itemObject.Pickup(_owner.transform);
                    return;
                }
            }
            for (int i = 0; i < _quickSlotInventory.Length; ++i)
                if (OnPushInventory(_quickSlotInventory[i], itemObject.GetItem, ref nullSlot, ref inventoryID))
                {
                    NetworkManager.Instance.ItemEventSender.RequestPickupItem(_owner.WorldID, itemObject.WorldID, true);
                    itemObject.Pickup(_owner.transform);
                    return;
                }
            if (itemObject.GetItem.Count > 0 && inventoryID != 0)
            {
                if (_defaultBag != null && _defaultBag.InventoryID == inventoryID)
                    _defaultBag.PushItemSlot(nullSlot, itemObject.GetItem);

                if (_expansionBag != null &&_expansionBag.InventoryID == inventoryID)
                    _expansionBag.PushItemSlot(nullSlot, itemObject.GetItem);

                for (int i = 0; i<_quickSlotInventory.Length; ++i)
                    if(_quickSlotInventory[i] != null && _quickSlotInventory[i].InventoryID == inventoryID)
                    {
                        _quickSlotInventory[i].PushItemSlot(nullSlot, itemObject.GetItem);
                        break;
                    }

                NetworkManager.Instance.ItemEventSender.RequestPickupItem(_owner.WorldID, itemObject.WorldID, true);
                itemObject.Pickup(_owner.transform);
                return;
            }

            if(firstCount != itemObject.GetItem.Count)
            {
                NetworkManager.Instance.ItemEventSender.RequestPickupItem(_owner.WorldID, itemObject.WorldID, true);
                itemObject.Pickup(_owner.transform);
                NetworkManager.Instance.ItemEventSender.RequestSpawnItem(itemObject.GetItem, _owner.Position);
            }
            else
            {
                NetworkManager.Instance.ItemEventSender.RequestPickupItem(_owner.WorldID, itemObject.WorldID, false);
            }
        }
        else
        {
            bool isPushed = false;
            if (_defaultBag != null)
                isPushed = _defaultBag.PushNotMergeItem(itemObject.GetItem);
            if (!isPushed && _expansionBag != null)
                isPushed = _expansionBag.PushNotMergeItem(itemObject.GetItem);
            if (!isPushed)
            {
                for (int i = 0; i < _quickSlotInventory.Length; ++i)
                {
                    isPushed = _quickSlotInventory[i].PushNotMergeItem(itemObject.GetItem);
                    if (isPushed)
                        break;
                }
            }
            NetworkManager.Instance.ItemEventSender.RequestPickupItem(_owner.WorldID, itemObject.WorldID, true);
            itemObject.Pickup(_owner.transform);
        }
    }
    void OnReceiveEventSelectQuickSlot(object[] messageArr)
    {
        int slotNumber = (int)messageArr[0];
        if (_selectSlot == slotNumber)
            return;

        if (_quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot) != null)
        {
            _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot).OnExitSlot(_owner);
            _selectIndex = 1;
        }
        _selectSlot = slotNumber;

        if(OnSelectSlot != null)
            OnSelectSlot(_selectQuickSlot, _selectSlot);

        if (_quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot) != null)
        {
            _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot).OnEnterSlot(_owner);
            _selectIndex = _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot).ItemData.ReferenceIndex;
        }
    }
    void OnReceiveEventMouseLeftDown(params object[] messageArr)
    {
        Item item = _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot);
        if (item != null)
        {
            if (item.IsIncludeItemType(eItemType.UseItem))
            {
                UseItem useItem = item as UseItem;
                if(useItem.ItemData.ReferenceIndex != 0)
                {
                    if (useItem.GetSkill.GetDefaultData.SkillInputType == eSkillInputType.Single)
                    {
                        if (useItem.IsUseable(_owner))
                        {
                            useItem.OnUseStart(_owner);
                            if (useItem.Count <= 0)
                            {
                                useItem.OnExitSlot(_owner);
                                _quickSlotInventory[_selectQuickSlot].PopItemSlot(_selectSlot);
                            }
                            _quickSlotInventory[_selectQuickSlot].SlotChangedCallback(_selectSlot);
                            return;
                        }
                    }
                }
            }
        }
        _isLeftDown = true;
    }
    void OnReceiveEventMouseLeftUp(params object[] messageArr)
    {
        _isLeftDown = false;
    }
    void OnReceiveEventMouseRightDown(params object[] messageArr)
    {
        Item item = _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot);
        if (item != null)
        {
            if (item.IsIncludeItemType(eItemType.UseItem))
            {
                UseItem useItem = item as UseItem;
                if (useItem.ItemData.ExReferenceIndex != 0)
                {
                    if (useItem.GetExSkill.GetDefaultData.SkillInputType == eSkillInputType.Single)
                    {
                        if (useItem.IsSpecialUseable(_owner))
                        {
                            useItem.OnSpecialUseStart(_owner);
                            if (useItem.Count <= 0)
                            {
                                useItem.OnExitSlot(_owner);
                                _quickSlotInventory[_selectQuickSlot].PopItemSlot(_selectSlot);
                            }
                            _quickSlotInventory[_selectQuickSlot].SlotChangedCallback(_selectSlot);
                            return;
                        }
                    }
                }
            }
        }
        _isRightDown = true;
    }
    void OnReceiveEventMouseRightUp(params object[] messageArr)
    {
        _isRightDown = false;
    }
    void OnUseDefaultSkill()
    {
        Item item = _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot);
        if (item != null)
        {
            if (item.IsIncludeItemType(eItemType.UseItem))
            {
                UseItem useItem = item as UseItem;
                if (useItem.ItemData.ReferenceIndex != 0)
                {
                    if (useItem.GetSkill.GetDefaultData.SkillInputType == eSkillInputType.Single)
                    {
                        _isLeftDown = false;
                        return;
                    }

                    if (useItem.IsUseable(_owner))
                    {
                        useItem.OnUseStart(_owner);
                        if (useItem.Count <= 0)
                        {
                            useItem.OnExitSlot(_owner);
                            _quickSlotInventory[_selectQuickSlot].PopItemSlot(_selectSlot);
                        }
                        _quickSlotInventory[_selectQuickSlot].SlotChangedCallback(_selectSlot);
                        return;
                    }
                }
            }
        }
        if (DataManager.Instance.SkillTable[1].Skill.IsUseable(_owner))
            DataManager.Instance.SkillTable[1].Skill.StartSequence(_owner, _owner.Direction);
    }
    void OnUseExSkill()
    {
        Item item = _quickSlotInventory[_selectQuickSlot].GetItem(_selectSlot);
        if (item != null)
        {
            if (item.IsIncludeItemType(eItemType.UseItem))
            {
                UseItem useItem = item as UseItem;
                if (useItem.ItemData.ExReferenceIndex != 0)
                {
                    if (useItem.GetExSkill.GetDefaultData.SkillInputType == eSkillInputType.Single)
                    {
                        _isRightDown = false;
                        return;
                    }

                    if (useItem.IsSpecialUseable(_owner))
                    {
                        useItem.OnSpecialUseStart(_owner);
                        if (useItem.Count <= 0)
                        {
                            useItem.OnExitSlot(_owner);
                            _quickSlotInventory[_selectQuickSlot].PopItemSlot(_selectSlot);
                        }
                        _quickSlotInventory[_selectQuickSlot].SlotChangedCallback(_selectSlot);
                        return;
                    }
                }
            }
        }
    }
    protected override void OnClientUpdate(float deltaTime)
    {
        if (!_isPossiblePickup)
        {
            _pickupWaitElapsedTime += TimeManager.DeltaTime;
            if (_pickupWaitElapsedTime > _pickupTargetTime)
                _isPossiblePickup = true;
        }
        if (_isLeftDown)
            OnUseDefaultSkill();
        if (_isRightDown)
            OnUseExSkill();
    }
}
