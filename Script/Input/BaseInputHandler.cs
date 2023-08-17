using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseInputHandler
{
    protected Dictionary<eInputType, KeyCode> _keycodeDictionary => InputManager.Instance.GetKeyCodeDictionary;
    static protected HashSet<eInputType> _currentInputHash = new HashSet<eInputType>((int)eInputType.End);
    static protected PlayerInput _networkInput;
    public PlayerInput GetPlayerInput => _networkInput;
    protected uint SetButton { set => _networkInput.ButtonDown |= value; }
    protected uint RemoveButton { set => _networkInput.ButtonDown &= ~value; }
    protected bool IsKeyInput(eInputType type) => Input.GetKey(_keycodeDictionary[type]);
    protected bool IsKeyDown(eInputType type)
    {
        if (_currentInputHash.Contains(type))
            return false;

        if (Input.GetKeyDown(_keycodeDictionary[type]))
        {
            _currentInputHash.Add(type);
            return true;
        }
        return false;
    }
    protected bool IsKeyUp(eInputType type) => Input.GetKeyUp(_keycodeDictionary[type]);
    public void Update()
    {
        OnUpdate();
    }
    protected abstract void OnUpdate();
}
