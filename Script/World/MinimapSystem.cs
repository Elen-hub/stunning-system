using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class MinimapSystem
{
    Dictionary<Vector2Int, Sprite> _minimapDictionary;
    Dictionary<Object, MinimapPinStructure> _minimapPinDictionary;
    MinimapFog _minimapFog;
    public Dictionary<Object, MinimapPinStructure> GetMinimapPinDictionary => _minimapPinDictionary;
    public Sprite GetMinimapPiece(Vector2Int pos) => _minimapDictionary.ContainsKey(pos) ? _minimapDictionary[pos] : null;
    public Sprite GetMinimapFogSprite(Vector2Int pos) => _minimapFog.GetSprite(pos);
    public System.Action<MinimapPinStructure> OnAddPinCallback;
    public void StartFogUpdater()
    {
        _minimapFog.StartFogTask();
    }
    public void EndFogUpdate()
    {
        _minimapFog.EndFogTask();
    }
    public MinimapSystem(int capacity)
    {
        _minimapDictionary = new Dictionary<Vector2Int, Sprite>(capacity);
        _minimapPinDictionary = new Dictionary<Object, MinimapPinStructure>(10);
        _minimapFog = new MinimapFog(capacity);
    }
    public void AddMinimapPiece(Vector2Int pos, Sprite minimapPiece)
    {
        _minimapDictionary.Add(pos, minimapPiece);
        _minimapFog.InstanceFogTexture(pos);
    }
    public void AddMinimapPin(Object key, MinimapPinStructure pinData)
    {
        pinData.IsActive = true;
        _minimapPinDictionary.Add(key, pinData);
        OnAddPinCallback?.Invoke(pinData);
    }
    public void RemovePin(Object key)
    {
        if(_minimapPinDictionary.ContainsKey(key))
        {
            _minimapPinDictionary[key].IsActive = false;
            _minimapPinDictionary.Remove(key);
        }    
    }
    public void Update(Vector2 position)
    {
        _minimapFog.Update(position);
    }
}
