using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : TSingletonMono<ItemManager>
{
#if UNITY_SERVER
    static ulong _currentInventoryID = 100;
#else
    static ulong _currentInventoryID = 0;
#endif
    Dictionary<ulong, Inventory> _inventoryDictionary;
    public Dictionary<ulong, Inventory> GetInventoryDictionary => _inventoryDictionary;
    public Inventory GetInventory(ulong inventoryID) => _inventoryDictionary[inventoryID];
    ItemFactory _itemFactory;
    public Dictionary<ulong, ItemObject> GetItemDictionary => _itemFactory.ItemDictionary;
    public Item SelectionItem;
    protected override void OnInitialize()
    {
        _itemFactory = new ItemFactory(transform);
        _inventoryDictionary = new Dictionary<ulong, Inventory>();
    }
    public void AddInventory(Inventory inventory) => _inventoryDictionary.Add(inventory.InventoryID, inventory);
    public Inventory InstanceInventory(ulong inventoyID, int capacity)
    {
        Inventory inventory = new Inventory(inventoyID, capacity);
        _inventoryDictionary.Add(inventoyID, inventory);
        return inventory;
    }
    public Inventory InstanceInventory(int capacity) 
    {
        Inventory inventory = InstanceInventory(++_currentInventoryID, capacity);
        if (NetworkManager.Instance.Runner.IsServer)
            NetworkManager.Instance.ItemEventSender.NotifyInstanceInventory(inventory);

        return inventory;
    }
    public ItemObject GetItem(ulong worldID) => _itemFactory.GetItem(worldID);
    public ItemObject SpawnItem(Item item, Vector2 position) => _itemFactory.SpawnItem(item, position);
    public ItemObject SpawnItem(Item item, Vector2 position, ulong worldID) => _itemFactory.SpawnItem(item, position, worldID);
    public void RegistWorld(ItemObject itemObject) => _itemFactory.RegistWorld(itemObject);
    public void RemoveWorld(ItemObject itemObject) => _itemFactory.RemoveWorld(itemObject);
    public void RegistObjectMemoryPool(ItemObject itemObject) => _itemFactory.RegistObjectMemoryPool(itemObject);

#if UNITY_SERVER
    private void OnGUI()
    {
        if (GUI.Button(new Rect(200, 0, 100, 100), "SpawnItem"))
        {
            Debug.LogWarning("Inventory Debuging Start");
            foreach (var inventory in _inventoryDictionary)
            {
                Debug.LogWarning($"Inventory Key: {inventory.Key}");
                for(int i = 0; i < inventory.Value.Capacity; ++i)
                    if(inventory.Value.GetItem(i) != null)
                        Debug.LogWarning($"ItemIndex: {inventory.Value.GetItem(i).Index}  Count: {inventory.Value.GetItem(i).Count}");
            }
            Debug.LogWarning("Inventory Debuging End");
        }
    }
#endif
}
