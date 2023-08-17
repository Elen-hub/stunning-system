using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class ItemFactory
{
    ulong _currentWorldID;
    NetworkRunner _runner => NetworkManager.Instance.Runner;
    ObjectMemoryPool<ItemObject> _itemObjectObjectMemoryPool;
    Dictionary<ulong, ItemObject> _itemDic = new Dictionary<ulong, ItemObject>();
    public Dictionary<ulong, ItemObject> ItemDictionary => _itemDic;
    Transform _root;
    public ItemFactory(Transform root)
    {
        _currentWorldID = 0;
        _root = root;
        _itemObjectObjectMemoryPool = new ObjectMemoryPool<ItemObject>(100, Resources.Load<ItemObject>("Actor/ItemObject"));
    }
    public ItemObject GetItem(ulong worldID)
    {
        if (_itemDic.ContainsKey(worldID))
            return _itemDic[worldID];

        return null;
    }
    ItemObject InstantiateItem()
    {
        ItemObject itemObject = _itemObjectObjectMemoryPool.GetItem();
        if (itemObject == null)
        {
            itemObject = _itemObjectObjectMemoryPool.GetItem();
            if (itemObject == null)
            {
                itemObject = _itemObjectObjectMemoryPool.InstantiateItem();
                itemObject.Initialize();
            }
        }
        return itemObject;
    }
    public Data.ItemData ItemData(int index) => DataManager.Instance.ItemTable[index];
    public Data.ObjectData EquipItemData(int index) => DataManager.Instance.ObjectTable[index];
    public ItemObject SpawnItem(Item item, Vector2 position)
    {
        ItemObject itemObject = InstantiateItem();
        itemObject.Spawn(++_currentWorldID, item, position);
        _itemDic.Add(_currentWorldID, itemObject);
        return itemObject;
    }
    public ItemObject SpawnItem(Item item, Vector2 position, ulong worldID)
    {
        ItemObject itemObject = InstantiateItem();
        itemObject.Spawn(worldID, item, position);
        _itemDic.Add(worldID, itemObject);
        return itemObject;
    }
    public void RegistWorld(ItemObject itemObject) => _itemDic.Add(itemObject.WorldID, itemObject);
    public void RemoveWorld(ItemObject itemObject) => _itemDic.Remove(itemObject.WorldID);
    public void RegistObjectMemoryPool(ItemObject itemObject) => _itemObjectObjectMemoryPool.Register(itemObject);
}
