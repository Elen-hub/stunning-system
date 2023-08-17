using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ModifySizingTMP : TextMeshProUGUI
{
    public bool HoldXOffset;
    public bool HoldYOffset;

    public override void GraphicUpdateComplete()
    {
        base.GraphicUpdateComplete();

        Vector2 currSize = rectTransform.sizeDelta;
        Vector2 deltaSize = GetPreferredValues();

        if(currSize.x < deltaSize.x)


        SetSizePreferredValue();
    }
    public void SetSizePreferredValue()
    {
        m_rectTransform.sizeDelta = GetPreferredValues();

        ForceMeshUpdate();
    }
}
