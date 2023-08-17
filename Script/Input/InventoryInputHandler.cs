using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class InventoryInputHandler : BaseInputHandler
{
    const string _itemSlotTag = "ItemSlot";
    const string _equipSlotTag = "EquipSlot";
    public UnityEngine.Events.UnityAction OnShiftLeftClick;
    public ItemComponent ItemComponent;
    public ulong SubInventory;
    Inventory GetSubInventory => ItemManager.Instance.GetInventory(SubInventory);
    void OnUpdateMouseInput()
    {
        if (IsKeyDown(eInputType.MouseLeft))
        {
            if (OnInputItemSlot())
                return;

            if (OnInputEquipSlot())
                return;

            if (ItemManager.Instance.SelectionItem != null)
            {
                if (PlayerManager.Instance.Me.Character != null)
                {
                    NetworkManager.Instance.ItemEventSender.RequestSpawnItem(ItemManager.Instance.SelectionItem, PlayerManager.Instance.Me.Character.Position);
                    ItemManager.Instance.SelectionItem = null;
                    UIManager.Instance.Close(eUIName.USelectionItem);
                }
            }
        }
    }
    bool OnInputItemSlot()
    {
        List<RaycastResult> raycastResult = UIManager.Instance.GetRaycast;
        for(int i = 0; i < raycastResult.Count; ++i)
        {
            GameObject element = raycastResult[i].gameObject;
            if (element.CompareTag(_itemSlotTag))
            {
                ItemSlot slot = element.gameObject.GetComponent<ItemSlot>();
                if (slot == null)
                    return false;

                bool isShift = IsKeyInput(eInputType.Shift);
                if (isShift)
                {
                    if (!slot.IsPossiblePush)
                        return false;

                    if (slot.GetItem == null)
                        return false;

                    if (SubInventory != 0)
                    {
                        // 서브인벤토리가 활성화 중일때
                        if (GetSubInventory.InventoryID == slot.InventoryID)
                        {
                            // 서브 인벤토리에서의 동작
                            if (ItemComponent.IsPossiblePushItem(slot.GetItem))
                            {
                                Item item = ItemManager.Instance.GetInventory(slot.InventoryID).PopItemSlot(slot.SlotArrayPosition);
                                if (slot.InventoryID >= 100)
                                    NetworkManager.Instance.ItemEventSender.RequestPopItemSlot(slot.InventoryID, slot.SlotArrayPosition, item.Count);

                                Item remainItem = ItemComponent.PushItem(item);
                                if (remainItem != null)
                                {
                                    if (slot.InventoryID >= 100)
                                        NetworkManager.Instance.ItemEventSender.RequestPushItemSlot(slot.InventoryID, slot.SlotArrayPosition, remainItem);

                                    ItemManager.Instance.GetInventory(slot.InventoryID).PushItemSlot(slot.SlotArrayPosition, remainItem);
                                }
                            }
                        }
                        else
                        {
                            // 메인 인벤토리에서의 동작
                            if (GetSubInventory.IsPossiblePushItem(slot.GetItem))
                            {
                                Item item = ItemManager.Instance.GetInventory(slot.InventoryID).PopItemSlot(slot.SlotArrayPosition);
                                if (GetSubInventory.InventoryID >= 100)
                                    NetworkManager.Instance.ItemEventSender.RequestPushItem(GetSubInventory.InventoryID, item);
                                GetSubInventory.PushItem(item);
                            }
                        }
                    }
                    else
                    {
                        // 메인인벤토리만 활성화 중일때 (장비 조작)
                        EquipmentComponent equipmentComponent = ItemComponent.Owner.GetComponent<EquipmentComponent>(eComponent.EquipmentComponent);
                        if (slot.GetItem.IsIncludeItemType(eItemType.EquipItem))
                        {
                            switch (slot.GetItem.ItemData.Type)
                            {
                                case eItemType.Hat:
                                    OnEquipment(equipmentComponent, eEquipSlotType.Hat, slot);
                                    break;
                                case eItemType.Armor:
                                    OnEquipment(equipmentComponent, eEquipSlotType.Armor, slot);
                                    break;
                                case eItemType.Shoes:
                                    OnEquipment(equipmentComponent, eEquipSlotType.Shoes, slot);
                                    break;
                                case eItemType.Necklace:
                                    OnEquipment(equipmentComponent, eEquipSlotType.Necklace, slot);
                                    break;
                                case eItemType.Ring:
                                    {
                                        if (IsContainsEquipSlot(equipmentComponent, eEquipSlotType.Ring1))
                                        {
                                            if (IsContainsEquipSlot(equipmentComponent, eEquipSlotType.Ring2))
                                                OnEquipment(equipmentComponent, eEquipSlotType.Ring1, slot);
                                            else
                                                OnEquipment(equipmentComponent, eEquipSlotType.Ring2, slot);
                                        }
                                        else
                                            OnEquipment(equipmentComponent, eEquipSlotType.Ring1, slot);
                                    }
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    SelectionItem(slot);
                }
                return true;
            }
        }
        return false;
    }
    public bool IsContainsEquipSlot(EquipmentComponent equipmentComponent, eEquipSlotType equipType) => equipmentComponent.IsContainsItem(equipType);
    void OnEquipment(EquipmentComponent equipmentComponent, eEquipSlotType equipType, ItemSlot itemSlot)
    {
        if (equipmentComponent.IsContainsItem(equipType))
        {
            Item item = itemSlot.GetItem;
            Item remainItem = equipmentComponent.PopSlot(equipType);
            ItemManager.Instance.GetInventory(itemSlot.InventoryID).PushItemSlot(itemSlot.SlotArrayPosition, remainItem);
            equipmentComponent.PushItem(equipType, (EquipItem)item);
        }
        else
        {
            Item item = ItemManager.Instance.GetInventory(itemSlot.InventoryID).PopItemSlot(itemSlot.SlotArrayPosition);
            equipmentComponent.PushItem(equipType, (EquipItem)item);
        }
    }
    bool OnInputEquipSlot()
    {
        List<RaycastResult> raycastResult = UIManager.Instance.GetRaycast;
        for(int i = 0; i < raycastResult.Count; ++i)
        {
            GameObject element = raycastResult[0].gameObject;
            if(element.gameObject.CompareTag(_equipSlotTag))
            {
                EquipSlot slot = element.gameObject.GetComponent<EquipSlot>();
                if (slot == null)
                    return false;

                bool isShift = IsKeyInput(eInputType.Shift);
                if (isShift)
                {
                    EquipmentComponent equipmentComponent = ItemComponent.Owner.GetComponent<EquipmentComponent>(eComponent.EquipmentComponent);
                    if (equipmentComponent.IsContainsItem(slot.EquipSlotType))
                    {
                        if (ItemComponent.IsPossiblePushItem(equipmentComponent.GetItem(slot.EquipSlotType)))
                        {
                            Item item = equipmentComponent.PopSlot(slot.EquipSlotType);
                            ItemComponent.PushItem(item);
                        }
                    }
                }
                else
                {
                    SelectionItem(slot);
                }
                return true;
            }
        }
        return false;
    }
    void SelectionItem(ItemSlot slot)
    {
        if (ItemManager.Instance.SelectionItem != null)
        {
            if (!slot.IsPossiblePush)
                return;

            Inventory inventory = ItemManager.Instance.GetInventory(slot.InventoryID);
            if(inventory.IsPossiblePushItem(slot.SlotArrayPosition, ItemManager.Instance.SelectionItem))
            {
                if (slot.InventoryID >= 100)
                    NetworkManager.Instance.ItemEventSender.RequestPushItemSlot(slot.InventoryID, slot.SlotArrayPosition, ItemManager.Instance.SelectionItem);

                Item item = inventory.PushItemSlot(slot.SlotArrayPosition, ItemManager.Instance.SelectionItem);
                ItemManager.Instance.SelectionItem = item;

                if (item != null) UIManager.Instance.Open(eUIName.USelectionItem);
                else UIManager.Instance.Close(eUIName.USelectionItem);
            }
        }
        else
        {
            Item item = ItemManager.Instance.GetInventory(slot.InventoryID).PopItemSlot(slot.SlotArrayPosition);
            if (item != null)
            {
                if (slot.InventoryID >= 100)
                    NetworkManager.Instance.ItemEventSender.RequestPopItemSlot(slot.InventoryID, slot.SlotArrayPosition, item.Count);

                ItemManager.Instance.SelectionItem = item;
                UIManager.Instance.Open(eUIName.USelectionItem);
            }
        }
    }
    void SelectionItem(EquipSlot slot)
    {
        if (ItemManager.Instance.SelectionItem != null)
        {
            Debug.Log(ItemManager.Instance.SelectionItem.ItemData.Type);
            if (ItemManager.Instance.SelectionItem.ItemData.Type == slot.AccessItemType)
            {
                Debug.Log("SelectionStart2");
                // 맞는 장비 아이템 슬롯에 넣을 경우
                EquipmentComponent equipmentComponent = ItemComponent.Owner.GetComponent<EquipmentComponent>(eComponent.EquipmentComponent);
                Item item = equipmentComponent.PushItem(slot.EquipSlotType, ItemManager.Instance.SelectionItem as EquipItem);
                ItemManager.Instance.SelectionItem = item;
                if (item != null) UIManager.Instance.Open(eUIName.USelectionItem);
                else UIManager.Instance.Close(eUIName.USelectionItem);
            }
            //else
            //{
            //    // 다른 장비 아이템 슬롯에 넣을 경우 인벤토리로 반환체크
            //    if (ItemComponent.IsPossiblePushItem(ItemManager.Instance.SelectionItem))
            //        ItemComponent.PushItem(ItemManager.Instance.SelectionItem);
            //    else
            //        NetworkManager.Instance.ItemEventSender.RequestSpawnItem(ItemManager.Instance.SelectionItem, PlayerManager.Instance.Me.Character.Position);

            //    ItemManager.Instance.SelectionItem = null;
            //    UIManager.Instance.Close(eUIName.USelectionItem);
            //}
        }
        else
        {
            EquipmentComponent equipmentComponent = ItemComponent.Owner.GetComponent<EquipmentComponent>(eComponent.EquipmentComponent);
            if(equipmentComponent.IsContainsItem(slot.EquipSlotType))
            {
                Item item = equipmentComponent.PopSlot(slot.EquipSlotType);
                ItemManager.Instance.SelectionItem = item;
                UIManager.Instance.Open(eUIName.USelectionItem);
            }
        }
    }
    void OnCloseInventory()
    {
        if (IsKeyDown(eInputType.OpenInventory))
        {
            UInventory inventory = UIManager.Instance.Get<UInventory>(eUIName.UInventory);
            if (inventory.SubInventoryIndex == 0)
                UIManager.Instance.Close(eUIName.UInventory);
        }
    }
    protected override void OnUpdate()
    {
        OnUpdateMouseInput();
        OnCloseInventory();
    }
}
