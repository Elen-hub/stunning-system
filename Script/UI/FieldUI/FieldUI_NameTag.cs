using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FieldUI_NameTag : BaseFieldUI
{
    Transform _followTarget;
    TextMeshProUGUI _nameText;
    IRegistFieldUI _owner;
    static Dictionary<eActorType, Color> _nameTagColor = new Dictionary<eActorType, Color>()
    {
        { eActorType.Character, Color.blue },
        { eActorType.Monster, Color.red },
        { eActorType.StaticNPC, Color.white },
    };
    public override BaseFieldUI Init(ObjectMemoryPool<BaseFieldUI>.del_Register register)
    {
        base.Init(register);

        _nameText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        return this;
    }
    public void Enable(IRegistFieldUI owner)
    {
        _owner = owner;
        _followTarget = _owner.Attachment.GetAttachmentElement(eAttachmentTarget.NameTag).Transform;
        _nameText.text = owner.Name;
        // _nameText.color = _nameTagColor[owner.ActorType];
        _owner.OnDisableFieldUIEvent += Disable;
        _owner.OnVisibleEvent += gameObject.SetActive;
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        del_Register(this);
        _owner.OnDisableFieldUIEvent -= Disable;
        _owner.OnVisibleEvent -= gameObject.SetActive;
        gameObject.SetActive(false);
    }
    private void FixedUpdate()
    {
        transform.position = _followTarget.position;
    }
}
