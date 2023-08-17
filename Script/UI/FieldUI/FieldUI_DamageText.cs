using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FieldUI_DamageText : BaseFieldUI
{
    Vector3 _pos;
    const float _targetTime = 1.25f;
    const float _fadeTime = 1f;
    const float _sizeTime = 0.2f;
    float _elapsedTime = 0;
    Text _damageText;
    Color _targetColor;
    public override BaseFieldUI Init(ObjectMemoryPool<BaseFieldUI>.del_Register register)
    {
        base.Init(register);

        _damageText = transform.GetComponentInChildren<Text>();
        return this;
    }
    public void Enabled(Vector3 pos, double damage, bool isCritical)
    {
        _pos = pos;
        _targetColor = isCritical ? Color.red : Color.white;
        _damageText.rectTransform.localScale = Vector3.zero;
        _damageText.color = _targetColor;
        transform.position = pos;
        _elapsedTime = 0;
        _damageText.text = damage.ToString();
        gameObject.SetActive(true);
    }
    public void Disabled()
    {
        del_Register(this);
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if(_elapsedTime > _targetTime)
        {
            Disabled();
            return;
        }
        _elapsedTime += Time.deltaTime;
        if (_sizeTime > _elapsedTime)
            _damageText.rectTransform.localScale = Vector3.one * Mathf.Lerp(1f, 3f, _elapsedTime / _sizeTime);
        else
        {
            _pos += Vector3.up * Time.deltaTime;
            transform.position = _pos;
        }

        if (_fadeTime < _elapsedTime)
            _damageText.color = Color.Lerp(_targetColor, Color.clear, (_elapsedTime - _fadeTime) / _fadeTime);
    }
}
