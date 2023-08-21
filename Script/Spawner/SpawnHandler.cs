using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Fusion;

public class SpawnHandler
{
    public Vector2Int Offset;
    public List<SpawnData> SpawnDataList = new List<SpawnData>();
    Queue<SpawnStructure> _spawnQueue = new Queue<SpawnStructure>();
    public Queue<SpawnStructure> SpawnerQueue => _spawnQueue;
    Dictionary<int, int> _currentSpawnedDic;
    Dictionary<int, float> _currentTickDic;

    public void Initialize()
    {
        _currentSpawnedDic = new Dictionary<int, int>(SpawnDataList.Count);
        _currentTickDic = new Dictionary<int, float>(SpawnDataList.Count);
        for (int i = 0; i < SpawnDataList.Count; ++i)
        {
            _currentSpawnedDic.Add(SpawnDataList[i].AreaNumber, 0);
            _currentTickDic.Add(SpawnDataList[i].AreaNumber, SpawnDataList[i].SpawnTick);
        }
    }
    public void DecreaseMonsterCount(int areaNumber) => _currentSpawnedDic[areaNumber] -= 1;
    bool IsRequireSpawn(SpawnData data) => data.MaxCount > _currentSpawnedDic[data.AreaNumber];
    void RequestPushSpawnQueue(SpawnData data)
    {
        int spawnCount = data.SpawnCount;
        if (spawnCount + _currentSpawnedDic[data.AreaNumber] > data.MaxCount)
            spawnCount = data.MaxCount - _currentSpawnedDic[data.AreaNumber];

        _currentSpawnedDic[data.AreaNumber] += spawnCount;
        _spawnQueue.Enqueue(new SpawnStructure()
        {
            AreaNumber = data.AreaNumber,
            SpawnActorType = eActorType.Monster,
            Index = data.MonsterIndex,
            SpawnPosition = Offset + data.SpawnRect.center + new Vector2
            {
                x = Random.Range(-data.SpawnRect.size.x, data.SpawnRect.size.x),
                y = Random.Range(-data.SpawnRect.size.y, data.SpawnRect.size.y),
            }
        });
    }
    public void Update(float tickTime)
    {
        for(int i = 0; i< SpawnDataList.Count; ++i)
        {
            int areaNumber = SpawnDataList[i].AreaNumber;
            _currentTickDic[areaNumber] -= tickTime;
            if(_currentTickDic[areaNumber] < 0f)
            {
                if (IsRequireSpawn(SpawnDataList[i]))
                {
                    _currentTickDic[areaNumber] += SpawnDataList[areaNumber].SpawnTick;
                    RequestPushSpawnQueue(SpawnDataList[i]);
                }
            }
        }
    }
}
