using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

public class CookObject : InteractObject
{
    [SerializeField] int _makeIndex;
    public int GetMakeIndex => _makeIndex;

    [SerializeField] bool _isCrafting;
    public bool IsCrafting => _isCrafting;
    [SerializeField] float _targetTime = 0f;
    [SerializeField] float _elapsedTime = 0f;
    public float TargetTime => _targetTime;
    public float ElapsedTime => _elapsedTime;

    public event System.Action<int> OnChangedCraftItemEvent;
    public event System.Action<bool> OnChangedCraftState;
    const int _storageCapacity = 15;
    [SerializeField] ulong _inventoryID;
    public Inventory Inventory => ItemManager.Instance.GetInventory(_inventoryID);

    public Inventory inven;
    #region Spawning
    public override void Initialize(int index)
    {
        base.Initialize(index);
    }
    public override void OnSpawnLoad(ulong worldID, SaveController saveController)
    {
        base.OnSpawnLoad(worldID, saveController);

        _inventoryID = saveController.GetUInt64();
        Inventory.OnSlotChangedCallback += OnCraftStateChagned;

        inven = Inventory;
    }
    public override void OnSpawnServer(ulong worldID)
    {
        base.OnSpawnServer(worldID);

        _inventoryID = ItemManager.Instance.InstanceInventory(_storageCapacity).InventoryID;
        Inventory.OnSlotChangedCallback += OnCraftStateChagned;

        inven = Inventory;
    }
    public override void OnSpawnClient(BaseEventReceiver eventReceiver)
    {
        base.OnSpawnClient(eventReceiver);

        _inventoryID = eventReceiver.GetUInt64();
        Inventory.OnSlotChangedCallback += OnCraftStateChagned;

        inven = Inventory;
    }
    #endregion
    #region Interact
    protected override void OnClientInteractStart(IActor caster)
    {
        base.OnClientInteractStart(caster);

        UIManager.Instance.Get<UCookUI>(eUIName.UCookUI).SetCookObject(this);
        UIManager.Instance.Open(eUIName.UCookUI);
        OnChangedCraftItemEvent?.Invoke(_makeIndex);
        OnChangedCraftState?.Invoke(_isCrafting);
    }
    protected override void OnClientInteractEnd()
    {
        base.OnClientInteractEnd();

        UIManager.Instance.Close(eUIName.UCookUI);
        UIManager.Instance.Close(eUIName.UInventory);
    }
    #endregion
    #region Craft
    public bool IsPossibleCraft(bool isStart)
    {
        if (isStart)
        {
            if (_makeIndex == 0)
                return false;

            foreach(var element in DataManager.Instance.RecipeTable[_makeIndex].MaterialDictionary)
            {
                if (!Inventory.IsContainsItem(element.Key, element.Value))
                    return false;
            }
            return !IsCrafting;
        }
        else
        {
            if (_makeIndex == 0)
                return false;

            return IsCrafting;
        }
    }
    void OnCraftStateChagned(int slotNumber)
    {
        if (_isCrafting)
            return;

        _makeIndex = 0;
        HashSet<int> searchCullingHash = new HashSet<int>();
        for (int i = 0; i < Inventory.Capacity; ++i)
        {
            Item item = Inventory.GetItem(i);
            if(item != null)
            {
                List<int> indexList = GetCraftPossibleList(item);
                if (indexList == null)
                    continue;

                ProcessCraftList(i, indexList, searchCullingHash);
                if(_makeIndex != 0)
                {
                    SelectCraftIndex(_makeIndex);
                    return;
                }
            }
        }
        SelectCraftIndex(_makeIndex);
    }
    List<int> GetCraftPossibleList(Item item)
    {
        if (!DataManager.Instance.RecipeTable.IsContainsMaterialNeedable(item.Index))
            return null;

        List<int> indexList = new List<int>();
        foreach (var element in DataManager.Instance.RecipeTable.GetDataDictionary(Index))
        {
            if (element.MaterialDictionary.ContainsKey(item.Index))
            {
                if (item.Count >= element.MaterialDictionary[item.Index])
                {
                    indexList.Add(element.Index);
                }
            }
        }
        return indexList;
    }
    void ProcessCraftList(int inventoryNumber, List<int> indexList, HashSet<int> searchCullingHash)
    {
        int price = 0;
        for(int i = 0; i< indexList.Count; ++i)
        {
            Data.RecipeData Recipe = DataManager.Instance.RecipeTable[indexList[i]];
            if (searchCullingHash.Contains(Recipe.Index))
                continue;

            if(Recipe.MaterialDictionary.Count == 1)
            {
                int finishPrice = DataManager.Instance.ItemTable[indexList[i]].Price;
                if (finishPrice > price)
                {
                    price = finishPrice;
                    _makeIndex = indexList[i];
                }
                continue;
            }

            foreach (var materials in Recipe.MaterialDictionary)
            {
                bool isContains = false;
                for(int j = 0; j < Inventory.Capacity; ++j)
                {
                    if (j == inventoryNumber)
                        continue;

                    Item item = Inventory.GetItem(j);
                    if(item != null)
                    {
                        if(materials.Key == item.Index)
                        {
                            if (materials.Value <= item.Count)
                            {
                                int finishPrice = DataManager.Instance.ItemTable[indexList[i]].Price;
                                if(finishPrice > price)
                                {
                                    price = finishPrice;
                                    _makeIndex = indexList[i];
                                }
                                isContains = true;
                                break;
                            }
                        }
                    }
                }
                if (!isContains)
                    continue;
            }
        }
    }
    void SelectCraftIndex(int index)
    {
        OnChangedCraftItemEvent?.Invoke(_makeIndex);

        if (index == 0) _targetTime = 0f;
        else _targetTime = DataManager.Instance.RecipeTable[_makeIndex].CraftTime;
    }
    public void StartCraft()
    {
        if (_makeIndex == 0)
            return;

        Debug.LogWarning("Start");
        _elapsedTime = 0f;
        _isCrafting = true;
#if UNITY_SERVER
        foreach (var element in DataManager.Instance.RecipeTable[_makeIndex].MaterialDictionary)
        {
            Inventory.PopItem(element.Key, element.Value);
            NetworkManager.Instance.ItemEventSender.NotifyPopItem(_inventoryID, element.Key, element.Value);
        }
#endif
    }
    public void StartCraft(int makeIndex)
    {
        if (makeIndex == 0)
            return;

        _makeIndex = makeIndex;
        SelectCraftIndex(makeIndex);
        _elapsedTime = 0f;
        _isCrafting = true;
        OnChangedCraftState?.Invoke(_isCrafting);
    }
    public void StopCraft()
    {
        _elapsedTime = 0f;
        _isCrafting = false;
        OnChangedCraftState?.Invoke(_isCrafting);
        OnCraftStateChagned(0);
    }
    public void CompleteCraft()
    {
        _elapsedTime = 0f;
        int makeIndex = _makeIndex;
        Item item = Item.NewItem(makeIndex, 1);
        if (Inventory.PushItem(item))
            NetworkManager.Instance.ItemEventSender.NotifyPushItem(_inventoryID, Item.NewItem(makeIndex, 1));
        else
            NetworkManager.Instance.ItemEventSender.RequestSpawnItem(item, transform.position);

        if (IsPossibleCraft(true))
        {
            StartCraft();
            NetworkManager.Instance.ObjectEventSender.NotifyStartCook(WorldID, _makeIndex, true);
        }
        else
        {
            OnCraftStateChagned(0);
            if (IsPossibleCraft(true))
            {
                StartCraft(_makeIndex);
                NetworkManager.Instance.ObjectEventSender.NotifyStartCook(WorldID, _makeIndex, true);
            }
            else
            {
                StopCraft();
                NetworkManager.Instance.ObjectEventSender.NotifyStopCook(WorldID, true);
            }
        }
    }
#endregion
    protected override void Update()
    {
        base.Update();

        if (_isCrafting) 
        {
            _elapsedTime += TimeManager.DeltaTime;
#if UNITY_SERVER
            if (_elapsedTime >= _targetTime)
            {
                _isCrafting = false;
                CompleteCraft();
            }
#endif
        }
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
