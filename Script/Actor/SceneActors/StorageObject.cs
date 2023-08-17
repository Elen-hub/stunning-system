using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

public class StorageObject : InteractObject
{
    public int StorageCapacity;
    [SerializeField] ulong _inventoryID;
    Inventory _inventory => ItemManager.Instance.GetInventory(_inventoryID);
    public override void OnSpawnLoad(ulong worldID, SaveController saveController)
    {
        base.OnSpawnLoad(worldID, saveController);

        _inventoryID = saveController.GetUInt64();
    }
    public override void OnSpawnClient(BaseEventReceiver eventReceiver)
    {
        base.OnSpawnClient(eventReceiver);

        _inventoryID = eventReceiver.GetUInt64();
    }
    public override void OnSpawnServer(ulong worldID)
    {
        base.OnSpawnServer(worldID);

        _inventoryID = ItemManager.Instance.InstanceInventory(StorageCapacity).InventoryID;
    }
    protected override void OnClientInteractStart(IActor caster)
    {
        base.OnClientInteractStart(caster);

        UIManager.Instance.Get<UStorageUI>(eUIName.UStorageUI).SetInventory(_inventory, caster.GetComponent<ItemComponent>(eComponent.ItemComponent));
        UIManager.Instance.Open(eUIName.UStorageUI);
    }
    protected override void OnClientInteractEnd()
    {
        base.OnClientInteractEnd();

        UIManager.Instance.Close(eUIName.UStorageUI);
        UIManager.Instance.Close(eUIName.UInventory);
    }
    #region Network
    public override int GetByteSize => base.GetByteSize + ReliableHelper.ULongSize;
    public override void EnqueueByte()
    {
        base.EnqueueByte();

        BaseEventSender.CopyBytes(_inventoryID);
    }
    #endregion
    #region Save & Load
    protected override void ProcessSaveByte()
    {
        base.ProcessSaveByte();

        _saveController.CopyBytes(_inventoryID);
    }
    #endregion
}
