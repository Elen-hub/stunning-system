using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class USelectionItem : BaseUI
{
    Item GetItem => ItemManager.Instance.SelectionItem;
    Image _itemImage;
    TextMeshProUGUI _countText;
    int _currentCount;
    protected override void InitReference()
    {
        _itemImage = transform.Find("Img_Icon").GetComponent<Image>();
        _countText = transform.Find("Text_Count").GetComponent<TextMeshProUGUI>();
    }
    protected override void OnOpen()
    {
        ProcessImage();
        OnProcessText();
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }
    public void ProcessImage()
    {
        if (GetItem != null)
        {
            _itemImage.sprite = GetItem.ItemData.Icon;
            _itemImage.enabled = true;
        }
        else
            _itemImage.enabled = false;
    }
    void OnProcessText()
    {
        if (GetItem == null)
        {
            _countText.enabled = false;
            return;
        }
        if (GetItem.ItemData.MergeCount > 1)
        {
            _countText.enabled = true;
            if (_currentCount != GetItem.Count)
            {
                _currentCount = GetItem.Count;
                _countText.text = _currentCount.ToString();
            }
        }
        else
            _countText.enabled = false;
    }
    private void LateUpdate()
    {
        transform.position = Input.mousePosition;
    }
}
