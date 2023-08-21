using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class MonsterChunkSystem
{
    public List<IActor> ActivatorList  { get; private set; }
    List<SpawnHandler> _spawnHandlerList;
    int _frameRate;
    CancellationTokenSource _cancelToken;
    public MonsterChunkSystem(int capacity)
    {
        _frameRate = Application.targetFrameRate;
        _spawnHandlerList = new List<SpawnHandler>(capacity);
        ActivatorList = new List<IActor>(1);
    }
    public void PushHandler(TextAsset textAsset)
    {
        SpawnHandler spawnHandler = Newtonsoft.Json.JsonConvert.DeserializeObject<SpawnHandler>(textAsset.text, ClientConst.SerializingSetting);
        spawnHandler.Initialize();
        _spawnHandlerList.Add(spawnHandler);
    }
    public void StartMonsterSpawnTask()
    {
        _cancelToken = new CancellationTokenSource();
        UniTask.RunOnThreadPool(OnPorcessMonsterSpawnerAsync, true, _cancelToken.Token);
    }
    async UniTaskVoid OnPorcessMonsterSpawnerAsync()
    {
        while (true)
        {
            await UniTask.DelayFrame(_frameRate);
            float monsterSpawnRange = RuntimePreference.Preference.MonsterSpawnRange * RuntimePreference.Preference.MonsterSpawnRange;
            for (int i = 0; i < _spawnHandlerList.Count; ++i)
            {
                SpawnHandler spawnHandler = _spawnHandlerList[i];
                spawnHandler.Update(1f);
                for (int j = 0; j < ActivatorList.Count; ++j)
                {
                    if ((spawnHandler.Offset - ActivatorList[j].Position).sqrMagnitude <= monsterSpawnRange)
                        goto UpdateSpawn;
                }
                continue;

                UpdateSpawn:
                {
                    while (spawnHandler.SpawnerQueue.Count > 0)
                    {
                        SpawnStructure spawnStructure = _spawnHandlerList[i].SpawnerQueue.Dequeue();
                        // await UniTask.SwitchToMainThread(_cancelToken.Token);
                        var waitSpawn = await ActorManager.Instance.SpawnMonster(spawnStructure);
                        waitSpawn.OnDestroyCallback += () => spawnHandler.DecreaseMonsterCount(spawnStructure.AreaNumber);
                    }
                }
            }
        }
    }
}
