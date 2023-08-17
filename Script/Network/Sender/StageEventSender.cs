using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public enum eStageEventCode
{
    RequestEnvironment,
    NotifyEnvironment,
    NotifyStartWeather,
    NotifyEndWeather,
    NotifyStartFog,
    RequestOverrideTile,
    NotifyOverrideTile,
    NotifyRollbackTile,
}
public class StageEventSender : BaseEventSender
{
    public void RequestEnvironment()
    {
        GeneratePacketOption(eStageEventCode.RequestEnvironment, 0, ReceiverOption.Host);
    }
    public void NotifyEnvironment(int playerID)
    {
        GeneratePacketOption(eStageEventCode.NotifyEnvironment, EnvironmentManager.Instance.GetByteSize, playerID);
        CopyBytes(EnvironmentManager.Instance.GetRoomTime);
        EnvironmentManager.Instance.WeatherController.CurrentWeahterHandler.EnqueueByte();
        if (EnvironmentManager.Instance.WeatherController.FogHandler != null)
            EnvironmentManager.Instance.WeatherController.FogHandler.EnqueueByte();
        else
            CopyBytes(BooleanSize);
    }
    public void NotifyStartWeather(WeatherHandler weatherHandler)
    {
        GeneratePacketOption(eStageEventCode.NotifyStartWeather, weatherHandler.GetByteSize, ReceiverOption.Other);
        weatherHandler.EnqueueByte();
    }
    public void NotifyEndWeahter()
    {
        GeneratePacketOption(eStageEventCode.NotifyEndWeather, 0, ReceiverOption.Other);
    }
    public void NotifyStartFog(FogHandler fogHandler)
    {
        GeneratePacketOption(eStageEventCode.NotifyStartFog, fogHandler.GetByteSize, ReceiverOption.Other);
        fogHandler.EnqueueByte();
    }
    public void RequestOverrideTile(Vector2Int worldPosition, eTileType overrideType)
    {
        int capacity = Vector2IntSize + IntSize;
        GeneratePacketOption(eStageEventCode.RequestOverrideTile, capacity, ReceiverOption.Host);
        CopyBytes(worldPosition);
        CopyBytes((int)overrideType);
    }
    public void NotifyOverrideTile(Vector2Int worldPosition, int overrideKey)
    {
        int capacity = Vector2IntSize + IntSize;
        GeneratePacketOption(eStageEventCode.NotifyOverrideTile, capacity, ReceiverOption.Other);
        CopyBytes(worldPosition);
        CopyBytes(overrideKey);
    }
    public void NotifyRollbackTile(Vector2Int worldPosition)
    {
        int capacity = Vector2IntSize;
        GeneratePacketOption(eStageEventCode.NotifyRollbackTile, capacity, ReceiverOption.Other);
        CopyBytes(worldPosition);
    }
}
