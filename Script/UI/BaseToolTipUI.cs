using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class BaseToolTipUI : MonoBehaviour
{
    protected eLanguage _currentLanguage;
    protected RectTransform _rectTransform;
    Vector2 _initScale;

    protected TextMeshProUGUI _nameText;
    protected TextMeshProUGUI _informationText;
    public BaseToolTipUI Initialize()
    {
        InitReference();
        return this;
    }
    protected virtual void InitReference()
    {
        _rectTransform = transform as RectTransform;
        _initScale = _rectTransform.sizeDelta;

        _nameText = transform.Find("Text_Name").GetComponent<TextMeshProUGUI>();
        _informationText = transform.Find("Text_Information").GetComponent<TextMeshProUGUI>();
    }
    public void RefreshLocalizing()
    {
        if (_currentLanguage != RuntimePreference.Preference.Language)
        {
            _currentLanguage = RuntimePreference.Preference.Language;
            OnRefreshLocalizing();
        }
    }
    public abstract void Disable();
    protected virtual void OnRefreshLocalizing()
    {

    }
    protected void OnProcessScale()
    {
        _rectTransform.sizeDelta = _initScale + Vector2.up * _informationText.preferredHeight;
    }
    protected void OnProcessPosition(Vector2 position)
    {
        float overHeight = position.y + _rectTransform.rect.height;
        if (overHeight > Screen.height)
            position.y -= (overHeight - Screen.height) * Screen.height / 1080f;

        float overWidth = position.x + _rectTransform.rect.width;
        if (overWidth > Screen.width)
            position.x -= (overWidth - Screen.width) * Screen.width / 1920f;

        _rectTransform.position = position;
    }
}
