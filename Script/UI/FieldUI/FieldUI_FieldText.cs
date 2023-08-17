using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class FieldUI_FieldText : BaseFieldUI
{
    TextMeshProUGUI _fieldText;
    IActor _owner;
    float _elapsedTime = 0f;
    const float _targetTime = 3f;
    Vector2 _posAdditinal;
    public override BaseFieldUI Init(ObjectMemoryPool<BaseFieldUI>.del_Register register)
    {
        base.Init(register);

        _fieldText = transform.Find("Text").GetComponent<TextMeshProUGUI>();
        return this;
    }
    public void Enable(IActor owner, int localizingKey, params object[] parsingParameters)
    {
        _owner = owner;
        _fieldText.text = LocalizingManager.Instance.GetLocalizing(localizingKey, parsingParameters);
        _posAdditinal.y = -0.25f;
        _elapsedTime = 0f;
        gameObject.SetActive(true);
    }
    public void Enable(IActor owner, string log)
    {
        _owner = owner;
        _fieldText.text = log;
        _posAdditinal.y = -0.25f;
        _elapsedTime = 0f;
        transform.position = _owner.Position + _posAdditinal;
        gameObject.SetActive(true);
    }
    public void Disable()
    {
        del_Register(this);
        gameObject.SetActive(false);
    }
    private void Update()
    {
        transform.position = _owner.Position;
        _elapsedTime += TimeManager.DeltaTime;
        if (_elapsedTime <= 0.5f)
            _fieldText.color = Color.Lerp(Color.clear, Color.yellow, _elapsedTime * 2);
        if(_elapsedTime >= _targetTime - 0.5f)
            _fieldText.color = Color.Lerp(Color.yellow, Color.clear, _elapsedTime - _targetTime + 0.5f);
        transform.position = _owner.Position + _posAdditinal;
        _posAdditinal.y += TimeManager.DeltaTime * 0.25f;
        if (_elapsedTime > _targetTime)
            Disable();
    }
}
