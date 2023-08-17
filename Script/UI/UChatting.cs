using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UChatting : BaseUI
{
    CanvasGroup _canvasGroup;
    ChatMsgElement _elementResource;
    RectTransform _elementParent;
    RectTransform _scrollRectTransform;

    ObjectMemoryPool<ChatMsgElement> _msgElementObjectMemoryPool = new ObjectMemoryPool<ChatMsgElement>(10);
    Queue<ChatMsgElement> _activeMsgElement = new Queue<ChatMsgElement>();
    LinkedList<MessageStructure> _msgLogContainer = new LinkedList<MessageStructure>();

    TMP_InputField _messageField;
    bool _isBlackColor = false;

    const float _sendTime = 1f;
    float _sendElapsedTime;

    Coroutine _stateAnimationCoroutine;
    Coroutine _closeReservationCoroutine;
    Dictionary<bool, Color> _backgroundColorDic = new Dictionary<bool, Color>() {
        { false, new Color(0.56f, 0.56f, 0.56f, 0.4f) },
        { true, new Color(0f, 0f, 0f, 0.4f) }
    };
    bool _isActviveWriteField;
    public bool ActiveTextField
    {
        get => _isActviveWriteField;
        set
        {
            _isActviveWriteField = value;
            if (value)
            {
                _messageField.gameObject.SetActive(true);
                _messageField.onEndEdit.AddListener(OnDisableFocus);
                _messageField.ActivateInputField();
            }
            else
            {
                _messageField.gameObject.SetActive(false);
                _messageField.onEndEdit.RemoveAllListeners();
                _messageField.text = null;
                if (_closeReservationCoroutine != null)
                    StopCoroutine(_closeReservationCoroutine);

                _closeReservationCoroutine = StartCoroutine(IECloseReservation());
            }
        }
    }
    IEnumerator IEFadeElement(bool isOpen, float targetTime)
    {
        float elapsedTime = 0f;
        if (isOpen) targetTime *= (1f - _canvasGroup.alpha);
        else targetTime *= _canvasGroup.alpha;
        float startAlpha = _canvasGroup.alpha;
        float targetAlpha = isOpen ? 1f : 0f;
        while (elapsedTime <= targetTime)
        {
            yield return null;
            elapsedTime += TimeManager.DeltaTime;
            _canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / targetTime);
        }
        _stateAnimationCoroutine = null;
        if (!isOpen)
            gameObject.SetActive(false);
    }
    IEnumerator IECloseReservation()
    {
        float elapsedTime = 0f;
        float targetTime = 3f;
        while (elapsedTime <= targetTime)
        {
            yield return null;
            elapsedTime += TimeManager.DeltaTime;
        }
        _closeReservationCoroutine = null;
        Disable();
    }
    void OnDisableFocus(string a) => _messageField.ActivateInputField();
    protected override void InitReference()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        _canvasGroup.alpha = 0f;
        _scrollRectTransform = transform.Find("LogLayer") as RectTransform;
        _elementParent = _scrollRectTransform.GetComponentInChildren<ContentSizeFitter>(true).transform as RectTransform;
        _elementResource = Resources.Load<ChatMsgElement>("UI/CacheElement/ChatMsgElement");

        Transform messageLayer = transform.Find("MessageLayer");
        _messageField = messageLayer.Find("InputField_Message").GetComponent<TMP_InputField>();
        PlayerEventReceiver.MessageUIEvent += PushMessage;
    }
    public void PushMessage(MessageStructure msgStructure)
    {
        ChatMsgElement msg = _msgElementObjectMemoryPool.GetItem();
        if (msg == null)
        {
            msg = Instantiate(_elementResource, _elementParent);
            msg.Initialize();
        }
        _isBlackColor = !_isBlackColor;
        msg.Enable(msgStructure, _backgroundColorDic[_isBlackColor]);
        _activeMsgElement.Enqueue(msg);
        _msgLogContainer.AddLast(msgStructure);
        if (_scrollRectTransform.rect.height < _elementParent.rect.height + msg.RectTransform.rect.height)
        {
            Vector2 position = _elementParent.anchoredPosition;
            position.y += msg.RectTransform.rect.height * 2f;
            _elementParent.anchoredPosition = position;
        }
        Enable();
    }
    protected override void OnOpen()
    {
        Enable();
    }
    protected override void OnClose()
    {
        
    }
    public void Enable()
    {
        gameObject.SetActive(true);
        if (_stateAnimationCoroutine != null)
            StopCoroutine(_stateAnimationCoroutine);

        _stateAnimationCoroutine = StartCoroutine(IEFadeElement(true, _isActviveWriteField ? 0.66f : 0.33f));

        if (_closeReservationCoroutine != null)
            StopCoroutine(_closeReservationCoroutine);

        if (!_isActviveWriteField)
            _closeReservationCoroutine = StartCoroutine(IECloseReservation());
    }
    public void Disable()
    {
        if (_stateAnimationCoroutine != null)
            StopCoroutine(_stateAnimationCoroutine);

        _stateAnimationCoroutine = StartCoroutine(IEFadeElement(false, 1.5f));
    }
    private void LateUpdate()
    {
        if (_sendElapsedTime < _sendTime)
            _sendElapsedTime += TimeManager.DeltaTime;
    }
    public void SendMessage()
    {
        if (string.IsNullOrEmpty(_messageField.text)) return;
        if (_sendTime > _sendElapsedTime) return;
        NetworkManager.Instance.PlayerEventSender.RequestSendMessage(new MessageStructure()
        {
            Type = eMessageType.Normal,
            PlayerID = PlayerManager.Instance.Me.PlayerID,
            Name = PlayerManager.Instance.Me.Character.Name,
            Message = _messageField.text,
        });
    }
}
