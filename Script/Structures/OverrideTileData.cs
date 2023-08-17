using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideTileData
{
    public bool IsModify = false;
    public Vector2Int TilePosition;
    public int TileKey;
    public Data.TileData GetTileData => DataManager.Instance.TileTable[TileKey];
    public int OverrideTileKey;
    public Data.TileData GetOverrideTileData => DataManager.Instance.TileTable[OverrideTileKey];
}
