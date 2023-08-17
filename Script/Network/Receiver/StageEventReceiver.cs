using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public class StageEventReceiver : BaseEventReceiver
{
    public StageEventReceiver()
    {
        MappingReceiveEvent(eStageEventCode.RequestEnvironment, OnReplyEnvironment);
        MappingReceiveEvent(eStageEventCode.NotifyEnvironment, OnNotifyEnvironment);
        MappingReceiveEvent(eStageEventCode.NotifyStartWeather, OnNotifyStartWeather);
        MappingReceiveEvent(eStageEventCode.NotifyEndWeather, OnNotifyEndWeather);
        MappingReceiveEvent(eStageEventCode.NotifyStartFog, OnNotifyStartFog);
        MappingReceiveEvent(eStageEventCode.RequestOverrideTile, OnReplyOverrideTile);
        MappingReceiveEvent(eStageEventCode.NotifyOverrideTile, OnNotifyOverrideTile);
        MappingReceiveEvent(eStageEventCode.NotifyRollbackTile, OnNotifyRollbackTile);
    }
    protected bool OnReplyEnvironment(int playerID)
    {
        NetworkManager.Instance.StageEventSender.NotifyEnvironment(playerID);
        DebugUtility.Log($"OnReplyEnvironment - PlayerID: {playerID}");
        return true;
    }
    protected bool OnNotifyEnvironment(int playerID)
    {
        float time = GetFloat();
        WeatherHandler weahterHandler = GetWeatherHandler();
        EnvironmentManager.Instance.SetRuntimeParamerter(time);
        EnvironmentManager.Instance.WeatherController.SetRuntimeParamerter(weahterHandler);
        DebugUtility.Log($"OnNotifyEnvironment - Time: {time}");
        return true;
    }
    protected bool OnNotifyStartWeather(int playerID)
    {
        WeatherHandler weahterHandler = GetWeatherHandler();
        EnvironmentManager.Instance.WeatherController.StartWeather(weahterHandler);
        DebugUtility.Log($"OnNotifyStartWeather - WeatherType: {weahterHandler.WeahterType}");
        return true;
    }
    protected bool OnNotifyEndWeather(int playerID)
    {
        EnvironmentManager.Instance.WeatherController.EndWeather();
        DebugUtility.Log($"OnNotifyEndWeather");
        return true;
    }
    protected bool OnNotifyStartFog(int playerID)
    {
        float startTime = GetFloat();
        float elapsedTime = GetFloat();
        float durationTime = GetFloat();
        float intencity = GetFloat();
        EnvironmentManager.Instance.WeatherController.SetFogParameter(startTime, elapsedTime, durationTime, intencity);
        return true;
    }
    protected bool OnReplyOverrideTile(int playerID)
    {
        Vector2Int worldPosition = GetVector2Int(); 
        eTileType overrideType = (eTileType)GetInt32();
        if(overrideType != eTileType.Default)
        {
            int overrideKey = WorldManager.Instance.TileChunkSystem.SetOverrideTile(worldPosition, overrideType);
            if (overrideKey > 0)
                NetworkManager.Instance.StageEventSender.NotifyOverrideTile(worldPosition, overrideKey);
        }
        else
        {
            if(WorldManager.Instance.TileChunkSystem.SetDefaultTile(worldPosition))
                NetworkManager.Instance.StageEventSender.NotifyRollbackTile(worldPosition);
        }
        DebugUtility.Log($"OnReplyOverrideTile - WorldPosition: {worldPosition} TileType: {overrideType}");
        return true;
    }
    protected bool OnNotifyRollbackTile(int playerID)
    {
        Vector2Int worldPosition = GetVector2Int();
        WorldManager.Instance.TileChunkSystem.SetDefaultTile(worldPosition);
        DebugUtility.Log($"OnNotifyRollbackTile - WorldPosition: {worldPosition}");
        return true;
    }
    protected bool OnNotifyOverrideTile(int playerID)
    {
        Vector2Int worldPosition = GetVector2Int();
        int overrideKey = GetInt32();
        WorldManager.Instance.TileChunkSystem.SetOverrideTile(worldPosition, overrideKey);
        DebugUtility.Log($"OnNotifyOverrideTile - WorldPosition: {worldPosition} OverrideKey: {overrideKey}");

        return true;
    }
}
