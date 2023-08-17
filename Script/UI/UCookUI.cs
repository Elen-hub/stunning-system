using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UCookUI : BaseUI
{
    [SerializeField] int _makeIndex;

    float _elapsedTime = 0f;
    Vector2 _openPosition;
    Vector2 _closePosition;

    ItemSlot[] _itemSlotList;
    CookObject _cookObject;

    Transform _slotLayout;

    Image _emptyImg;
    Image _craftResultImg;
    GameObject _slotMask;
    TextMeshProUGUI _slotMaskText;
    TextMeshProUGUI _progressingText;

    TextMeshProUGUI _titleText;

    GameObject _craftButton;
    TextMeshProUGUI _craftingText;
    RectScaleProgress _craftingProgress;

    int _currentSeconds;
    public void SetCookObject(CookObject cookObject)
    {
        if (_cookObject != null)
        {
            _cookObject.Inventory.OnSlotChangedCallback -= OnSlotChange;
            _cookObject.OnChangedCraftItemEvent -= OnChangedCraftItem;
            _cookObject.OnChangedCraftState -= OnUpdateCraftButton;
        }
        _cookObject = cookObject;
        if (_cookObject != null)
        {
            _cookObject.Inventory.OnSlotChangedCallback += OnSlotChange;
            _cookObject.OnChangedCraftItemEvent += OnChangedCraftItem;
            _cookObject.OnChangedCraftState += OnUpdateCraftButton;
            UIManager.Instance.Get<UInventory>(eUIName.UInventory).SetSubInventory = _cookObject.Inventory.InventoryID;
            UIManager.Instance.Open(eUIName.UInventory);
        }
    }
    float GetCurrentAmount() => _cookObject != null ? _cookObject.ElapsedTime : 0f;
    float GetMaxAmount() => _cookObject != null ? _cookObject.TargetTime : 1f;
    void OnSlotChange(int number) => _itemSlotList[number].Redraw();
    protected override void InitReference()
    {
        _closePosition = new Vector2(0f, rectTransform.sizeDelta.y);
        _openPosition = new Vector2(0f, -65f);

        _slotLayout = transform.Find("SlotLayout");
        _slotMask = transform.Find("Img_SlotMask").gameObject;
        _craftingProgress = transform.Find("Img_SlotMask/CraftingProgress").GetComponent<RectScaleProgress>();
        _craftingProgress.Initialize();
        _craftingProgress.GetCurrentAmount = GetCurrentAmount;
        _craftingProgress.GetMaxAmount = GetMaxAmount;
        _slotMaskText = transform.Find("Img_SlotMask/Text_Mask").GetComponent<TextMeshProUGUI>();
        _progressingText = transform.Find("Img_SlotMask/CraftingProgress/Text_Progress").GetComponent<TextMeshProUGUI>();
        _itemSlotList = _slotLayout.GetComponentsInChildren<ItemSlot>(true);
        _emptyImg = transform.Find("ResultLayout/RecipeLayer/Img_Empty").GetComponent<Image>();
        _craftResultImg = transform.Find("ResultLayout/RecipeLayer/Img_CraftResult").GetComponent<Image>();
        _titleText = transform.Find("ResultLayout/NameTag/Text_Title").GetComponent<TextMeshProUGUI>();
        _craftButton = transform.Find("ResultLayout/Btn_Crafting").gameObject;
        _craftingText = transform.Find("ResultLayout/Btn_Crafting/Text_Crafting").GetComponent<TextMeshProUGUI>();
        _craftButton.GetComponent<Button>().onClick.AddListener(OnClickCrafting);
        for (int i = 0; i < _itemSlotList.Length; ++i)
            _itemSlotList[i].Initialize();
    }
    protected override void OnOpen()
    {
        UIManager.Instance.Open(eUIName.UInventory);

        DrawAllSlot();
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        SetCookObject(null);
    }
    public override void Refresh()
    {
        _titleText.text = LocalizingManager.Instance.GetLocalizing(10001);
        _slotMaskText.text = LocalizingManager.Instance.GetLocalizing(28);
    }
    void DrawAllSlot()
    {
        if (_cookObject == null)
        {
            for (int i = 0; i < _itemSlotList.Length; ++i)
                _itemSlotList[i].Disable();
        }
        else
        {
            for(int i = 0; i< _cookObject.Inventory.Capacity; ++i)
                _itemSlotList[i].SetReferenceRoot(_cookObject.Inventory.InventoryID, i);
        }
    }
    void OnUpdateActivateAnimation()
    {
        if (IsActive)
        {
            if (_elapsedTime < 1f)
            {
                const float speed = 5f;
                _elapsedTime += TimeManager.DeltaTime * speed;
                Vector2 anchoredPosition = Vector2.Lerp(_closePosition, _openPosition, _elapsedTime);
                rectTransform.anchoredPosition = anchoredPosition;
            }
        }
        else
        {
            if (_elapsedTime > 0f)
            {
                const float speed = 4f;
                _elapsedTime -= TimeManager.DeltaTime * speed;
                Vector2 anchoredPosition = Vector2.Lerp(_closePosition, _openPosition, _elapsedTime);
                rectTransform.anchoredPosition = anchoredPosition;
            }
            else
                gameObject.SetActive(false);
        }
    }
    void OnClickCrafting()
    {
        if (_cookObject == null)
            return;

        if (_cookObject.IsCrafting)
        {
            if(_cookObject.IsPossibleCraft(false))
                NetworkManager.Instance.ObjectEventSender.RequestStopCook(_cookObject.WorldID);
        }
        else
        {
            if (_cookObject.IsPossibleCraft(true))
                NetworkManager.Instance.ObjectEventSender.RequestStartCook(_cookObject.WorldID);
        }
    }
    void OnChangedCraftItem(int index)
    {
        if(_makeIndex != index)
        {
            _makeIndex = index;
            if(_makeIndex == 0)
            {
                _emptyImg.enabled = true;
                _craftResultImg.enabled = false;
                _craftButton.SetActive(false);
            }
            else
            {
                _emptyImg.enabled = false;
                _craftResultImg.sprite = DataManager.Instance.ItemTable[index].Icon;
                _craftResultImg.enabled = true;
                _craftButton.SetActive(true);
            }
        }
    }
    void OnUpdateCraftButton(bool isCraft)
    {
        if (isCraft)
        {
            _craftingText.text = LocalizingManager.Instance.GetLocalizing(29);
            _slotMask.SetActive(true);
        }
        else
        {
            _craftingText.text = LocalizingManager.Instance.GetLocalizing(27);
            _slotMask.SetActive(false);
        }
    }
    void OnUpdateProgressText()
    {
        if (_cookObject == null)
            return;

        if (!_cookObject.IsCrafting)
            return;

        int sec = Mathf.FloorToInt(GetMaxAmount() - GetCurrentAmount());
        if(_currentSeconds != sec)
        {
            _currentSeconds = sec;
            _progressingText.text = LocalizingManager.Instance.GetLocalizing(30, _currentSeconds);
        }
    }
    private void LateUpdate()
    {
        OnUpdateActivateAnimation();
        OnUpdateProgressText();
    }
}
