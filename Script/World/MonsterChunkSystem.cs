using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
public class MonsterChunkSystem
{
    public List<IActor> ActivatorList = new List<IActor>();
    List<SpawnHandler> _spawnHandlerList;
    public MonsterChunkSystem(int capacity)
    {
        _spawnHandlerList = new List<SpawnHandler>(capacity);
    }
    public void PushHandler(TextAsset textAsset)
    {
        SpawnHandler spawnHandler = Newtonsoft.Json.JsonConvert.DeserializeObject<SpawnHandler>(textAsset.text, ClientConst.SerializingSetting);
        spawnHandler.Initialize();
        _spawnHandlerList.Add(spawnHandler);
    }
    public void UpdateServer()
    {
        for(int i = 0; i < _spawnHandlerList.Count; ++i)
        {
            SpawnHandler spawnHandler = _spawnHandlerList[i];
            spawnHandler.Update(TimeManager.DeltaTime);
            while (spawnHandler.SpawnerQueue.Count > 0)
            {
                SpawnStructure spawnStructure = _spawnHandlerList[i].SpawnerQueue.Dequeue();
                ActorManager.Instance.SpawnMonster(spawnStructure).ContinueWith(result =>
                {
                    result.OnDestroyCallback += delegate {
                        spawnHandler.DecreaseMonsterCount(spawnStructure.AreaNumber);
                    };
                });
            }
        }
    }
}
