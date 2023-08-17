using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UStatusHUD : BaseUI
{
    UCurrentProgressLayer _currentProgressLayer;
    UCurrentStatusStateLayer _currentStatusStateLayer;
    UBuffLayer _buffLayer;
    public Character Character
    {
        set {
            _currentProgressLayer.Enable(value);
            _currentStatusStateLayer.Enable(value);
            _buffLayer.Enable(value);
        }
    }
    protected override void InitReference()
    {
        _currentProgressLayer = transform.Find("UCurrentProgressLayer").GetComponent<UCurrentProgressLayer>();
        _currentProgressLayer.Initialize();
        _currentStatusStateLayer = transform.Find("UCurrentStatusStateLayer").GetComponent<UCurrentStatusStateLayer>();
        _currentStatusStateLayer.Initialize();
        _buffLayer = transform.Find("UBuffLayer").GetComponent<UBuffLayer>();
        _buffLayer.Initialize();
    }
    protected override void OnOpen()
    {
        gameObject.SetActive(true);
    }
    protected override void OnClose()
    {
        gameObject.SetActive(false);
    }
}
