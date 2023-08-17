using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class WorldManager : TSingletonMono<WorldManager>
{
    public RoomConfiguration RoomConfiguration;
    // Dictionary<Vector2Int, ChunkData>
    public TileChunkSystem TileChunkSystem;
    public HouseChunkSystem HouseChunkSystem;
    public MonsterChunkSystem MonsterChunkSystem;
    public ObjectChunkSystem ObjectChunkSystem;
    public MinimapSystem MinimapSystem;
    public Sprite GetMinimapPiece(Vector2Int pos) => MinimapSystem.GetMinimapPiece(pos);
    int _processCount = 0;
    IActor _mainCharacter;
    protected override void OnInitialize()
    {
        RoomConfiguration = new RoomConfiguration();
    }
    public void SetMainCharacter(IActor mainCharacter)
    {
        if(mainCharacter != null)
        {
            _mainCharacter = mainCharacter;
            MinimapSystem.StartFogUpdater();
        }
        else
        {
            _mainCharacter = null;
        }
    }
    void OnGenerateWorld()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Database/SceneData/SceneBuildData");
        List<ChunkRootData> spawnArr = (List<ChunkRootData>)Newtonsoft.Json.JsonConvert.DeserializeObject(textAsset.text, typeof(List<ChunkRootData>));
#if UNITY_SERVER
        MonsterChunkSystem = new MonsterChunkSystem(spawnArr.Count);
        ObjectChunkSystem = new ObjectChunkSystem();
#else
        MinimapSystem = new MinimapSystem(spawnArr.Count);
#endif
        TileChunkSystem = new TileChunkSystem();
        HouseChunkSystem = new HouseChunkSystem(transform);
        _processCount = spawnArr.Count;
        for (int i = 0; i < spawnArr.Count; ++i)
            OnProcessChunkAsync(spawnArr[i]).Forget();

#if UNITY_SERVER
        IsCompleteLoad().ContinueWith(result => TileChunkSystem.BuildNavmesh());
#endif
    }
    async UniTask<bool> IsCompleteLoad()
    {
        while (_processCount != 0)
            await UniTask.Yield();

        return true;
    }
    async UniTaskVoid OnProcessChunkAsync(ChunkRootData chunkConfigure)
    {
        string path_HouseSpawnData = "/HouseSpawnData";
        string path_MinimapBlock = "/MinimapBlock";
        string path_TileSpawnData = "/TileSpawnData";
        string path_WaterTileSpawnData = "/WaterTileSpawnData";

#if UNITY_SERVER
        string path_MonsterSpawnData = "/MonsterSpawnData";
        string path_ObjectSpawnData = "/ObjectSpawnData";
        var monsterSpawnAssetRequest = await Resources.LoadAsync<TextAsset>(chunkConfigure.RootPath + path_MonsterSpawnData);
        if (monsterSpawnAssetRequest != null)
        {
            TextAsset textAsset = (TextAsset)monsterSpawnAssetRequest;
            MonsterChunkSystem.PushHandler(textAsset);
        }
        var objectSpawnAssetRequest = await Resources.LoadAsync<TextAsset>(chunkConfigure.RootPath + path_ObjectSpawnData);
        if (objectSpawnAssetRequest != null)
        {
            TextAsset textAsset = (TextAsset)objectSpawnAssetRequest;
            ObjectChunkSystem.SpawnObjectAsync(textAsset).Forget();
        }
#else
        var minimapAssetRequest = await Resources.LoadAsync<Sprite>(chunkConfigure.RootPath + path_MinimapBlock);
        if (minimapAssetRequest != null)
        {
            Sprite spriteAsset = (Sprite)minimapAssetRequest;
            MinimapSystem.AddMinimapPiece(chunkConfigure.Position, spriteAsset);
        }
#endif
        var houseAssetRequest = await Resources.LoadAsync<TextAsset>(chunkConfigure.RootPath + path_HouseSpawnData);
        if (houseAssetRequest != null)
        {
            TextAsset textAsset = (TextAsset)houseAssetRequest;
            HouseChunkSystem.SetHouseData(textAsset);
        }
        var waterTileAssetRequest = await Resources.LoadAsync<TextAsset>(chunkConfigure.RootPath + path_WaterTileSpawnData);
        if (waterTileAssetRequest != null)
        {
            TextAsset textAsset = (TextAsset)waterTileAssetRequest;
            TileChunkSystem.SetTileData(chunkConfigure.Position, textAsset, true);
        }
        var tileAssetRequest = await Resources.LoadAsync<TextAsset>(chunkConfigure.RootPath + path_TileSpawnData);
        if (tileAssetRequest != null)
        {
            TextAsset textAsset = (TextAsset)tileAssetRequest;
            TileChunkSystem.SetTileData(chunkConfigure.Position, textAsset, false);
        }
        --_processCount;
    }
//    IEnumerator IEGenerateWorld()
//    {
//        Object[] loadAllData = Resources.LoadAll("Database/SceneData");
//        for (int i = 0; i < loadAllData.Length; ++i)
//        {
//            TextAsset textAsset = loadAllData[i] as TextAsset;
//            switch (textAsset.name)
//            {
//#if UNITY_SERVER
//                case "ObjectSpawnData":
//                    ObjectChunkSystem.SpawnObject(textAsset).Forget();
//                    break;
//                case "MonsterSpawnData":
//                    MonsterChunkSystem.PushHandler(textAsset);
//                    break;
//#endif
//                case "TileSpawnData":
//                    TileChunkSystem.SetTileData(textAsset);
//                    break;
//                case "HouseSpawnData":
//                    HouseChunkSystem.SetHouseData(textAsset);
//                    break;
//            }
//            yield return null;
//        }
//        yield return null;

//        TileChunkSystem.BuildNavmeshAsync();
//    }
    public void GenerateFirstWorld()
    {
        OnGenerateWorld();
        // StartCoroutine(IEGenerateWorld());
    }
    private void Update()
    {
        if (!NetworkManager.Instance.Runner.IsRunning)
            return;

#if UNITY_SERVER
        MonsterChunkSystem.UpdateServer();
#else
        if (_mainCharacter != null)
            MinimapSystem.Update(_mainCharacter.Position);
#endif
    }
}
