using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
using Cysharp.Threading.Tasks;

public class Player : IPacket
{
    public string Guid = "GuidTest";
    public string PlayerName = "NameTest";
    public int PlayerID;
    public int AllyNumber = 1;
    public PlayerCharacterData PlayerCharacterData;
    public DynamicActor Character;
    public void SpawnCharacter()
    {
        Vector3 spawnPos = Vector3.zero;
        ActorManager.Instance.SpawnCharacter(PlayerCharacterData.Index, spawnPos, AllyNumber).ContinueWith(result =>
        {
            result.Name = PlayerCharacterData.Name;
            result.Object.AssignInputAuthority(PlayerID);
            WorldManager.Instance.SetActivator = result;
            Character = result;
        });
    }
    public void SetMainCharacter(DynamicActor character)
    {
        Character = character;
    }
    public void Destroy()
    {
        if(Character != null)
        {
#if UNITY_SERVER
            WorldManager.Instance.MonsterChunkSystem.ActivatorList.Remove(Character);
#endif
        }
    }
#region Network
    public int GetByteSize => ReliableHelper.StringSize(Guid.Length) + ReliableHelper.StringSize(PlayerName.Length) + ReliableHelper.IntSize + ReliableHelper.IntSize + PlayerCharacterData.GetByteSize;
    public void EnqueueByte()
    {
        BaseEventSender.CopyBytes(Guid);
        BaseEventSender.CopyBytes(PlayerName);
        BaseEventSender.CopyBytes(PlayerID);
        BaseEventSender.CopyBytes(AllyNumber);
        PlayerCharacterData.EnqueueByte();
    }
#endregion
}
