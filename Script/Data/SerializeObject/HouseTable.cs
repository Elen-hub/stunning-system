using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HouseTable", menuName = "ScriptableObjects/HouseTable", order = 1)]
public class HouseTable : ScriptableObject
{
    public List<GameObject> HouseIndexList;
}
