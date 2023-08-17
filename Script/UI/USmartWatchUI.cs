using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class USmartWatchUI : BaseUI
{
    CircularLinkedList<USmartWatchSubUI> _subUILinkedList;
    CircularLinkedListNode<USmartWatchSubUI> _currentUI;
    bool _isChanged;
    protected override void InitReference()
    {
        base.InitReference();

        _subUILinkedList = new CircularLinkedList<USmartWatchSubUI>();
        USmartWatchSubUI mainLayer = transform.Find("Canvas/MainLayer").GetComponent<USmartWatchSubUI>();
        mainLayer.Initialize();
        _subUILinkedList.AddLast(mainLayer);
        USmartWatchSubUI statusLayer = transform.Find("Canvas/StatusLayer").GetComponent<USmartWatchSubUI>();
        statusLayer.Initialize();
        _subUILinkedList.AddLast(statusLayer);
        USmartWatchSubUI minimapLayer = transform.Find("Canvas/MinimapLayer").GetComponent<USmartWatchSubUI>();
        minimapLayer.Initialize();
        _subUILinkedList.AddLast(minimapLayer);

        _inputHandler = new SmartWatchInputHandler();
    }
    protected override void OnOpen()
    {
        if (_currentUI != null)
            _currentUI.GetValue.Disable();

        _currentUI = _subUILinkedList.Head;
        _currentUI.GetValue.RectTransform.anchoredPosition = Vector2.zero;
        _currentUI.GetValue.Enable();
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }
    public void SelectBefore()
    {
        if(!_isChanged)
            StartCoroutine(IEChanged(false));
    }
    public void SelectAfter()
    {
        if (!_isChanged)
            StartCoroutine(IEChanged(true));
    }
    IEnumerator IEChanged(bool isNext)
    {
        const float switchTime = 0.15f;
        _isChanged = true;
        float elapsedTime = 0f;
        CircularLinkedListNode<USmartWatchSubUI> nextNode = isNext ? _currentUI.Next : _currentUI.Prev;
        nextNode.GetValue.Enable();
        Vector2 targetPosition = new Vector2(_currentUI.GetValue.RectTransform.rect.size.x * 2, 0);
        while (elapsedTime <= switchTime)
        {
            elapsedTime += TimeManager.DeltaTime;
            if(isNext)
            {
                _currentUI.GetValue.RectTransform.anchoredPosition = Vector2.Lerp(Vector3.zero, -targetPosition, elapsedTime / switchTime);
                nextNode.GetValue.RectTransform.anchoredPosition = Vector2.Lerp(targetPosition, Vector3.zero, elapsedTime / switchTime);
            }
            else
            {
                _currentUI.GetValue.RectTransform.anchoredPosition = Vector2.Lerp(Vector3.zero, targetPosition, elapsedTime / switchTime);
                nextNode.GetValue.RectTransform.anchoredPosition = Vector2.Lerp(-targetPosition, Vector3.zero, elapsedTime / switchTime);
            }
            yield return null;
        }
        _currentUI.GetValue.Disable();
        _currentUI = nextNode;
        _isChanged = false;
    }
}
