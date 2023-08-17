using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UMinimapPin : MonoBehaviour
{
    Dictionary<ePinType, GameObject> _pinObjectDic;
    MinimapPinStructure _pinData;
    RectTransform _rectTransform;
    public Vector2 Offset { private get; set; }
    public bool IsActivate => _pinData.IsActive;
    public void Initialize()
    {
        _rectTransform = transform as RectTransform;
        _pinObjectDic = new Dictionary<ePinType, GameObject>(transform.childCount);
        _pinObjectDic.Add(ePinType.Player, transform.Find("Player").gameObject);
        _pinObjectDic.Add(ePinType.Friend, transform.Find("Friend").gameObject);
    }
    public void Enable(MinimapPinStructure pinData)
    {
        _pinObjectDic[pinData.PinType].SetActive(true);
        _pinData = pinData;
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        _pinObjectDic[_pinData.PinType].SetActive(false);
        _pinData = null;
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        _rectTransform.anchoredPosition = ((Vector2)_pinData.Transform.position - Offset) * 2;
    }
}
