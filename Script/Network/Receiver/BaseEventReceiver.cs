using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEventReceiver : Network.BaseEventReceiver
{
    protected bool IsServer => !Runner.IsSinglePlayer && Runner.IsServer;
    public Player GetPlayer()
    {
        return new Player()
        {
            Guid = GetString(),
            PlayerName = GetString(),
            PlayerID = GetInt32(),
            AllyNumber = GetInt32(),
            PlayerCharacterData = GetPlayerCharacterData()
        };
    }
    public PlayerCharacterData GetPlayerCharacterData()
    {
        return new PlayerCharacterData()
        {
            Name = GetString(),
            Index = GetInt32()
        };
    }
    public WeatherHandler GetWeatherHandler()
    {
        bool isNull = GetBoolean();
        if (isNull)
            return WeatherHandler.Null;

        return new WeatherHandler()
        {
            WeahterType = (eWeatherType)GetInt16(),
            StartTime = GetFloat(),
            DurationTime = GetFloat(),
            Intencity = GetFloat(),
        };
    }
    public MessageStructure GetMessageStructure()
    {
        return new MessageStructure((eMessageType)GetUInt16(), GetInt32(), GetString(), GetString());
    }
    public Item GetItem()
    {
        int index = GetInt32();
        int count = GetInt32();
        return Item.NewItem(index, count);
    }
    public Inventory GetInventory()
    {
        Inventory newInven = new Inventory(GetUInt64(), GetInt32());
        int count = GetInt32();
        for(int i = 0; i < count; ++i)
            newInven.PushItemSlot(GetInt32(), GetItem());

        return newInven;
    }
    public AttackProperty GetAttackProperty()
    {
        return new AttackProperty((eDamageType)GetInt16(), GetUInt64(), GetUInt64(), GetBoolean(), GetFloat());
    }
    //public AttackProperty GetAttackProperty()
    //{
    //    return new AttackProperty()
    //    {
    //        DamageType = (eDamageType)GetInt32(),
    //        Caster = ActorManager.Instance.GetActor(_runner, GetUInt32()),
    //        Target = ActorManager.Instance.GetActor(_runner, GetUInt32()),
    //        Damage = GetFloat(),
    //    };
    //}
    //public MessageStructure GetMessageStructure()
    //{
    //    return new MessageStructure((eMessageType)GetUInt16(), GetInt32(), GetString(), GetString());
    //}
    //public CharacterProperty GetCharacterProperty()
    //{
    //    return new CharacterProperty()
    //    {
    //        Index = GetInt32(),
    //         StageIndex = GetInt32(),
    //         Position = GetVector2(),
    //    };
    //}
    //public UnityEditor.U2D.Animation.CharacterData GetCharacterData()
    //{
    //    return new UnityEditor.U2D.Animation.CharacterData(GetCharacterSkill());
    //}
    //public CharacterSkill GetCharacterSkill()
    //{
    //    Dictionary<long, long> characterSkillDic;
    //    int count = GetInt32();
    //    if(count > 0)
    //    {
    //        characterSkillDic = new Dictionary<long, long>(count);
    //        for (int i = 0; i < count; ++i)
    //            characterSkillDic.Add(GetInt64(), GetInt64());

    //        return new CharacterSkill(characterSkillDic);
    //    }
    //    return new CharacterSkill();
    //}
}
