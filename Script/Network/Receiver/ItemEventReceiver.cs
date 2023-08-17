using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public class ItemEventReceiver : BaseEventReceiver
{
    public ItemEventReceiver()
    {
        MappingReceiveEvent(eItemEventCode.NotifyInstanceInventory, OnNotifyInstanceInventory);
        MappingReceiveEvent(eItemEventCode.RequestSpawnItem, OnReplySpawnItem);
        MappingReceiveEvent(eItemEventCode.NotifySpawnItem, OnNotifySpawnItem);
        MappingReceiveEvent(eItemEventCode.RequestPickupCheck, OnReplyPickupCheck);
        MappingReceiveEvent(eItemEventCode.NotifyPickupCheck, OnNotifyPickupCheck);
        MappingReceiveEvent(eItemEventCode.RequestPickupItem, OnReplyPickupItem);
        MappingReceiveEvent(eItemEventCode.NotifyPickupItem, OnNotifyPickupItem);        
        MappingReceiveEvent(eItemEventCode.RequestPushItem, OnReplyPushItem);
        MappingReceiveEvent(eItemEventCode.RequestPushItemSlot, OnReplyPushItemSlot);
        MappingReceiveEvent(eItemEventCode.RequestPopItemSlot, OnReplyPopItemSlot);
        MappingReceiveEvent(eItemEventCode.NotifyPushItem, OnNotifyPushItem);
        MappingReceiveEvent(eItemEventCode.NotifyPushItemSlot, OnNotifyPushItemSlot);
        MappingReceiveEvent(eItemEventCode.NotifyPopItemSlot, OnNotifyPopItemSlot);
        MappingReceiveEvent(eItemEventCode.NotifyPopItem, OnNotifyPopItem);
        MappingReceiveEvent(eItemEventCode.RequestEquipItem, OnReplyEquipItem);
        MappingReceiveEvent(eItemEventCode.RequestUnequipItem, OnReplyUnequipItem);
    }
    bool OnNotifyInstanceInventory(int playerID)
    {
        Inventory inventory = GetInventory();
        ItemManager.Instance.AddInventory(inventory);
        DebugUtility.Log($"OnNotifyInstanceInventory - ID: {inventory.InventoryID} Capacity: {inventory.Capacity}");
        return true;
    }
    bool OnReplySpawnItem(int playerID)
    {
        Item item = GetItem();
        Vector2 worldPosition = GetVector2();
        ItemObject itemObject = ItemManager.Instance.SpawnItem(item, worldPosition);
        NetworkManager.Instance.ItemEventSender.NotifySpawnItem(itemObject);
        DebugUtility.Log($"OnReplySpawnItem - ItemIndex: {item.Index} WorldPosition: {worldPosition}");
        return true;
    }
    bool OnNotifySpawnItem(int playerID)
    {
        Item item = GetItem();
        ulong worldID = GetUInt64();
        Vector2 worldPosition = GetVector2();
        ItemManager.Instance.SpawnItem(item, worldPosition, worldID);
        DebugUtility.Log($"OnNotifySpawnItem - ItemIndex: {item.Index} ItemID: {worldID} WorldPosition: {worldPosition}");
        return true;
    }
    bool OnReplyPickupCheck(int playerID)
    {
        ulong itemID = GetUInt64();
        ItemObject itemObject = ItemManager.Instance.GetItem(itemID);
        if (itemObject == null)
            return false;

        if (!itemObject.IsPickupProcess)
        {
            itemObject.IsPickupProcess = true;
            NetworkManager.Instance.ItemEventSender.NotifyPickupCheck(playerID, itemID);
        }

        DebugUtility.Log($"OnNotifySpawnItem - ItemID: {itemID}");
        return true;
    }
    bool OnNotifyPickupCheck(int playerID)
    {
        ulong itemID = GetUInt64();
        PlayerManager.Instance.Me.Character.SendComponentMessage(eComponentEvent.PickupItem, itemID);
        DebugUtility.Log($"OnNotifyPickupCheck - ItemID: {itemID}");
        return true;
    }
    bool OnReplyPickupItem(int playerID)
    {
        ulong characterID = GetUInt64();
        ulong itemID = GetUInt64();
        bool isRemove = GetBoolean();
        ItemObject itemObject = ItemManager.Instance.GetItem(itemID);
        if (itemObject == null)
            return false;

        if (itemObject.IsPickupProcess)
        {
            if (isRemove)
            {
                itemObject.Destroy();
                NetworkManager.Instance.ItemEventSender.NotifyPickupItem(characterID, itemID, playerID);
            }
            else
            {
                itemObject.IsPickupProcess = false;
            }
        }
        DebugUtility.Log($"OnReplyPickupItem - CharacterID: {characterID} ItemID: {itemID}");
        return true;
    }
    bool OnNotifyPickupItem(int playerID)
    {
        ulong characterID = GetUInt64();
        ulong itemID = GetUInt64();

        IActor actor = ActorManager.Instance.GetActor(characterID);
        ItemObject itemObject = ItemManager.Instance.GetItem(itemID);
        if (actor != null) itemObject.Pickup(actor.transform);
        else itemObject.Destroy();
        DebugUtility.Log($"OnNotifyPickupItem - CharacterID: {characterID} ItemID: {itemID}");
        return true;
    }
    bool OnReplyPushItem(int playerID)
    {
        ulong inventoryID = GetUInt64();
        Item item = GetItem();
        Inventory inventory = ItemManager.Instance.GetInventory(inventoryID);
        if (inventory == null)
            return false;

        if (inventoryID >= 100)
            NetworkManager.Instance.ItemEventSender.NotifyPushItem(inventoryID, item, playerID);

        inventory.PushItem(item);
        DebugUtility.Log($"OnReplyPushItem - ID: {inventoryID} ItemIndex: {item.Index} ItemCount: {item.Count}");
        return true;
    }
    bool OnNotifyPushItem(int playerID)
    {
        ulong inventoryID = GetUInt64();
        Item item = GetItem();
        Inventory inventory = ItemManager.Instance.GetInventory(inventoryID);
        if (inventory == null)
            return false;

        inventory.PushItem(item);
        DebugUtility.Log($"OnNotifyPushItem - ID: {inventoryID} ItemIndex: {item.Index} ItemCount: {item.Count}");
        return true;
    }
    bool OnReplyPushItemSlot(int playerID)
    {
        ulong inventoryID = GetUInt64();
        int arrayNumber = GetInt32();
        Item item = GetItem();
        ItemManager.Instance.GetInventory(inventoryID).PushItemSlot(arrayNumber, item);
        if (inventoryID >= 100)
            NetworkManager.Instance.ItemEventSender.NotifyPushItemSlot(inventoryID, arrayNumber, item, playerID);

        DebugUtility.Log($"OnReplyPushItemSlot - ID: {inventoryID} SlotNumber: {arrayNumber} ItemIndex: {item.Index} ItemCount: {item.Count}");
        return true;
    }
    bool OnNotifyPushItemSlot(int playerID)
    {
        ulong inventoryID = GetUInt64();
        int arrayNumber = GetInt32();
        Inventory inventory = ItemManager.Instance.GetInventory(inventoryID);
        if (inventory == null)
            return false;

        Item item = GetItem();
        inventory.PushItemSlot(arrayNumber, item);
        DebugUtility.Log($"OnNotifyPushItemSlot - ID: {inventoryID} SlotNumber: {arrayNumber} ItemIndex: {item.Index} ItemCount: {item.Count}");
        return true;
    }
    bool OnReplyPopItemSlot(int playerID)
    {
        ulong inventoryID = GetUInt64();
        int arrayNumber = GetInt32();
        int count = GetInt32();
        ItemManager.Instance.GetInventory(inventoryID).PopItemSlot(arrayNumber, count);
        if (inventoryID >= 100)
            NetworkManager.Instance.ItemEventSender.NotifyPopItemSlot(inventoryID, arrayNumber, count, playerID);

        DebugUtility.Log($"OnReplyPopItemSlot - ID: {inventoryID} SlotNumber: {arrayNumber} ItemCount: {count}");
        return true;
    }
    bool OnNotifyPopItemSlot(int playerID)
    {
        ulong inventoryID = GetUInt64();
        int arrayNumber = GetInt32();
        int count = GetInt32();
        Inventory inventory = ItemManager.Instance.GetInventory(inventoryID);
        if (inventory == null)
            return false;

        inventory.PopItemSlot(arrayNumber, count);
        DebugUtility.Log($"OnNotifyPopItemSlot - ID: {inventoryID} SlotNumber: {arrayNumber} ItemCount: {count}");
        return true;
    }
    bool OnNotifyPopItem(int playerID)
    {
        ulong inventoryID = GetUInt64();
        int index = GetInt32();
        int count = GetInt32();
        Inventory inventory = ItemManager.Instance.GetInventory(inventoryID);
        if (inventory == null)
            return false;

        inventory.PopItem(index, count);
        DebugUtility.Log($"OnNotifyPopItem - ID: {inventoryID} ItemIndex: {index} ItemCount: {count}");
        return true;
    }
    bool OnReplyEquipItem(int playerID)
    {
        ulong characterID = GetUInt64();
        eEquipSlotType equipSlotType = (eEquipSlotType)GetInt32();
        Item item = GetItem();
        if (!item.IsIncludeItemType(eItemType.EquipItem))
            return false;

        IActor actor = ActorManager.Instance.GetActor(characterID);
        if (actor == null)
            return false;

        EquipmentComponent equipmentComponent = actor.GetComponent<EquipmentComponent>(eComponent.EquipmentComponent);
        if (equipmentComponent == null)
            return false;

        equipmentComponent.PushItem(equipSlotType, item as EquipItem);
        DebugUtility.Log($"OnReplyEquipItem - CharacterID: {characterID} SlotType: {equipSlotType} ItemIndex: {item.Index}");
        return true;
    }
    bool OnReplyUnequipItem(int playerID)
    {
        ulong characterID = GetUInt64();
        eEquipSlotType equipSlotType = (eEquipSlotType)GetInt32();
        IActor actor = ActorManager.Instance.GetActor(characterID);
        if (actor == null)
            return false;

        EquipmentComponent equipmentComponent = actor.GetComponent<EquipmentComponent>(eComponent.EquipmentComponent);
        if (equipmentComponent == null)
            return false;

        equipmentComponent.PopSlot(equipSlotType);
        DebugUtility.Log($"OnReplyUnequipItem - CharacterID: {characterID} SlotType: {equipSlotType}");
        return true;
    }
}
