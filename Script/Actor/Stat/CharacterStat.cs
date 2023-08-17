using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStat : ActorStat
{
    public CharacterStat(DynamicActor owner)
    {
        int referenceStatIndex = DataManager.Instance.CharacterTable[owner.Index].StatTableIndex;
        _statusDic = DataManager.Instance.StatTable[referenceStatIndex].GenerateStat();
    }
}
