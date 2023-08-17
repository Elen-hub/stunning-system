using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[System.Serializable]
public class StatusStateInfo
{
    public Sprite Sprite;
    public int NameKey;
    public int DescriptionKey;
}
public class StatusState : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool _isActive;
    Character _character;
    eStatusType _statusType;
    eStatusState _currentStatusState;
    Image _currentImage;
    [SerializeField] protected StatusStateInfo _highnestStateInfo;
    [SerializeField] protected StatusStateInfo _highStateInfo;
    [SerializeField] protected StatusStateInfo _middleStateInfo;
    [SerializeField] protected StatusStateInfo _lowStateInfo;
    [SerializeField] protected StatusStateInfo _lowestStateInfo;

    public System.Action OnChagnedInfoCallback;
    public StatusStateInfo CurrentInfo { get; set; }
    public StatusState Initialize(eStatusType statusType)
    {
        _statusType = statusType;
        _currentImage = GetComponent<Image>();
        return this;
    }
    public void Enable(Character character)
    {
        _character = character;
        _isActive = true;
        gameObject.SetActive(true);
        SetStateInfo(character.GetStatusState(_statusType));
    }
    public void Disable()
    {
        gameObject.SetActive(false);
    }
    void OnUpdateElement(StatusStateInfo targetStateInfo)
    {
        CurrentInfo = targetStateInfo;
        if (targetStateInfo.Sprite != null)
        {
            _currentImage.sprite = targetStateInfo.Sprite;
            if (!_isActive)
            {
                gameObject.SetActive(true);
                _isActive = true;
            }
        }
        else if (_isActive)
        {
            gameObject.SetActive(false);
            _isActive = false;
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.Instance.Open<UToolTipUI>(eUIName.UToolTipUI).SetStatusStateToolTip(transform.position, this);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.Instance.Close(eUIName.UToolTipUI);
    }
    void SetStateInfo(eStatusState state)
    {
        _currentStatusState = state;
        switch (_currentStatusState)
        {
            case eStatusState.Highnest:
                OnUpdateElement(_highnestStateInfo);
                break;
            case eStatusState.High:
                OnUpdateElement(_highStateInfo);
                break;
            case eStatusState.Middle:
                OnUpdateElement(_middleStateInfo);
                break;
            case eStatusState.Low:
                OnUpdateElement(_lowStateInfo);
                break;
            case eStatusState.Lowest:
                OnUpdateElement(_lowestStateInfo);
                break;
        }
        if (OnChagnedInfoCallback != null)
            OnChagnedInfoCallback();
    }
    public void OnUpdate()
    {
        if (_character == null)
            return;

        eStatusState statusState = _character.GetStatusState(_statusType);
        if (_currentStatusState != statusState)
            SetStateInfo(statusState);
    }
}
