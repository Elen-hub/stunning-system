using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : ActorStat
{
    public MonsterStat(DynamicActor owner)
    {
        int referenceStatIndex = DataManager.Instance.MonsterTable[owner.Index].StatTableIndex;
        _statusDic = DataManager.Instance.StatTable[referenceStatIndex].GenerateStat();
    }
}
