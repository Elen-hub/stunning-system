using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : TSingletonMono<InputManager>
{
    Dictionary<eInputType, KeyCode> _keycodeDictionary;
    public Dictionary<eInputType, KeyCode> GetKeyCodeDictionary => _keycodeDictionary;
    BaseInputHandler _mainController;
    FirstOrderInputHandler _firstOrderHandler;
    List<BaseInputHandler> _uiHandlerContainer;
    Stack<BaseInputHandler> _removeHandlerStack;
    public BaseInputHandler SetInputHandler { set => _mainController = value; }
    protected override void OnInitialize()
    {
        _keycodeDictionary = GetInitKeySetting();
        _firstOrderHandler = new FirstOrderInputHandler();
        _uiHandlerContainer = new List<BaseInputHandler>(3);
        _removeHandlerStack = new Stack<BaseInputHandler>();
    }
    public Dictionary<eInputType, KeyCode> GetInitKeySetting()
    {
        return new Dictionary<eInputType, KeyCode>()
        {
            { eInputType.MoveUp, KeyCode.W },
            { eInputType.MoveDown, KeyCode.S },
            { eInputType.MoveLeft, KeyCode.A },
            { eInputType.MoveRight, KeyCode.D },
            { eInputType.QuickSlot1, KeyCode.Alpha1 },
            { eInputType.QuickSlot2, KeyCode.Alpha2 },
            { eInputType.QuickSlot3, KeyCode.Alpha3 },
            { eInputType.QuickSlot4, KeyCode.Alpha4 },
            { eInputType.QuickSlot5, KeyCode.Alpha5 },
            { eInputType.QuickSlot6, KeyCode.Alpha6 },
            { eInputType.QuickSlot7, KeyCode.Alpha7 },
            { eInputType.QuickSlot8, KeyCode.Alpha8 },
            { eInputType.MouseLeft, KeyCode.Mouse0 },
            { eInputType.MouseRight, KeyCode.Mouse1 },
            { eInputType.Chatting, KeyCode.Return },
            { eInputType.Option, KeyCode.Escape },
            { eInputType.QuickSlotSwap, KeyCode.BackQuote },
            { eInputType.PickupItem, KeyCode.Space },
            { eInputType.Interact, KeyCode.E },
            { eInputType.OpenInventory, KeyCode.Tab },
            { eInputType.Sprint, KeyCode.LeftShift },
            { eInputType.Shift, KeyCode.LeftShift },
            { eInputType.Reload, KeyCode.R },
            { eInputType.Left, KeyCode.LeftArrow },
            { eInputType.Right, KeyCode.RightArrow },
        };
    }
    public void AddUIHandler(BaseInputHandler handler)
    {
        if (!_uiHandlerContainer.Contains(handler))
            _uiHandlerContainer.Add(handler);
    }
    public void RemoveUIHandler(BaseInputHandler key)
    {
        if (_uiHandlerContainer.Contains(key))
            _removeHandlerStack.Push(key);
    }
    void OnUpdateRemoveHandler()
    {
        while (_removeHandlerStack.Count > 0)
            _uiHandlerContainer.Remove(_removeHandlerStack.Pop());
    }
    private void Update() 
    {
        if (_firstOrderHandler != null)
        {
            _firstOrderHandler.Update();
            if (_firstOrderHandler.BlockInput)
                return;
        }
        for(int i = _uiHandlerContainer.Count - 1; i >= 0; --i)
            _uiHandlerContainer[i].Update();

        OnUpdateRemoveHandler();
        if (_mainController != null)
        {
            _mainController.Update();
            PlayerInput playerInput = _mainController.GetPlayerInput;
            NetworkManager.Instance.SetInput = playerInput;
        }
    }
}
