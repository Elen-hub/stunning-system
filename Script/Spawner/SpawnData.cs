using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpawnData
{
    public int AreaNumber;
    public Rect SpawnRect = new Rect() {
        position = Vector2.zero,
        size = new Vector2(1f, 1f),
    };

    public int MonsterIndex;
    public float SpawnTick;
    public int SpawnCount;
    public int MaxCount;
}
