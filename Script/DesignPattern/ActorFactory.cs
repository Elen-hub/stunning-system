using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Cysharp.Threading.Tasks;

public class ActorFactory
{
    NetworkRunner _runner => NetworkManager.Instance.Runner;
    Dictionary<int, ObjectMemoryPool<StaticActor>> _staticActorObjectMemoryPool;
    Transform _root;
    public ActorFactory(Transform root)
    {
        _root = root;
        _staticActorObjectMemoryPool = new Dictionary<int, ObjectMemoryPool<StaticActor>>();
    }
    T GetCachedResource<T>(string key) where T : Object
    {
        var resource = Resources.Load<T>(key);
        return resource as T;
    }
    async UniTask<T> GetCachedResourceAsync<T>(string key) where T : Object
    {
        var resource = await Resources.LoadAsync<T>(key);
        return resource as T;
    }
    public async UniTask<Character> SpawnCharacter(int index, Vector3 position, int allyNumber)
    {
        Debug.Log(DataManager.Instance.CharacterTable[index]);
        var resourceAsync = await GetCachedResourceAsync<NetworkObject>(DataManager.Instance.CharacterTable[index].ResourcePath);
        NetworkObject networkObject = _runner.Spawn(resourceAsync, position, Quaternion.identity);
        Character actor = networkObject.GetComponent<Character>();
        actor.Index = index;
        actor.AllyNumber = allyNumber;
        return actor;
    }
    public async UniTask<Monster> SpawnMonster(int index, Vector3 position, int allyNumber)
    {
        var resourceAsync = await GetCachedResourceAsync<NetworkObject>(DataManager.Instance.MonsterTable[index].ResourcePath);
        NetworkObject networkObject = _runner.Spawn(resourceAsync, position, Quaternion.identity);
        Monster actor = networkObject.GetComponent<Monster>();
        actor.Index = index;
        return actor;
    }
    StaticActor InstantiateActor(int index)
    {
        IActor instance = null;
        StaticActor staticActor = null;
        if (_staticActorObjectMemoryPool.ContainsKey(index))
        {
            instance = _staticActorObjectMemoryPool[index].GetItem();
            staticActor = instance as StaticActor;
        }
        else
            _staticActorObjectMemoryPool.Add(index, new ObjectMemoryPool<StaticActor>(10));

        if (instance == null)
        {
            var resourceAsync = GetCachedResource<StaticActor>(DataManager.Instance.ObjectTable[index].ResourcePath);
            instance = Object.Instantiate(resourceAsync, _root);
            staticActor = instance as StaticActor;
            staticActor.Initialize(index);
        }
        staticActor.Initialize(index);
        return staticActor;
    }
    async UniTask<StaticActor> InstantiateActorAsync(int index)
    {
        IActor instance = null;
        StaticActor staticActor = null;
        if (_staticActorObjectMemoryPool.ContainsKey(index))
        {
            instance = _staticActorObjectMemoryPool[index].GetItem();
            staticActor = instance as StaticActor;
        }
        else
            _staticActorObjectMemoryPool.Add(index, new ObjectMemoryPool<StaticActor>(10));

        if (instance == null)
        {
            var resourceAsync = await GetCachedResourceAsync<StaticActor>(DataManager.Instance.ObjectTable[index].ResourcePath);
            instance = Object.Instantiate(resourceAsync, _root);
            staticActor = instance as StaticActor;
            staticActor.Initialize(index);
        }
        staticActor.Initialize(index);
        return staticActor;
    }
    public StaticActor InstanceObject(int index)
    {
        StaticActor staticActor = InstantiateActor(index);
        staticActor.gameObject.SetActive(true);
        return staticActor;
    }
    public async UniTask<StaticActor> InstanceObjectAsync(int index)
    {
        StaticActor staticActor = await InstantiateActorAsync(index);
        staticActor.gameObject.SetActive(true);
        return staticActor;
    }
    public async UniTask<StaticActor> InstallStaticActor(int index)
    {
        StaticActor staticActor = await InstantiateActorAsync(index);
        staticActor.gameObject.SetActive(true);
        return staticActor;
    }
    public void RegistObjectMemoryPool(StaticActor actor)
    {
        _staticActorObjectMemoryPool[actor.Index].Register(actor);
    }
    public void Destroy()
    {

    }
}
