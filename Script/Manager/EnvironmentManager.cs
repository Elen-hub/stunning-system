using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : TSingletonMono<EnvironmentManager>
{
    public bool IsActivateEnvironment;
    Dictionary<eTimeTag, float> _timeDic = new Dictionary<eTimeTag, float>();
    public Dictionary<eTimeTag, float> TimeTagDictionary => _timeDic;
    [SerializeField] float _roomTime = 500f;
    public float GetRoomTime => _roomTime;
    [SerializeField] int _hour;
    public int GetHour => _hour;
    [SerializeField] int _minute;
    public int GetMinute => _minute;
    [SerializeField][Range(0f,30f)] float _speed = 5f;
    [SerializeField] long _liveTime;
    public LightController LightController;
    public WeatherController WeatherController = new WeatherController();

    [SerializeField] eTimeTag _timeTag;
    event System.Action<eTimeTag> OnChangedTimeTag;
    public event System.Action<int> OnChangedHour;
    public event System.Action<int> OnChangedMinute;
    public int GetByteSize => Network.ReliableHelper.FloatSize + WeatherController.GetByteSize;

    int _currentTimeIndex;
    eTimeTag _tempTargetTimeTag;
    [SerializeField] float _currentTemp;
    public float GetTemp
    {
        get {
            if (_tempTargetTimeTag == _timeTag)
                return _currentTemp;

            _tempTargetTimeTag = _timeTag;
            switch (_timeTag) {
                case eTimeTag.Dawn:
                    _currentTemp =  GetEnvironmentData().MaximumTemp * 0.1f + GetEnvironmentData().MinimumTemp * 0.9f;
                    break;
                case eTimeTag.Twilight:
                    _currentTemp =  GetEnvironmentData().MaximumTemp * 0.5f + GetEnvironmentData().MinimumTemp * 0.5f;
                    break;
                case eTimeTag.Morning:
                    _currentTemp = GetEnvironmentData().MaximumTemp * 0.7f + GetEnvironmentData().MinimumTemp * 0.3f;
                    break;
                case eTimeTag.Noon:
                    _currentTemp = GetEnvironmentData().MaximumTemp;
                    break;
                case eTimeTag.Evening:
                    _currentTemp =  GetEnvironmentData().MaximumTemp * 0.3f + GetEnvironmentData().MinimumTemp * 0.7f;
                    break;
                case eTimeTag.Night:
                    _currentTemp = GetEnvironmentData().MaximumTemp * 0.1f + GetEnvironmentData().MinimumTemp * 0.9f;
                    break;
                case eTimeTag.Midnight:
                    _currentTemp = GetEnvironmentData().MinimumTemp;
                    break;
            }
            return _currentTemp;
        }
    }
    public Data.EnvironmentData GetEnvironmentData() => DataManager.Instance.EnvironmentTable[_currentTimeIndex];
    Data.EnvironmentData GetEnvironmentData(int timeIndex) {
        if (timeIndex > 365)
            timeIndex -= 365;
        else if (timeIndex < 1)
            timeIndex += 365;
        return DataManager.Instance.EnvironmentTable[timeIndex];
    }
    protected override void OnInitialize()
    {
        _currentTimeIndex = Random.Range(1, 366);
        _timeDic = new Dictionary<eTimeTag, float>() {
            { eTimeTag.Dawn, 0 },
            { eTimeTag.Twilight, GetEnvironmentData(_currentTimeIndex).SunriseTime },
            { eTimeTag.Evening,  GetEnvironmentData(_currentTimeIndex).SunSetTime },
        };
        float sum = _timeDic[eTimeTag.Twilight] + _timeDic[eTimeTag.Evening];
        float sub = (_timeDic[eTimeTag.Evening] - _timeDic[eTimeTag.Twilight]) * 0.1f;
        _timeDic.Add(eTimeTag.Morning, _timeDic[eTimeTag.Twilight] + sub);
        _timeDic.Add(eTimeTag.Noon, (sum) * 0.5f);
        _timeDic.Add(eTimeTag.Night, _timeDic[eTimeTag.Evening] + sub*3f);
        _timeDic.Add(eTimeTag.Midnight, 1380);

        GenerateEnvironment();
#if UNITY_SERVER
        OnDayChanged();
#endif
    }
    public void GenerateEnvironment()
    {
        IsActivateEnvironment = true;
        Transform lightController = transform.Find("LightController");
        if (lightController == null)
        {
            LightController = Instantiate(Resources.Load<LightController>("LightController"), transform);
            LightController.Initialize();
            OnChangedTimeTag += LightController.SetTimeTag;
            // OnChangedTimeTag += WeatherController.SetTimeTag;
        }
        WeatherController.Initialize();

        if (Application.isPlaying)
        {
#if UNITY_SERVER
            SetRuntimeParamerter(_roomTime);
#endif
        }
    }
    void OnDayChanged()
    {
        OnGenerateWeather();
        OnGenerateFog();
    }
    void OnGenerateWeather()
    {
        float randomProbability = Random.Range(0f, 1f);
        float currentProbability = GetEnvironmentData().FallProbability;
        if (randomProbability < currentProbability)
        {
            // Rain
            float intensity = Random.Range(GetEnvironmentData().FallAmount[0], GetEnvironmentData().FallAmount[1]);
            WeatherHandler weatherHandler = new WeatherHandler()
            {
                StartTime = Random.Range(0f, 1440f),
                DurationTime = Random.Range(120f, 1440f),
                Intencity = intensity,
                WeahterType = eWeatherType.Rain,
            };
            WeatherController.PushWeatherHandler(weatherHandler);
        }
        else if(randomProbability < currentProbability + GetEnvironmentData().SnowFallProbability)
        {
            // Snow
            float intensity = Random.Range(GetEnvironmentData().SnowFallAmount[0], GetEnvironmentData().SnowFallAmount[1]);
            WeatherHandler weatherHandler = new WeatherHandler()
            {
                StartTime = Random.Range(0f, 1440f),
                DurationTime = Random.Range(120f, 1440f),
                Intencity = intensity,
                WeahterType = eWeatherType.Snow,
            };
            WeatherController.PushWeatherHandler(weatherHandler);
        }
    }
    void OnGenerateFog()
    {
        float randomProbability = Random.Range(0f, 1f);
        if (randomProbability < RoomConfiguration.FogData.FogProbability)
        {
            float intensity = Random.Range(RoomConfiguration.FogData.FogIntencity * 0.5f, RoomConfiguration.FogData.FogIntencity);
            float startTime = Random.Range(240f, 360f);
            float durationTime = Random.Range(420f, 600f) - startTime;
            WeatherController.SetFogParameter(startTime, 0f, durationTime, intensity);
        }
    }
    public void SetRuntimeParamerter(float roomTime)
    {
        _roomTime = roomTime;
        _hour = Mathf.FloorToInt(_roomTime / 60);
        _minute = Mathf.FloorToInt(_roomTime % 60);
        eTimeTag runtimeTimeTag = eTimeTag.Midnight;
        if (_roomTime >= _timeDic[eTimeTag.Dawn] && _roomTime < _timeDic[eTimeTag.Twilight])
            runtimeTimeTag = eTimeTag.Dawn;
        else if (_roomTime >= _timeDic[eTimeTag.Twilight] && _roomTime < _timeDic[eTimeTag.Morning])
            runtimeTimeTag = eTimeTag.Twilight;
        else if (_roomTime >= _timeDic[eTimeTag.Morning] && _roomTime < _timeDic[eTimeTag.Noon])
            runtimeTimeTag = eTimeTag.Morning;
        else if (_roomTime >= _timeDic[eTimeTag.Noon] && _roomTime < _timeDic[eTimeTag.Evening])
            runtimeTimeTag = eTimeTag.Noon;
        else if (_roomTime >= _timeDic[eTimeTag.Evening] && _roomTime < _timeDic[eTimeTag.Night])
            runtimeTimeTag = eTimeTag.Evening;
        else if (_roomTime >= _timeDic[eTimeTag.Night] && _roomTime < _timeDic[eTimeTag.Midnight])
            runtimeTimeTag = eTimeTag.Night;

        _timeTag = runtimeTimeTag;
        _currentTemp = GetEnvironmentData().MaximumTemp * 0.1f + GetEnvironmentData().MinimumTemp * 0.9f;
        if (OnChangedTimeTag != null)
            OnChangedTimeTag(_timeTag);
    }
    void OnUpdateTime()
    {
        _roomTime += Time.deltaTime * _speed;
        if (_roomTime > 1440f)
        {
            _roomTime -= 1440f;
#if UNITY_SERVER
            OnDayChanged();
#endif
        }

        int hour = Mathf.FloorToInt(_roomTime / 60);
        if (_hour != hour)
        {
            _hour = Mathf.FloorToInt(_roomTime / 60);
            if (OnChangedHour != null)
                OnChangedHour(hour);
        }
        int minute = Mathf.FloorToInt(_roomTime % 60);
        if (_minute != minute)
        {
            _minute = minute;
            if (LightController != null)
                LightController.UpdateMinute();
            if (OnChangedMinute != null)
                OnChangedMinute(minute);
        }

        eTimeTag timeTag = GetTimeTag();
        if (_timeTag != timeTag)
        {
            _timeTag = timeTag;
            if (OnChangedTimeTag != null)
                OnChangedTimeTag(_timeTag);
        }
    }
    eTimeTag GetTimeTag()
    {
        switch (_timeTag)
        {
            default:
                if (_roomTime >= _timeDic[_timeTag + 1])
                    return _timeTag + 1;
                break;
            case eTimeTag.Midnight:
                if (_roomTime < _timeDic[eTimeTag.Midnight])
                    return eTimeTag.Dawn;
                break;
        }
        return _timeTag;
    }
    private void Update()
    {
        OnUpdateTime();
        WeatherController.Update();
    }
}
