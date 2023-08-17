using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UStatusStateToolTip : BaseToolTipUI
{
    StatusState _statusState;
    public void Enable(Vector3 uiPosition, StatusState statusState)
    {
        _statusState = statusState;
        OnProcessText();
        OnProcessScale();
        _statusState.OnChagnedInfoCallback += OnProcessText;
        OnProcessPosition(uiPosition);
        gameObject.SetActive(true);
    }
    public override void Disable()
    {
        _statusState.OnChagnedInfoCallback -= OnProcessText;
        gameObject.SetActive(false);
    }
    void OnProcessText()
    {
        _nameText.text = LocalizingManager.Instance.GetLocalizing(_statusState.CurrentInfo.NameKey);
        _informationText.text = LocalizingManager.Instance.GetLocalizing(_statusState.CurrentInfo.DescriptionKey);
    }
}
