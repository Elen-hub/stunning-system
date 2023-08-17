using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RectScaleProgress : MonoBehaviour
{
    [SerializeField] protected Image _progressImg;
    [SerializeField] protected Image _trailProgressImg;
    Vector2 _zeroProgressScale;
    Vector2 _progressScale;
    Vector2 _zeroTrailScale;
    Vector2 _trailScale;
    [SerializeField] float _progressFillAmount;
    public float ProgressFillAmount {
        get => _progressFillAmount;
        set {
            _progressFillAmount = value;
            _progressImg.rectTransform.sizeDelta = Vector2.Lerp(_zeroProgressScale, _progressScale, _progressFillAmount);
        }
    }
    float _trailFillAmount;
    public float TrailFillAmount {
        get => _trailFillAmount;
        set {
            _trailFillAmount = value;
            _trailProgressImg.rectTransform.sizeDelta = Vector2.Lerp(_zeroTrailScale, _trailScale, _trailFillAmount);
        }
    }
    bool _useHPText;
    TextMeshProUGUI _hpText;

    float _currentHP;
    float _trailRate;
    float _trailTime = 0f;

    public DelegateUtility.GetFloatDelegate GetCurrentAmount;
    public DelegateUtility.GetFloatDelegate GetMaxAmount;

    public void Initialize()
    {
        _progressScale = _progressImg.rectTransform.sizeDelta;
        _zeroProgressScale = new Vector2(0f, _progressScale.y);
        if (_trailProgressImg != null)
        {
            _trailScale = _trailProgressImg.rectTransform.sizeDelta;
            _zeroTrailScale = new Vector2(0f, _trailScale.y);
            _trailProgressImg.rectTransform.sizeDelta = _zeroTrailScale;
        }
        _progressImg.rectTransform.sizeDelta = _zeroProgressScale;
    }
    void OnUpdateText()
    {
        if (_useHPText)
        {
            float tempHP = GetCurrentAmount();
            if (_currentHP != tempHP)
            {
                _currentHP = tempHP;
                _hpText.text = _currentHP.ToString();
            }
        }
    }
    void OnUpdateHPRate()
    {
        _currentHP = GetCurrentAmount() / GetMaxAmount();
        if (_currentHP != ProgressFillAmount)
        {
            ProgressFillAmount = _currentHP;
            if (_trailProgressImg != null)
            {
                if (_trailRate == _currentHP)
                    _trailTime = 0f;
            }
        }
    }
    void OnUpdateTrailRate()
    {
        if (_trailProgressImg == null)
            return;

        if (_currentHP != _trailRate)
        {
            if (_currentHP > _trailRate)
            {
                _trailRate = _currentHP;
                _trailTime = 1.5f;
                TrailFillAmount = _trailRate;
            }

            _trailTime += Time.deltaTime;
            if (_trailTime > 0.25f)
            {
                _trailRate = Mathf.Lerp(_trailRate, _currentHP, Time.deltaTime * 3f);
                if (_trailRate - _currentHP <= float.Epsilon)
                    _trailRate = _currentHP;

                TrailFillAmount = _trailRate;
            }
        }
    }
    private void LateUpdate()
    {
        if (GetCurrentAmount == null || GetMaxAmount == null)
            return;

        OnUpdateText();
        OnUpdateHPRate();
        OnUpdateTrailRate();
    }
}
