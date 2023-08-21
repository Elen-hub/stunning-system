using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileChunkSystem
{
    Transform _grid;
    Tilemap _tilemap;
    Tilemap _waterTilemap;
    Tilemap _overrideTilemap;
    AstarPath _astarPath;
    Dictionary<Vector2Int, OverrideTileData> _overrideTileData;
    public Dictionary<Vector2Int, OverrideTileData> GetOverrideTileDictionary => _overrideTileData;
    public TileChunkSystem()
    {
        _grid = Object.Instantiate(Resources.Load<Transform>("Tiles/Grid"));
        _tilemap = _grid.Find("MainTilemap").GetComponent<Tilemap>();
        _waterTilemap = _grid.Find("WaterTilemap").GetComponent<Tilemap>();
        _overrideTilemap = _grid.Find("OverrideTilemap").GetComponent<Tilemap>();
        _astarPath = _grid.GetComponent<AstarPath>();
        _astarPath.scanOnStartup = false;
        _overrideTileData = new Dictionary<Vector2Int, OverrideTileData>(500);
        Object.DontDestroyOnLoad(_grid);
    }
    public void SetTileData(Vector2Int pivot, TextAsset textAsset, bool isWater)
    {
        SaveController saveController = new SaveController(textAsset.bytes);
        eTileCompressionType compressionType = (eTileCompressionType)saveController.GetByte();
        if(compressionType != eTileCompressionType.None)
        {
            CompressionUtility.Decoder decoder = null;
            switch (compressionType)
            {
                case eTileCompressionType.RLE:
                    decoder = new CompressionUtility.RLEDecoder(saveController);
                    break;
            }
            for (int x = -25; x < 25; ++x) {
                for (int y = -25; y < 25; ++y) {
                    Vector2Int tilePos = pivot + new Vector2Int(x, y);
                    int tileKey = decoder.GetValue;
                    OnSetTileData(tilePos, tileKey, isWater);
                }
            }
        }
        else
        {
            for (int x = -25; x < 25; ++x) {
                for (int y = -25; y < 25; ++y) {
                    Vector2Int tilePos = pivot + new Vector2Int(x, y);
                    int tileKey = saveController.GetInt32();
                    OnSetTileData(tilePos, tileKey, isWater);
                }
            }
        }
    }
    void OnSetTileData(Vector2Int tilePos, int tileKey, bool isWater)
    {
        if (tileKey == 0)
            return;

        if (_overrideTileData.ContainsKey(tilePos))
        {
            if (isWater)
            {
                _overrideTileData[tilePos].TileKey = tileKey;
                _waterTilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), DataManager.Instance.TileTable[tileKey].Tile);
            }
            else 
                _tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), DataManager.Instance.TileTable[tileKey].Tile);
        }
        else
        {
            OverrideTileData tileData = new OverrideTileData()
            {
                TilePosition = tilePos,
                TileKey = tileKey,
            };
            _overrideTileData.Add(tilePos, tileData);
            if(isWater) _waterTilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), DataManager.Instance.TileTable[tileKey].Tile);
            else _tilemap.SetTile(new Vector3Int(tilePos.x, tilePos.y, 0), DataManager.Instance.TileTable[tileKey].Tile);
        }
    }
    public void InstanceTile(bool isOverride, OverrideTileData tileData)
    {
        if (isOverride)
        {
            if (tileData.OverrideTileKey != 0)
                _overrideTilemap.SetTile(new Vector3Int(tileData.TilePosition.x, tileData.TilePosition.y, 0), DataManager.Instance.TileTable[tileData.OverrideTileKey].Tile);
        }
        else
        {
            if (tileData.TileKey != 0)
                _tilemap.SetTile(new Vector3Int(tileData.TilePosition.x, tileData.TilePosition.y, 0), DataManager.Instance.TileTable[tileData.TileKey].Tile);
        }
    }
    public int SetOverrideTile(Vector2Int worldPosition, eTileType overrideType)
    {
        if (!_overrideTileData.ContainsKey(worldPosition))
            return 0;

        if (!_overrideTileData[worldPosition].GetTileData.OverrideTileData.ContainsKey(overrideType))
            return 0;

        if(_overrideTileData[worldPosition].OverrideTileKey == _overrideTileData[worldPosition].GetTileData.OverrideTileData[overrideType])
            return 0;

        _overrideTileData[worldPosition].OverrideTileKey = _overrideTileData[worldPosition].GetTileData.OverrideTileData[overrideType];
        InstanceTile(true, _overrideTileData[worldPosition]);
        return _overrideTileData[worldPosition].OverrideTileKey;
    }
    public bool SetDefaultTile(Vector2Int worldPosition)
    {
        if (!_overrideTileData.ContainsKey(worldPosition))
            return false;

        if (_overrideTileData[worldPosition].OverrideTileKey == 0)
            return false;

        _overrideTileData[worldPosition].OverrideTileKey = 0;
        InstanceTile(false, _overrideTileData[worldPosition]);
        return true;
    }
    public void SetOverrideTile(Vector2Int worldPosition, int overrideKey)
    {
        if (!_overrideTileData.ContainsKey(worldPosition))
            return;

        _overrideTileData[worldPosition].OverrideTileKey = overrideKey;
        InstanceTile(true, _overrideTileData[worldPosition]);
    }
    public void ModifyTileCollider(Vector2Int worldPosition, bool enabled)
    {
        if (!_overrideTileData.ContainsKey(worldPosition))
            return;

        _overrideTilemap.SetColliderType((Vector3Int)worldPosition, enabled ? Tile.ColliderType.Sprite : Tile.ColliderType.None);
    }
    public void BuildNavmesh()
    {
        _astarPath.Scan();
        _waterTilemap.gameObject.SetActive(true);
    }
}
