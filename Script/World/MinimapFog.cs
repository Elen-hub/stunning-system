using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public class MinimapFog
{
    const int _sightRange = 5;
    const int _sightSqrRange = _sightRange * _sightRange;
    const int _size = 50, _halfExtents = 25;

    Dictionary<Vector2Int, Sprite> _fogTextureDictionary;
    Dictionary<Vector2Int, HashSet<Vector2Int>> _modifyPixelDictionary;
    Queue<ModifyPixelRequest> _requestQueue;
    Color32 _clearColor = Color.clear;
    HashSet<Vector2Int> _modifyTextureList = new HashSet<Vector2Int>(4);
    CancellationTokenSource _cancelToken;
    Vector2Int _position;
    Vector2Int _prevPosition = Vector2Int.down*int.MaxValue;

    public Sprite GetSprite(Vector2Int offset) => _fogTextureDictionary[offset];
    public MinimapFog(int capacity)
    {
        _fogTextureDictionary = new Dictionary<Vector2Int, Sprite>(capacity);
        _modifyPixelDictionary = new Dictionary<Vector2Int, HashSet<Vector2Int>>(capacity);
        _requestQueue = new Queue<ModifyPixelRequest>();
    }
    public void Update(Vector3 position)
    {
        if (_cancelToken != null)
        {
            _position = new Vector2Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y));
            OnUpdateFog();
        }
    }
    // 택스쳐 생성
    public void InstanceFogTexture(Vector2Int pos)
    {
        Color blackColor = Color.black;
        Texture2D texture = new Texture2D(_size, _size, TextureFormat.Alpha8, false);
        for (int x = 0; x < _size; ++x)
            for (int y = 0; y < _size; ++y)
            {
                texture.SetPixel(x, y, blackColor);
                texture.Apply(false, false);
            }
        texture.wrapMode = TextureWrapMode.Clamp;
        _fogTextureDictionary.Add(pos, Sprite.Create(texture, new Rect(0,0, _size, _size), new Vector2(0.5f, 0.5f)));
        _modifyPixelDictionary.Add(pos, new HashSet<Vector2Int>(_size * _size));
    }
    public void StartFogTask()
    {
        if (_cancelToken != null)
            _cancelToken.Cancel();

        _cancelToken = new CancellationTokenSource();
        UniTask.RunOnThreadPool(OnPorcessFogAsync, true, _cancelToken.Token);
    }
    public void EndFogTask()
    {
        if (_cancelToken != null)
        {
            _cancelToken.Cancel();
            _cancelToken = null;
        }
    }
    async UniTaskVoid OnPorcessFogAsync()
    {
        while (true)
        {
            if(_prevPosition == _position)
            {
                await UniTask.DelayFrame(1);
                continue;
            }
            _prevPosition = _position;
            int xMinRange = _position.x - _sightRange;
            int xMaxRange = _position.x + _sightRange;
            int yMinRange = _position.y - _sightRange;
            int yMaxRange = _position.y + _sightRange;
            for (int x = xMinRange; x <= xMaxRange; ++x)
                for (int y = yMinRange; y <= yMaxRange; ++y)
                {
                    if((new Vector2(x, y) - _position).sqrMagnitude <= _sightSqrRange)
                        OnWriteByte(x, y);
                }
            await UniTask.DelayFrame(5);
        }
    }
    void OnWriteByte(int x, int y)
    {
        int divX = x / _halfExtents;
        int divY = y / _halfExtents;
        Vector2Int pivot = new Vector2Int(divX, divY);
        if (pivot.x % 2 != 0) pivot.x += System.Math.Sign(x);
        if (pivot.y % 2 != 0) pivot.y += System.Math.Sign(y);
        pivot *= _halfExtents;
        Vector2Int position = new Vector2Int(x - pivot.x, y - pivot.y);
        if (_modifyPixelDictionary[pivot].Contains(position))
            return;

        _modifyPixelDictionary[pivot].Add(position);
        _requestQueue.Enqueue(new ModifyPixelRequest(pivot, position));
        if (!_modifyTextureList.Contains(pivot))
            _modifyTextureList.Add(pivot);
    }
    void OnUpdateFog()
    {
        int count = _requestQueue.Count;
        // 쓰레드에서의 요청사항 메인쓰레드 처리
        while (count > 0)
        {
            ModifyPixelRequest key;
            if (_requestQueue.TryDequeue(out key))
                _fogTextureDictionary[key.pivot].texture.SetPixel(key.position.x + _halfExtents, key.position.y + _halfExtents, _clearColor);

            --count;
        }
        // 변경사항이 적용된 텍스쳐 GPU에 적용
        if (_modifyTextureList.Count > 0)
        {
            foreach (var element in _modifyTextureList)
                _fogTextureDictionary[element].texture.Apply(false, false);

            _modifyTextureList.Clear();
        }
    }

    private readonly struct ModifyPixelRequest
    {
        public readonly Vector2Int pivot;
        public readonly Vector2Int position;
        public ModifyPixelRequest(Vector2Int pivot, Vector2Int position)
        {
            this.pivot = pivot;
            this.position = position;
        }
    }
}
