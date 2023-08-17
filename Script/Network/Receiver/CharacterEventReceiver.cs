using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;
public class CharacterEventReceiver : BaseEventReceiver
{
    public CharacterEventReceiver()
    {
        MappingReceiveEvent(eCharacterEventCode.RequestSpawnCharacter, OnReplySpawnCharacter);
    }
    protected bool OnReplySpawnCharacter(int playerID)
    {
        PlayerCharacterData characterData = GetPlayerCharacterData();
        Player player = PlayerManager.Instance.GetPlayer(playerID);
        if (player == null) return false;
        player.PlayerCharacterData = characterData;
        player.SpawnCharacter();
        DebugUtility.Log($"OnReplySpawnCharacter - CharacterIndex: {characterData.Index}");

        return true;
    }
}
