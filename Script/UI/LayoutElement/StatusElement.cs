using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatusElement : MonoBehaviour
{
    TextMeshProUGUI _nameText;
    TextMeshProUGUI _valueText;

    StatusFloat _statusFloat;
    [SerializeField] protected eStatusType _statusType;
    public StatusElement Initialize()
    {
        _nameText = transform.Find("Text_Type").GetComponent<TextMeshProUGUI>();
        _valueText = transform.Find("Text_Value").GetComponent<TextMeshProUGUI>();
        return this;
    }
    public void Enable(ActorStat actorStat)
    {
        _statusFloat = actorStat.GetStatusContents(_statusType);
        if(_statusFloat != null)
        {
            _statusFloat.OnChangedEvent += OnChangedCallback;
            OnChangedCallback(_statusFloat.GetValue);
        }
        OnDrawText();
    }
    public void Disable()
    {
        if (_statusFloat != null)
            _statusFloat.OnChangedEvent -= OnChangedCallback;
    }
    void OnChangedCallback(float value)
    {
        _valueText.text = value.ToString();
    }
    void OnDrawText()
    {
        switch (_statusType)
        {
            case eStatusType.STR:
                _nameText.text = LocalizingManager.Instance.GetLocalizing(101);
                break;
            case eStatusType.DEX:
                _nameText.text = LocalizingManager.Instance.GetLocalizing(102);
                break;
            case eStatusType.INT:
                _nameText.text = LocalizingManager.Instance.GetLocalizing(103);
                break;
            case eStatusType.WIS:
                _nameText.text = LocalizingManager.Instance.GetLocalizing(104);
                break;
            case eStatusType.HP:
                _nameText.text = LocalizingManager.Instance.GetLocalizing(105);
                break;
        }
    }
}
