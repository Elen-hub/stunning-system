using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEditor;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;

public class TileSaveEditor
{
    static string _spawnHandlerPath;
    string SceneName => SceneManager.GetActiveScene().name;
    public TileSaveEditor()
    {
        _spawnHandlerPath = Application.dataPath + "/Resources/Database/SceneData/";
        if (!System.IO.Directory.Exists(_spawnHandlerPath))
            System.IO.Directory.CreateDirectory(_spawnHandlerPath);
    }
    public void OnGUI()
    {
        if (GUILayout.Button("타일 저장"))
            SaveTile();
    }
    public void SaveTile()
    {
        Vector3 pivot = GameObject.Find("ScenePivot").transform.position;
        GameObject grid = GameObject.Find("Grid");
        Vector2Int gridPosition = new Vector2Int(Mathf.RoundToInt(pivot.x), Mathf.RoundToInt(pivot.y));
        BoundsInt bounds = new BoundsInt(
            new Vector3Int(-25, -25, 0),
            new Vector3Int(50, 50, 0));
        if (grid != null)
        {
            Tilemap tilemap = grid.transform.Find("MainTilemap").GetComponent<Tilemap>();
            if (tilemap != null)
            {
                List<int> tileList = GetTileList(tilemap, gridPosition, bounds);
                OnSaveData(tileList, "TileSpawnData.bytes");
            }
            Tilemap waterTilemap = grid.transform.Find("WaterTilemap").GetComponent<Tilemap>();
            if (waterTilemap != null)
            {
                List<int> tileList = GetTileList(waterTilemap, gridPosition, bounds);
                OnSaveData(tileList, "WaterTileSpawnData.bytes");
            }
            AssetDatabase.Refresh();
        }
    }
    List<int> GetTileList(Tilemap tilemap, Vector2Int gridOffset, BoundsInt bounds)
    {
        List<int> tileList = new List<int>(bounds.size.x * bounds.size.y);
        for (int x = bounds.position.x; x < bounds.position.x + bounds.size.x; ++x)
        {
            for (int y = bounds.position.y; y < bounds.position.y + bounds.size.y; ++y)
            {
                Vector3Int pos = new Vector3Int(x, y, 0);
                TileBase tile = tilemap.GetTile<TileBase>((Vector3Int)gridOffset + pos);
                if (tile != null) tileList.Add(DataManager.Instance.TileTable[tile.name].Index); 
                else tileList.Add(0);
            }
        }
        return tileList;
    }
    void OnSaveData(List<int> dataList, string name)
    {
        string scenePath = _spawnHandlerPath + SceneName;
        if (!System.IO.Directory.Exists(scenePath))
            System.IO.Directory.CreateDirectory(scenePath);

        if (dataList.Count == 0)
            return;

        List<int> outDataList;
        eTileCompressionType compressionType = CompressionUtility.CompressionTileData(dataList, out outDataList);
        Debug.Log("CompressionType: " + compressionType);
        string jsonPath = $"{scenePath}/{name}";
        System.IO.FileStream fs = new System.IO.FileStream(jsonPath, System.IO.FileMode.Create);
        SaveController saveController = new SaveController(sizeof(byte) + sizeof(int) * outDataList.Count);
        saveController.CopyBytes((byte)compressionType);
        for (int i = 0; i < outDataList.Count; ++i)
        {
            saveController.CopyBytes(outDataList[i]);
        }
        fs.Write(saveController.GetByteArray, 0, saveController.GetByteArray.Length);
        fs.Close();
        Debug.Log($"SaveTileData: {scenePath}");
    }
}