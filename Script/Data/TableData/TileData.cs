using System.Collections.Generic;

namespace Data
{
    public class TileData
    {
        public readonly int Index;
        public readonly eTileType TileType;
        public readonly Dictionary<eTileType, int> OverrideTileData;
        readonly int _tileHash;
        public UnityEngine.Tilemaps.TileBase Tile => ResourceManager.Instance.GetTile(_tileHash);
        public TileData(int index, Dictionary<string, string> dataPair)
        {
            Index = index;
            _tileHash = ResourceManager.Instance.AddTileHash(dataPair["ResourcePath"]); 
            TileType = EnumConverter.TileTypeConvert[dataPair["TileType"]];
            OverrideTileData = new Dictionary<eTileType, int>();
            int overrideFarmTileIndex = int.Parse(dataPair["OverrideFarm"]);
            if (overrideFarmTileIndex > 0)
                OverrideTileData.Add(eTileType.Farm, overrideFarmTileIndex);
        }
    }
}