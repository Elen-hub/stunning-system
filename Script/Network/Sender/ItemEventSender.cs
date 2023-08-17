using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public enum eItemEventCode
{
    NotifyInstanceInventory,
    RequestSpawnItem,
    NotifySpawnItem,
    RequestPickupCheck,
    NotifyPickupCheck,
    RequestPickupItem,
    NotifyPickupItem,
    RequestPushItemSlot,
    RequestPushItem,
    RequestPopItemSlot,
    NotifyPushItemSlot,
    NotifyPushItem,
    NotifyPopItemSlot,
    NotifyPopItem,
    RequestEquipItem,
    RequestUnequipItem,
}
public class ItemEventSender : BaseEventSender
{
    public void NotifyInstanceInventory(Inventory inventory)
    {
        int capacity = inventory.GetByteSize;
        GeneratePacketOption(eItemEventCode.NotifyInstanceInventory, capacity, ReceiverOption.Other);
        inventory.EnqueueByte();
    }
    public void RequestSpawnItem(Item item, Vector2 position)
    {
        int capacity = item.GetByteSize + Vector2Size;
        GeneratePacketOption(eItemEventCode.RequestSpawnItem, capacity, ReceiverOption.Host);
        item.EnqueueByte();
        CopyBytes(position);
    }
    public void NotifySpawnItem(ItemObject itemObject)
    {
        int capacity = itemObject.GetByteSize;
        GeneratePacketOption(eItemEventCode.NotifySpawnItem, capacity, ReceiverOption.Other);
        itemObject.EnqueueByte();
    }
    public void RequestPickupCheck(ulong itemID)
    {
        int capacity =  ULongSize;
        GeneratePacketOption(eItemEventCode.RequestPickupCheck, capacity, ReceiverOption.Host);
        CopyBytes(itemID);
    }
    public void NotifyPickupCheck(int playerID, ulong itemID)
    {
        int capacity = ULongSize;
        GeneratePacketOption(eItemEventCode.NotifyPickupCheck, capacity, playerID);
        CopyBytes(itemID);
    }
    public void RequestPickupItem(ulong characterID, ulong itemID, bool isRemove)
    {
        int capacity = ULongSize + ULongSize + BooleanSize;
        GeneratePacketOption(eItemEventCode.RequestPickupItem, capacity, ReceiverOption.Host);
        CopyBytes(characterID);
        CopyBytes(itemID);
        CopyBytes(isRemove);
    }
    public void NotifyPickupItem(ulong characterID, ulong itemID, int exceptID)
    {
        List<int> activePlayers = new List<int>(4);
        foreach (var player in Runner.ActivePlayers)
        {
            if (player.PlayerId != exceptID)
                activePlayers.Add(player);
        }

        int capacity = ULongSize + ULongSize;
        GeneratePacketOption(eItemEventCode.NotifyPickupItem, capacity, activePlayers);
        CopyBytes(characterID);
        CopyBytes(itemID);
    }
    public void RequestPushItemSlot(ulong inventoryID, int arrayNumber, Item item)
    {
        int capacity = ULongSize + IntSize + item.GetByteSize;
        GeneratePacketOption(eItemEventCode.RequestPushItemSlot, capacity, ReceiverOption.Host);
        CopyBytes(inventoryID);
        CopyBytes(arrayNumber);
        item.EnqueueByte();
    }
    public void RequestPushItem(ulong inventoryID, Item item)
    {
        int capacity = ULongSize + item.GetByteSize;
        GeneratePacketOption(eItemEventCode.RequestPushItem, capacity, ReceiverOption.Host);
        CopyBytes(inventoryID);
        item.EnqueueByte();
    }
    public void RequestPopItem(ulong inventoryID, int arrayNumber)
    {
        int capacity = ULongSize + IntSize;
        GeneratePacketOption(eItemEventCode.RequestPopItemSlot, capacity, ReceiverOption.Host);
        CopyBytes(inventoryID);
        CopyBytes(arrayNumber);
    }
    public void RequestPopItemSlot(ulong inventoryID, int arrayNumber, int count)
    {
        int capacity = ULongSize + IntSize + IntSize;
        GeneratePacketOption(eItemEventCode.RequestPopItemSlot, capacity, ReceiverOption.Host);
        CopyBytes(inventoryID);
        CopyBytes(arrayNumber);
        CopyBytes(count);
    }
    public void NotifyPushItemSlot(ulong inventoryID, int arrayNumber, Item item, int exceptID)
    {
        HashSet<int> activePlayers = new HashSet<int>();
        foreach(var player in Runner.ActivePlayers) {
            if (player.PlayerId != exceptID)
                activePlayers.Add(player);
        }
        int capacity = ULongSize + IntSize + item.GetByteSize;
        GeneratePacketOption(eItemEventCode.NotifyPushItemSlot, capacity, activePlayers);
        CopyBytes(inventoryID);
        CopyBytes(arrayNumber);
        item.EnqueueByte();
    }
    public void NotifyPushItemSlot(ulong inventoryID, int arrayNumber, Item item)
    {
        int capacity = ULongSize + IntSize + item.GetByteSize;
        GeneratePacketOption(eItemEventCode.NotifyPushItemSlot, capacity, ReceiverOption.Other);
        CopyBytes(inventoryID);
        CopyBytes(arrayNumber);
        item.EnqueueByte();
    }
    public void NotifyPushItem(ulong inventoryID, Item item)
    {
        int capacity = ULongSize + item.GetByteSize;
        GeneratePacketOption(eItemEventCode.NotifyPushItem, capacity, ReceiverOption.Other);
        CopyBytes(inventoryID);
        item.EnqueueByte();
    }
    public void NotifyPushItem(ulong inventoryID, Item item, int exceptID)
    {
        HashSet<int> activePlayers = new HashSet<int>();
        foreach (var player in Runner.ActivePlayers)
        {
            if (player.PlayerId != exceptID)
                activePlayers.Add(player);
        }

        int capacity = ULongSize + item.GetByteSize;
        GeneratePacketOption(eItemEventCode.NotifyPushItem, capacity, activePlayers);
        CopyBytes(inventoryID);
        item.EnqueueByte();
    }
    public void NotifyPopItemSlot(ulong inventoryID, int arrayNumber, int count, int exceptID)
    {
        HashSet<int> activePlayers = new HashSet<int>();
        foreach (var player in Runner.ActivePlayers) {
            if (player.PlayerId != exceptID)
                activePlayers.Add(player);
        }
        int capacity = ULongSize + IntSize + IntSize;
        GeneratePacketOption(eItemEventCode.NotifyPopItemSlot, capacity, activePlayers);
        CopyBytes(inventoryID);
        CopyBytes(arrayNumber);
        CopyBytes(count);
    }
    public void NotifyPopItemSlot(ulong inventoryID, int arrayNumber, int count)
    {
        int capacity = ULongSize + IntSize + IntSize;
        GeneratePacketOption(eItemEventCode.NotifyPopItemSlot, capacity, ReceiverOption.Other);
        CopyBytes(inventoryID);
        CopyBytes(arrayNumber);
        CopyBytes(count);
    }
    public void NotifyPopItem(ulong inventoryID, int index, int count)
    {
        int capacity = ULongSize + IntSize + IntSize;
        GeneratePacketOption(eItemEventCode.NotifyPopItem, capacity, ReceiverOption.Other);
        CopyBytes(inventoryID);
        CopyBytes(index);
        CopyBytes(count);
    }
    public void RequestEquipItem(ulong characterID, eEquipSlotType itemSlotType, Item item)
    {
        int capacity = ULongSize + IntSize + item.GetByteSize;
        GeneratePacketOption(eItemEventCode.RequestEquipItem, capacity, ReceiverOption.Host);
        CopyBytes(characterID);
        CopyBytes((int)itemSlotType);
        item.EnqueueByte();
    }
    public void RequestUnequipItem(ulong characterID, eEquipSlotType itemSlotType)
    {
        int capacity = ULongSize + IntSize;
        GeneratePacketOption(eItemEventCode.RequestUnequipItem, capacity, ReceiverOption.Host);
        CopyBytes(characterID);
        CopyBytes((int)itemSlotType);
    }
}
