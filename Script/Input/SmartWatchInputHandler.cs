using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmartWatchInputHandler : BaseInputHandler
{
    USmartWatchUI _smartWatchUI;
    public SmartWatchInputHandler()
    {
        _smartWatchUI = UIManager.Instance.Get<USmartWatchUI>(eUIName.USmartWatchUI);
    }
    protected override void OnUpdate()
    {
        if (IsKeyDown(eInputType.Left))
        {
            _smartWatchUI.SelectBefore();
        }
        if (IsKeyDown(eInputType.Right))
        {
            _smartWatchUI.SelectAfter();
        }
    }
}
