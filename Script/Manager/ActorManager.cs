using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Newtonsoft.Json;
using Cysharp.Threading.Tasks;

public class ActorManager : TSingletonMono<ActorManager>
{
    ulong _currentWorldID;
    ActorFactory _actorFactory;
    SpawnHandler _spawnHandler;
    Queue<AttackProperty> _attackPropertyQueue = new Queue<AttackProperty>(30);
    Dictionary<ulong, IActor> _actorDic = new Dictionary<ulong, IActor>();
    public Dictionary<ulong, IActor> GetSpawnedActors => _actorDic;
    public void EnqueueAttackProperty(AttackProperty property) => _attackPropertyQueue.Enqueue(property);
#if UNITY_SERVER
    JsonSerializerSettings _serializeSetting = new JsonSerializerSettings()
    {
        TypeNameHandling = TypeNameHandling.Objects,
        Formatting = Formatting.Indented,
        ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
    };
    public void StartSpawner()
    {
        _spawnHandler = new SpawnHandler();
    }
#endif
    public IActor GetActor(ulong worldID)
    {
        if (_actorDic.ContainsKey(worldID))
            return _actorDic[worldID];

        return null;
    }
    public T GetActor<T>(ulong worldID) where T : class, IActor
    {
        if (_actorDic.ContainsKey(worldID))
            return _actorDic[worldID] as T;

        return null;
    }
    public async UniTask<Character> SpawnCharacter(int index, Vector3 position, int allyNumber)
    {
        Character character = await _actorFactory.SpawnCharacter(index, position, allyNumber);
        character.WorldID = ++_currentWorldID;
        return character;
    }
    public async UniTask<Monster> SpawnMonster(int index, Vector3 position, int allyNumber)
    {
        Monster character = await _actorFactory.SpawnMonster(index, position, allyNumber);
        character.WorldID = ++_currentWorldID;
        return character;
    }
    public async UniTask<Monster> SpawnMonster(SpawnStructure spawnStructure) => await SpawnMonster(spawnStructure.Index, spawnStructure.SpawnPosition, 0);
    public async UniTask<StaticActor> SpawnServerObject(int index, Vector2Int position, string ownerGUID)
    {
        StaticActor staticActor = await _actorFactory.InstanceObjectAsync(index);
        staticActor.transform.position = new Vector3Int(position.x, position.y, 0);
        staticActor.OwnerGUID = ownerGUID;
        staticActor.OnSpawnServer(++_currentWorldID);
        return staticActor;
    }
    public StaticActor SpawnClientObject(int index, BaseEventReceiver eventReceiver)
    {
        StaticActor staticActor = _actorFactory.InstanceObject(index);
        staticActor.OnSpawnClient(eventReceiver);
        return staticActor;
    }
    public async UniTask<StaticActor> SpawnLoadObject(int index, SaveController saveController)
    {
        StaticActor staticActor = await _actorFactory.InstanceObjectAsync(index);
        ulong worldID = saveController.GetUInt64();
        if (worldID == 0)
            worldID = ++_currentWorldID;
        staticActor.OnSpawnLoad(worldID, saveController);

        return staticActor;
    }
    public async UniTask<StaticActor> InstallStaticActor(int index) => await _actorFactory.InstallStaticActor(index);
    //public StaticNPC SpawnStaticNPC(NetworkRunner runner, int index, Vector3 position) => _actorFactoryDictionary[runner].SpawnStaticNPC(index, 0, position);
    //public StaticNPC SpawnStaticNPC(NetworkRunner runner, int index, uint worldID, Vector3 position) => _actorFactoryDictionary[runner].SpawnStaticNPC(index, worldID, position);
    public void RegistWorld(IActor actor) => _actorDic.Add(actor.WorldID, actor);
    public void RemoveWorld(ulong worldID) => _actorDic.Remove(worldID);
    public void RegistObjectMemoryPool(StaticActor actor) => _actorFactory.RegistObjectMemoryPool(actor);
    // public void RegistMonster(NetworkRunner runner, Monster monster) => _spawnHandlerDictionary[runner].DecreaseMonsterCount(monster.AreaNumber);
    protected override void OnInitialize()
    {
        _actorFactory = new ActorFactory(transform);
        _actorDic = new Dictionary<ulong, IActor>();
    }
    public IActor FindActor(ActorSearchHandler tool)
    {
        foreach (var element in _actorDic)
        {
            IActor actor = element.Value;
            if (tool.CheckCondition(actor))
                return actor;
        }
        return null;
    }
    public void FindActor(int count, ActorSearchHandler tool)
    {
        foreach (var element in _actorDic)
        {
            IActor actor = element.Value;
            if (tool.CheckCondition(actor))
            {
                tool.SetActorEnqueue = actor;
                --count;
                if (count <= 0)
                    return;
            }
        }
    }
    public void Destroy()
    {
        _actorFactory.Destroy();
#if UNITY_SERVER
        foreach (var actor in _actorDic)
            actor.Value.Destroy();
#endif
    }
    void OnUpdateAttackHandler()
    {
        while (_attackPropertyQueue.Count > 0)
        {
            AttackProperty attackProperty = _attackPropertyQueue.Dequeue();
            IActor target = GetActor(attackProperty.TargetID);
            if (target != null)
            {
                if (target.IsAlive)
                {
                    target.Hit(attackProperty);
                }
            }
        }
    }
#if UNITY_SERVER
    void OnUpdateSpawnHandler()
    {
        if (_spawnHandler == null) return;
        _spawnHandler.Update(Time.deltaTime);
        while (_spawnHandler.SpawnerQueue.Count > 0)
        {
            SpawnStructure spawnStructure = _spawnHandler.SpawnerQueue.Dequeue();
            SpawnMonster(spawnStructure.Index, spawnStructure.SpawnPosition, 0);
        }
    }
    private void Update()
    {
        OnUpdateAttackHandler();
        OnUpdateSpawnHandler();
    }
#endif
}
