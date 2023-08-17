using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class BaseUI : MonoBehaviour
{
    public bool IsActive;
    public RectTransform rectTransform;
    protected BaseInputHandler _inputHandler;
    public BaseUI Initialize()
    {
        rectTransform = transform as RectTransform;
        InitReference();
        InitInput();
        InitSound();
        return this;
    }
    protected virtual void InitReference()
    {

    }
    protected virtual void InitInput()
    {

    }
    protected virtual void InitSound()
    {

    }
    public virtual void Refresh() { } 
    public void Open()
    {
        if(_inputHandler != null)
            InputManager.Instance.AddUIHandler(_inputHandler);

        OnOpen();
        Refresh();
        IsActive = true;
    }
    public void Close()
    {
        if (_inputHandler != null)
            InputManager.Instance.RemoveUIHandler(_inputHandler);

        OnClose();
        IsActive = false;
    }
    protected abstract void OnOpen();
    protected abstract void OnClose();
    public virtual void Release()
    {
        Close();
    }
    protected virtual void OnUpdateInput()
    {

    }
}