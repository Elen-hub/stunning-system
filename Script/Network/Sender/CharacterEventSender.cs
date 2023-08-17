using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public enum eCharacterEventCode
{
    RequestSpawnCharacter,
    NotifyLevelup,
}
public class CharacterEventSender : BaseEventSender
{
    public void RequestSpawnCharacter(PlayerCharacterData data)
    {
        int capacity = data.GetByteSize;
        GeneratePacketOption(eCharacterEventCode.RequestSpawnCharacter, capacity, ReceiverOption.Host);
        data.EnqueueByte();
    }
    public void NotifyLevelup(uint worldID, int levelUpCount)
    {
        int capacity = UIntSize + IntSize;
        GeneratePacketOption(eCharacterEventCode.NotifyLevelup, capacity, ReceiverOption.Other);
        CopyBytes(worldID);
        CopyBytes(levelUpCount);
    }
}
