using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ChatMsgElement : MonoBehaviour
{
    public RectTransform RectTransform => _rectTransform;
    RectTransform _rectTransform;
    Image _background;
    TextMeshProUGUI _idText;
    TextMeshProUGUI _messageText;
    public void Initialize()
    {
        _rectTransform = transform as RectTransform;
        _background = GetComponent<Image>();
        _idText = transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
        _messageText = transform.Find("Text_Message").GetComponent<TextMeshProUGUI>();
    }
    public void Enable(MessageStructure msgStructure, Color bgColor)
    {
        _idText.text = msgStructure.Name;
        _messageText.text = msgStructure.Message;
        _background.rectTransform.sizeDelta = new Vector2(_background.rectTransform.sizeDelta.x, _messageText.preferredHeight * 1.25f);
        _background.color = bgColor;
    }
}
