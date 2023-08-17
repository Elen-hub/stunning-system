using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITileSetable
{
    Transform transform { get; }
    List<Vector2Int> GetTileBlockCoordList { get; }
}
