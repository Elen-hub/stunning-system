using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public enum eUIName
{
    USmartWatchUI,
    UQuickSlot,
    UStatusHUD,
    UInventory,
    UStorageUI,
    UCookUI,
    URefinementUI,
    UEquipmentUI,
    UBulletMagazineUI,
    ULoginUI,
    UChatting,
    UToolTipUI,
    USelectionItem,
    End,
}
public class UIManager : TSingletonMono<UIManager>
{
    PointerEventData _pointerEventData;
    EventSystem _eventSystem;
    List<RaycastResult> _raycastResult = new List<RaycastResult>();

    Dictionary<eUIName, BaseUI> _uiDictionary = new Dictionary<eUIName, BaseUI>();
    public Transform Root => _root;
    Transform _root;
    eUIName _lastOrderUI = 0;
    FieldUI _fieldUI;
    public FieldUI FieldUI => _fieldUI;
    protected override void OnInitialize()
    {
        Transform trs = Resources.Load<Transform>("UI/MainCanvas");
        _root = Instantiate(trs, transform);

        CanvasScaler scaler = _root.GetComponent<CanvasScaler>();
        scaler.scaleFactor = Screen.width / 1920f;
        _fieldUI = Instantiate(Resources.Load<FieldUI>("UI/FieldUI")/*, CameraManager.Instance.GetCamera(eCameraType.MainCamera).Camera.transform*/);
        _fieldUI.Initialize();

        _eventSystem = Instantiate(Resources.Load<EventSystem>("UI/EventSystem"), transform);
        _pointerEventData = new PointerEventData(_eventSystem);
        OnCacheInitialize();
    }
    public List<RaycastResult> GetRaycast
    {
        get {
            if(_raycastResult.Count == 0) {
                _pointerEventData.position = Input.mousePosition;
                EventSystem.current.RaycastAll(_pointerEventData, _raycastResult);
            }
            return _raycastResult;
        }
    }
    public bool IsOpen(eUIName name)
    {
        if (!_uiDictionary.ContainsKey(name))
            return false;

        return _uiDictionary[name].IsActive;
    }
    public BaseUI Open(eUIName name)
    {
        BaseUI prefab = Create<BaseUI>(name);
        prefab.Open();

        return prefab;
    }
    public T Open<T>(eUIName name) where T : BaseUI
    {
        T prefab = Create<T>(name);
        prefab.Open();

        return prefab;
    }
    public BaseUI Get(eUIName name)
    {
        if (!_uiDictionary.ContainsKey(name))
            return Create<BaseUI>(name);

        return _uiDictionary[name];
    }
    public T Get<T>(eUIName name) where T : BaseUI
    {
        if (!_uiDictionary.ContainsKey(name))
            return Create<T>(name);

        return _uiDictionary[name] as T;
    }
    T Create<T>(eUIName name) where T : BaseUI
    {
        if (!_uiDictionary.ContainsKey(name))
        {
            string path = "UI/" + name.ToString();
            T Prefab = Instantiate(Resources.Load<T>(path), _root);
            _uiDictionary.Add(name, Prefab);
            Prefab.Initialize();
            UpdateOrderInLayer(name);
        }
        return _uiDictionary[name] as T;
    }
    public void Close(eUIName name)
    {
        if (!_uiDictionary.ContainsKey(name))
            return;

        _uiDictionary[name].Close();
    }
    void UpdateOrderInLayer(eUIName uiType)
    {
        if (uiType > _lastOrderUI)
        {
            _lastOrderUI = uiType;
            return;
        }

        for (int i = (int)uiType + 1; i < (int)eUIName.End; ++i)
            if (_uiDictionary.ContainsKey((eUIName)i))
                (_uiDictionary[(eUIName)i].transform as RectTransform).SetAsLastSibling();

        _lastOrderUI = uiType;

        // _frontOrderTransform.SetAsLastSibling();
    }
    private void Update()
    {
        _raycastResult.Clear();
    }
    #region Cached element resource
    ObjectMemoryPool<ItemSlot> _itemSlotObjectMemoryPool;
    public ItemSlot GetItemSlot()
    {
        ItemSlot slot = _itemSlotObjectMemoryPool.GetItem();
        if (slot == null)
        {
            slot = _itemSlotObjectMemoryPool.InstantiateItem();
            slot.Initialize();
        }
        return slot;
    }
    public void Register(ItemSlot slot) => _itemSlotObjectMemoryPool.Register(slot);
    void OnCacheInitialize()
    {
        _itemSlotObjectMemoryPool = new ObjectMemoryPool<ItemSlot>(20, Resources.Load<ItemSlot>("UI/CacheElement/ItemSlot"));
    }
    #endregion
}
