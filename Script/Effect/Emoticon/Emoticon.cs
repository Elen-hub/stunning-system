using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emoticon : MonoBehaviour
{
    public DelegateUtility.GetBoolDelegate DisableCondition;
    public event System.Action<Emoticon> OnDisableEvent;
    eEmoticonType _type;
    public eEmoticonType EmoticonType => _type;
    Transform _followTransform;
    float _elapsedTime = 0f;
    float _durationTime;
    public void Initialize(eEmoticonType type)
    {
        _type = type;
    }
    public void Enable(Vector3 position, float durationTime)
    {
        _durationTime = durationTime;
        _followTransform.position = position;
        gameObject.SetActive(true);
    }
    public void Enable(Transform targetTransform, float durationTime)
    {
        _durationTime = durationTime;
        _followTransform = targetTransform;
        transform.SetParent(targetTransform);
        transform.localPosition = Vector3.zero;
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        DisableCondition = null;
        _elapsedTime = 0f;
        OnDisableEvent?.Invoke(this);
        if (_followTransform != null)
        {
            _followTransform = null;
            transform.SetParent(_emoticonFactory.transform);
        }
        OnDisableEvent = null;
        gameObject.SetActive(false);
    }
    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        if (_elapsedTime > _durationTime)
            Disable();

        if (DisableCondition != null)
            if (DisableCondition())
                Disable();
    }

    #region Spawner
    public static void StaticInitialize()
    {
        _emoticonFactory = new EmoticonFactory();
    }
    static EmoticonFactory _emoticonFactory;
    public static void SetEmoticon(DynamicActor targetActor, eEmoticonType type, eAttachmentTarget attachmentType, bool isFollow, float durationTime)
    {
        Emoticon emoticon = _emoticonFactory.SetEmoticon(targetActor, type, attachmentType, isFollow, durationTime);
        switch (type)
        {
            case eEmoticonType.SleepMark:
                emoticon.DisableCondition = delegate {
                    Debug.Log(targetActor.IsSleep);
                    if (emoticon._elapsedTime < 0.5f)
                        return false;

                    return !targetActor.IsSleep;
                };
                break;
        }
    }
    #endregion
}
