using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class FieldUI_MessageBox : BaseFieldUI
{
    RectTransform _rectTransform;
    public event UnityEngine.Events.UnityAction<int> DisableCallback;
    MessageStructure _messageStructure;
    float _elapsedTime = 0f;
    const float _targetTime = 5f;
    Transform _targetTransform;

    TextMeshProUGUI _text;
    public override BaseFieldUI Init(ObjectMemoryPool<BaseFieldUI>.del_Register register)
    {
        base.Init(register);

        _rectTransform = transform as RectTransform;
        _text = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        return this;
    }
    public void Enable(MessageStructure messageStructure)
    {
        _messageStructure = messageStructure;
        Player player = PlayerManager.Instance.GetPlayer(messageStructure.PlayerID);
        if(player != null)
        {
            DynamicActor dynamicActor = player.Character;
            if(dynamicActor != null)
            {
                _elapsedTime = 0f;
                _targetTransform = dynamicActor.Attachment.GetAttachmentElement(eAttachmentTarget.OverHead).Transform;
                _text.text = messageStructure.Message;
                _text.rectTransform.sizeDelta = Vector2.zero;
                _rectTransform.sizeDelta = Vector2.zero;
                gameObject.SetActive(true);
                Vector2 pos = _text.GetPreferredValues();
                if (pos.x > 200f)
                {
                    if (pos.x > 400f)
                    {
                        pos.x *= 0.3333333f;
                        pos.y *= 3;
                    }
                    else
                    {
                        pos.x *= 0.5f;
                        pos.y *= 2;
                    }
                }
                _text.rectTransform.sizeDelta = pos;
                pos.x += 30f;
                pos.y += 25f;
                _rectTransform.sizeDelta = pos;
                return;
            }
        }
        Disable();
    }
    public void Disable()
    {
        DisableCallback(_messageStructure.PlayerID);
        DisableCallback = null;
        del_Register(this);
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        transform.position = _targetTransform.position;
        _elapsedTime += Time.deltaTime;
        if(_elapsedTime > _targetTime)
        {
            Disable(); ;
        }
    }
}
