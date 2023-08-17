using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Status
{
    protected uint _currentKey;
    protected Dictionary<uint, float> _dataMulDic = new Dictionary<uint, float>();
    public virtual event System.Action<float> OnChangedEvent;
    protected virtual void OnChangedCallback(float value) => OnChangedEvent?.Invoke(value);
    protected abstract void Calculate();
    public abstract uint AddSum(float value);
    public abstract uint AddSum(int value);
    public uint AddMul(float value)
    {
        ++_currentKey;
        _dataMulDic.Add(_currentKey, value);
        Calculate();

        return _currentKey;
    }
    public abstract void Remove(eStatusCalculateSign type, uint key);
    public abstract void UpdateStatus(eStatusCalculateSign type, uint key, object value);
    public static implicit operator bool(Status exists)
    {
        return exists != null;
    }
    public abstract void Clear();
}
public class StatusInt : Status
{
    Dictionary<uint, int> _dataSumDic = new Dictionary<uint, int>();
    int _abs;
    int _value;
    public int GetValue => _value;
    public int GetDefaultValue => _dataSumDic[0];
    public StatusInt(int defaultValue)
    {
        _dataSumDic.Add(0, defaultValue);
        Calculate();
    }
    protected override void Calculate()
    {
        float sum = 0;
        float mul = 0;
        foreach (var content in _dataSumDic)
            sum += content.Value;
        foreach (var content in _dataMulDic)
            mul *= content.Value;

        float calculate = (sum * mul);
        _value = (int)calculate;
        OnChangedCallback(calculate);
    }
    public override uint AddSum(int value)
    {
        ++_currentKey;
        _dataSumDic.Add(_currentKey, value);
        Calculate();
        return _currentKey;
    }
    public override uint AddSum(float value)
    {
        ++_currentKey;
        _dataSumDic.Add(_currentKey, (int)value);
        Calculate();
        return _currentKey;
    }
    public override void Remove(eStatusCalculateSign type, uint key)
    {
        switch (type)
        {
            case eStatusCalculateSign.Add:
                _dataSumDic.Remove(key);
                break;
            case eStatusCalculateSign.Mul:
                _dataMulDic.Remove(key);
                break;
        }
        Calculate();
    }
    public override void UpdateStatus(eStatusCalculateSign type, uint key, object value)
    {
        int convertValue = (int)value;
        switch (type)
        {
            case eStatusCalculateSign.Add:
                _dataSumDic[key] = convertValue;
                break;
            case eStatusCalculateSign.Mul:
                _dataMulDic[key] = convertValue;
                break;
        }
        Calculate();
    }

    public override void Clear()
    {
        _value = _dataSumDic[0];
        _dataSumDic.Clear();
        _dataMulDic.Clear();
        _dataSumDic.Add(0, _value);
    }
}
public class StatusFloat : Status
{
    Dictionary<uint, float> _dataSumDic = new Dictionary<uint, float>();
    public float SetDefaultValue { set { _dataSumDic[0] = value; Calculate(); } }
    float _abs;
    float _value;
    public float GetValue => _value;
    public float GetDefaultValue => _dataSumDic[0];
    public StatusFloat(float defaultValue)
    {
        _dataSumDic.Add(0, defaultValue);
        Calculate();
    }
    protected override void Calculate()
    {
        float sum = 0;
        float mul = 1;
        foreach (var content in _dataSumDic)
            sum += content.Value;
        foreach (var content in _dataMulDic)
            mul *= content.Value;

        _value = sum * mul;
        OnChangedCallback(_value);
    }
    public override uint AddSum(int value)
    {
        ++_currentKey;

        _dataSumDic.Add(_currentKey, value);
        Calculate();
        return _currentKey;
    }
    public override uint AddSum(float value)
    {
        ++_currentKey;
        _dataSumDic.Add(_currentKey, value);
        Calculate();
        return _currentKey;
    }
    public override void Remove(eStatusCalculateSign type, uint key)
    {
        switch (type)
        {
            case eStatusCalculateSign.Add:
                _dataSumDic.Remove(key);
                break;
            case eStatusCalculateSign.Mul:
                _dataMulDic.Remove(key);
                break;
        }
        Calculate();
    }
    public override void UpdateStatus(eStatusCalculateSign type, uint key, object value)
    {
        float convertValue = (float)value;
        switch (type)
        {
            case eStatusCalculateSign.Add:
                _dataSumDic[key] = convertValue;
                break;
            case eStatusCalculateSign.Mul:
                _dataMulDic[key] = convertValue;
                break;
        }
        Calculate();
    }

    public override void Clear()
    {
        _value = _dataSumDic[0];
        _dataSumDic.Clear();
        _dataMulDic.Clear();
        _dataSumDic.Add(0, _value);
    }
}
