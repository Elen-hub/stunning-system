using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UBuffToolTip : BaseToolTipUI
{
    Buff _trackingBuff;
    public void Enable(Vector3 uiPosition, Buff buff)
    {
        if(_trackingBuff != buff)
        {
            _trackingBuff = buff;
            OnProcessText();
            OnProcessScale();
        }
        OnProcessPosition(uiPosition);
        gameObject.SetActive(true);
    }
    public override void Disable()
    {
        gameObject.SetActive(false);
    }
    void OnProcessText()
    {
        _nameText.text = LocalizingManager.Instance.GetLocalizing(_trackingBuff.Data.NameKey);
        if(_trackingBuff.Data.IsMul) _informationText.text = LocalizingManager.Instance.GetLocalizing(_trackingBuff.Data.DescriptionKey, _trackingBuff.Data.Value * 100f);
        else _informationText.text = LocalizingManager.Instance.GetLocalizing(_trackingBuff.Data.DescriptionKey, _trackingBuff.Data.Value);
    }
}
