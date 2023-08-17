using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ResourceManager : TSingletonMono<ResourceManager>
{
    Dictionary<int, Sprite> _spriteResourceDictionary;
    Dictionary<int, TileBase> _tileResourceDictionary;
    public int AddSpriteHash(string path)
    {
        if (string.IsNullOrEmpty(path))
            return 0;

        int hashCode = path.GetHashCode();
        _spriteResourceDictionary.Add(hashCode, Resources.Load<Sprite>(path));
        return hashCode;
    }
    public int AddTileHash(string path)
    {
        if (string.IsNullOrEmpty(path))
            return 0;

        int hashCode = path.GetHashCode();
        TileBase tile = Resources.Load<TileBase>(path);
        _tileResourceDictionary.Add(hashCode, tile);
        return hashCode;
    }
    /// <returns>某教等 府家胶</returns>
    public Sprite GetSprite(int hash) => _spriteResourceDictionary[hash];
    public TileBase GetTile(int hash) => _tileResourceDictionary[hash];
    protected override void OnInitialize()
    {
        _spriteResourceDictionary = new Dictionary<int, Sprite>();
        _tileResourceDictionary = new Dictionary<int, TileBase>();
    }
}
