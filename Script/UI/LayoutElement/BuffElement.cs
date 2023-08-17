using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class BuffElement : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Buff _buff;
    Image _iconImg;
    Image _progressImg;
    public void Initialize()
    {
        _iconImg = GetComponent<Image>();
        _progressImg = transform.Find("Img_Progress").GetComponent<Image>();
    }
    public void Enable(Buff buff)
    {
        if (_buff == buff)
            return;

        _buff = buff;
        _iconImg.sprite = _buff.Data.Icon;
        _progressImg.sprite = _buff.Data.Icon;
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        _buff.OnDisableCallback -= OnDisableToolTip;
        _buff = null;
        _iconImg.sprite = null;
        _progressImg.sprite = null;
        gameObject.SetActive(false);
    }
    void OnDisableToolTip()
    {
        _buff.OnDisableCallback -= OnDisableToolTip;
        UIManager.Instance.Close(eUIName.UToolTipUI);
    }
    private void LateUpdate()
    {
        _progressImg.fillAmount = _buff.GetProgress;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.Open<UToolTipUI>(eUIName.UToolTipUI).SetBuffToolTip(transform.position, _buff);
        _buff.OnDisableCallback += OnDisableToolTip;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        OnDisableToolTip();
    }
}
