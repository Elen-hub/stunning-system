using UnityEngine;
using UnityEngine.UI;

public abstract class USmartWatchSubUI : UBaseSubUI
{
    public RectTransform RectTransform;
    protected RectMask2D _rectMask;
    protected override void InitReference()
    {
        base.InitReference();

        RectTransform = transform as RectTransform;
        _rectMask = GetComponent<RectMask2D>();
    }
    public abstract void Enable();
    public abstract void Disable();
}
