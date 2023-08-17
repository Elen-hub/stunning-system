using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

public class RefinementObject : InteractObject
{
    [SerializeField] int _makeIndex;
    public int GetMakeIndex => _makeIndex;
    [SerializeField] bool _isRunning;
    public bool IsRunning => _isRunning;
    [SerializeField] float _targetTime = 0f;
    [SerializeField] float _elapsedTime = 0f;
    public float TargetTime => _targetTime;
    public float ElapsedTime => _elapsedTime;
    [SerializeField] ulong _inventoryID;
    public Inventory Inventory => ItemManager.Instance.GetInventory(_inventoryID);
    public Inventory inven;
    InventoryConstraint _materialSlotConstraint;
    InventoryConstraint _rewardSlotConstraint;
    #region Spawning
    public override void Initialize(int index)
    {
        base.Initialize(index);

        _materialSlotConstraint = new InventoryConstraint(true);
        for(int i = 0; i < DataManager.Instance.RecipeTable.GetDataDictionary(_index).Count; ++i)
            foreach(var element in DataManager.Instance.RecipeTable.GetDataDictionary(_index)[i].MaterialDictionary)
                _materialSlotConstraint.SetIndexConstraint(element.Key);

        _rewardSlotConstraint = new InventoryConstraint(true);
        for (int i = 0; i < DataManager.Instance.RecipeTable.GetDataDictionary(_index).Count; ++i)
            _rewardSlotConstraint.SetIndexConstraint(DataManager.Instance.RecipeTable.GetDataDictionary(_index)[i].Index);
    }
    public override void OnSpawnLoad(ulong worldID, SaveController saveController)
    {
        base.OnSpawnLoad(worldID, saveController);

        _inventoryID = saveController.GetUInt64();
        Inventory.SetSlotConstraint(0, _materialSlotConstraint);
        Inventory.SetSlotConstraint(1, _rewardSlotConstraint);

#if UNITY_SERVER
        Inventory.OnSlotChangedCallback += OnCraftStateChagned;
#endif
        inven = Inventory;
    }
    public override void OnSpawnServer(ulong worldID)
    {
        base.OnSpawnServer(worldID);

        _inventoryID = ItemManager.Instance.InstanceInventory(2).InventoryID;
        Inventory.SetSlotConstraint(0, _materialSlotConstraint);
        Inventory.SetSlotConstraint(1, _rewardSlotConstraint);
#if UNITY_SERVER
        Inventory.OnSlotChangedCallback += OnCraftStateChagned;
#endif


        inven = Inventory;
    }
    public override void OnSpawnClient(BaseEventReceiver eventReceiver)
    {
        base.OnSpawnClient(eventReceiver);

        _inventoryID = eventReceiver.GetUInt64();
        Inventory.SetSlotConstraint(0, _materialSlotConstraint);
        Inventory.SetSlotConstraint(1, _rewardSlotConstraint);
#if UNITY_SERVER
        Inventory.OnSlotChangedCallback += OnCraftStateChagned;
#endif
        inven = Inventory;
    }
#endregion
#region Interact
    protected override void OnClientInteractStart(IActor caster)
    {
        base.OnClientInteractStart(caster);

        UIManager.Instance.Get<URefinementUI>(eUIName.URefinementUI).SetRefinementObject(this);
        UIManager.Instance.Open(eUIName.URefinementUI);
    }
    protected override void OnClientInteractEnd()
    {
        base.OnClientInteractEnd();

        UIManager.Instance.Close(eUIName.URefinementUI);
        UIManager.Instance.Close(eUIName.UInventory);
    }
#endregion
    void OnCraftStateChagned(int slotNumber)
    {
        if (_isRunning)
            return;

        if(slotNumber == 0)
        {
            _makeIndex = 0;

            Item item = Inventory.GetItem(1);
            if (item != null)
            {
                for (int i = 0; i < DataManager.Instance.RecipeTable.GetDataDictionary(_index).Count; ++i)
                {
                    Data.RecipeData data = DataManager.Instance.RecipeTable.GetDataDictionary(_index)[i];
                    if (data.Index == item.Index)
                    {
                        foreach (var element in data.MaterialDictionary)
                        {
                            if (!Inventory.IsContainsItem(element.Key, element.Value))
                            {
                                _makeIndex = 0;
                                break;
                            }
                        }
                        _makeIndex = data.Index;
                        SelectCraftIndex(_makeIndex);
                        return;
                    }
                }
            }

            Item material = Inventory.GetItem(slotNumber);
            if (material != null)
            {
                foreach (var element in DataManager.Instance.RecipeTable.GetDataDictionary(_index))
                {
                    if (element.MaterialDictionary.ContainsKey(material.Index))
                    {
                        if (element.MaterialDictionary[material.Index] <= material.Count)
                        {
                            _makeIndex = element.Index;
                            break;
                        }
                    }
                }
            }

            SelectCraftIndex(_makeIndex);
        }
    }
    void SelectCraftIndex(int index)
    {
        if (index == 0)
            _targetTime = 0f;
        else
        {
            _targetTime = DataManager.Instance.RecipeTable[_makeIndex].CraftTime;
#if UNITY_SERVER
            if (!_isRunning)
                StartCraft();
#endif
        }
    }
    public void StartCraft(int makeIndex)
    {
        if (makeIndex == 0)
            return;

        _makeIndex = makeIndex;
        SelectCraftIndex(makeIndex);
        _elapsedTime = 0f;
        _isRunning = true;
#if !UNITY_SERVER
        SendComponentMessage(eComponentEvent.SetObjectRender, eObjectRenderState.Action);
#endif
    }
    public void StopCraft()
    {
        _elapsedTime = 0f;
        _isRunning = false;
        OnCraftStateChagned(0);
#if UNITY_SERVER
        NetworkManager.Instance.ObjectEventSender.NotifyStopRefinement(WorldID);
#else
        SendComponentMessage(eComponentEvent.SetObjectRender, eObjectRenderState.Action);
#endif
    }
    public void StartCraft()
    {
        if (_makeIndex == 0)
            return;

        _elapsedTime = 0f;
        _isRunning = true;
#if UNITY_SERVER
        NetworkManager.Instance.ObjectEventSender.NotifyStartRefinement(WorldID, _makeIndex);
        foreach (var element in DataManager.Instance.RecipeTable[_makeIndex].MaterialDictionary)
        {
            Inventory.PopItem(element.Key, element.Value);
            NetworkManager.Instance.ItemEventSender.NotifyPopItem(_inventoryID, element.Key, element.Value);
        }
#endif
    }
    public bool IsPossibleCraft()
    {
        if (_makeIndex == 0)
            return false;

        Item item = Inventory.GetItem(1);
        if(item != null)
        {
            for(int i = 0; i < DataManager.Instance.RecipeTable.GetDataDictionary(_index).Count; ++i)
            {
                Data.RecipeData data = DataManager.Instance.RecipeTable.GetDataDictionary(_index)[i];
                if (data.Index == item.Index)
                {
                    foreach(var element in data.MaterialDictionary)
                    {
                        if (!Inventory.IsContainsItem(element.Key, element.Value))
                            return false;
                    }
                    return true;
                }
            }
        }

        foreach (var element in DataManager.Instance.RecipeTable[_makeIndex].MaterialDictionary)
        {
            if (!Inventory.IsContainsItem(element.Key, element.Value))
                return false;
        }
        return true;
    }
    public void CompleteCraft()
    {
        Item item = Item.NewItem(_makeIndex, DataManager.Instance.RecipeTable[_makeIndex].Count);
        if (Inventory.PushItem(item))
            NetworkManager.Instance.ItemEventSender.NotifyPushItemSlot(_inventoryID, 1, item);
        else
            NetworkManager.Instance.ItemEventSender.RequestSpawnItem(item, transform.position);

        _isRunning = false;
        if (IsPossibleCraft())
            StartCraft();
        else
            StopCraft();
    }
    protected override void Update()
    {
        base.Update();

        if (_isRunning)
        {
            _elapsedTime += TimeManager.DeltaTime;
#if UNITY_SERVER
            if (_elapsedTime >= _targetTime)
            {
                _isRunning = false;
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
