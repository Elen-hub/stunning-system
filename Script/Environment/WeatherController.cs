using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

[System.Serializable]
public class WeatherController
{
    public event System.Action<eWeatherType> OnChagnedWeatherEvent;
    public Fusion.NetworkRunner Runner => NetworkManager.Instance.Runner;
    Dictionary<eWeatherType, IWeather> _weatherDic;
    [SerializeField] WeatherHandler _currentWeatherHandler = WeatherHandler.Null;
    public WeatherHandler CurrentWeahterHandler => _currentWeatherHandler;
    [SerializeField] WeatherHandler _nextWeatherHandler = WeatherHandler.Null;
    public WeatherHandler NextWeatherHandler => _nextWeatherHandler;
    public FogHandler FogHandler { get; private set; }
    public int GetByteSize => _currentWeatherHandler.GetByteSize + (FogHandler != null ? FogHandler.GetByteSize : ReliableHelper.BooleanSize);
    public void Initialize()
    {
#if !UNITY_SERVER
        _weatherDic = CameraManager.Instance.GetCamera<MainCamera>(eCameraType.MainCamera).GetWeather;
#endif
    }
    public void SetFogParameter(float startTime, float elapsedTime, float durationTime, float intensity)
    {
        FogHandler = new FogHandler() {
            StartTime = startTime,
            ElapsedTime = elapsedTime,
            DurationTime = durationTime,
            Intencity = intensity,
        };
        NetworkManager.Instance.StageEventSender.NotifyStartFog(FogHandler);
    }
    public void SetRuntimeParamerter(WeatherHandler weatherHandler)
    {
        _currentWeatherHandler = weatherHandler;
        if(weatherHandler != WeatherHandler.Null)
            _weatherDic[weatherHandler.WeahterType].StartWeather();
    }
    public void PushWeatherHandler(WeatherHandler weatherHandler)
    {
        _nextWeatherHandler = weatherHandler;
    }
    public void StartWeather(WeatherHandler weatherHandler)
    {
        _currentWeatherHandler = weatherHandler;
#if UNITY_SERVER
        NetworkManager.Instance.StageEventSender.NotifyStartWeather(_currentWeatherHandler);
#else
        _weatherDic[weatherHandler.WeahterType].StartWeather();
        if (OnChagnedWeatherEvent != null)
            OnChagnedWeatherEvent(weatherHandler.WeahterType);
#endif
    }
    public void EndWeather()
    {
        if (_currentWeatherHandler != null)
        {
#if UNITY_SERVER
            NetworkManager.Instance.StageEventSender.NotifyEndWeahter();
#else
            _weatherDic[_currentWeatherHandler.WeahterType].EndWeather();
#endif
            _currentWeatherHandler = WeatherHandler.Null;
            if (OnChagnedWeatherEvent != null)
                OnChagnedWeatherEvent(eWeatherType.None);
        }
    }
    void OnUpdateWeatherHandler()
    {
        if (_currentWeatherHandler != WeatherHandler.Null)
        {
            _currentWeatherHandler.DurationTime -= TimeManager.DeltaTime;
#if UNITY_SERVER
            if (_currentWeatherHandler.DurationTime <= 0)
                EndWeather();
#endif
        }
        else
        {
            if (_nextWeatherHandler != WeatherHandler.Null)
            {
#if UNITY_SERVER
                if (EnvironmentManager.Instance.GetRoomTime > _nextWeatherHandler.StartTime)
                {
                    StartWeather(_nextWeatherHandler);
                    _nextWeatherHandler = WeatherHandler.Null;
                }
#endif
            }
        }
    }
    void OnUpdateFog()
    {
        if(FogHandler != null)
        {
            if (FogHandler.IsStart)
            {
                FogHandler.ElapsedTime += TimeManager.DeltaTime;
                if(FogHandler.ElapsedTime > FogHandler.DurationTime)
                {
                    RenderingManager.Instance.EndFog();
                    return;
                }
                float amount = FogHandler.ElapsedTime / FogHandler.DurationTime;
                if(amount <= 0.2f)
                    RenderingManager.Instance.SetFogValue(FogHandler.Intencity * amount / 0.2f);
                else if (amount >= 0.8f)
                    RenderingManager.Instance.SetFogValue(FogHandler.Intencity * (1f - amount) * 5f);
                else
                    RenderingManager.Instance.SetFogValue(FogHandler.Intencity);

            }
            else
            {
                if (FogHandler.StartTime <= EnvironmentManager.Instance.GetRoomTime)
                {
                    FogHandler.IsStart = true;
                    RenderingManager.Instance.StartFog();
                }
            }
        }
    }
    void OnProcessFog()
    {

    }
    public void Update()
    {
        OnUpdateWeatherHandler();
        OnUpdateFog();
    }
}
