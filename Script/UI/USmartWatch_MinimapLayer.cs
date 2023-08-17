using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class USmartWatch_MinimapLayer : USmartWatchSubUI
{
    MemoryPool<UMinimapPin> _minimapPinPool = new MemoryPool<UMinimapPin>(5);
    List<UMinimapPin> _activatePinList = new List<UMinimapPin>(3);
    Queue<UMinimapPin> _removePinQueue = new Queue<UMinimapPin>();
    RectTransform _minimapTransform;
    Character _character;
    SmartWatch_MinimapPiece[] _minimapPieceArr;
    Vector2Int _currentOffset;
    Vector2Int _updatePos;
    protected override void InitReference()
    {
        base.InitReference();

        _minimapPinPool = new MemoryPool<UMinimapPin>(6);
        _minimapTransform = transform.Find("Pivot") as RectTransform;
        _minimapPieceArr = GetComponentsInChildren<SmartWatch_MinimapPiece>(true);
        int count = 0;
        for(int x = -100; x <= 100; x += 50)
            for (int y = -100; y <= 100; y += 50)
            {
                _minimapPieceArr[count].Initialize(new Vector2Int(x, y));
                ++count;
            }
    }
    UMinimapPin GetMinimapPin()
    {
        UMinimapPin pin = _minimapPinPool.GetItem();
        if (pin == null)
        {
            pin = Instantiate(Resources.Load<UMinimapPin>("UI/CacheElement/UMinimapPin"), transform);
            pin.Initialize();
        }
        return pin;
    }
    void OnAddMinimapPin(MinimapPinStructure pinData)
    {
        UMinimapPin pin = GetMinimapPin();
        pin.Enable(pinData);
        _activatePinList.Add(pin);
    }
    void OnUpdatePosition()
    {
        Vector2Int pos = new Vector2Int(Mathf.RoundToInt(_character.Position.x), Mathf.RoundToInt(_character.Position.y));
        if(_updatePos != pos)
        {
            _updatePos = pos;
            pos = new Vector2Int((pos.x / 50) * 50, (pos.y / 50) * 50);
            if (_currentOffset != pos)
            {
                _currentOffset = pos;
                for (int i = 0; i < _minimapPieceArr.Length; ++i)
                {
                    Vector2Int offset = _currentOffset + _minimapPieceArr[i].Offset;
                    Sprite sprite = WorldManager.Instance.GetMinimapPiece(offset);
                    if (sprite != null)
                    {
                        _minimapPieceArr[i].sprite = sprite;
                        _minimapPieceArr[i].SetFogSprite(WorldManager.Instance.MinimapSystem.GetMinimapFogSprite(offset));
                    }
                    else
                    {
                        _minimapPieceArr[i].sprite = null;
                        _minimapPieceArr[i].SetFogSprite(null);
                    }
                }
            }
        }
        _minimapTransform.anchoredPosition = (_currentOffset - _character.Position) * 2;
    }
    void OnUpdatePin()
    {
        for (int i = 0; i < _activatePinList.Count; ++i)
        {
            _activatePinList[i].Offset = _updatePos;
            if (!_activatePinList[i].IsActivate)
                _removePinQueue.Enqueue(_activatePinList[i]);
        }
        while(_removePinQueue.Count > 0)
        {
            UMinimapPin pin = _removePinQueue.Dequeue();
            pin.Disable();
        }
    }
    public override void Enable()
    {
        for(int i = 0; i < _minimapPieceArr.Length; ++i)
        {
            Vector2Int offset = _currentOffset + _minimapPieceArr[i].Offset;
            Sprite sprite = WorldManager.Instance.GetMinimapPiece(offset);
            if (sprite != null)
            {
                _minimapPieceArr[i].sprite = sprite;
                _minimapPieceArr[i].SetFogSprite(WorldManager.Instance.MinimapSystem.GetMinimapFogSprite(offset));
            }
            else
            {
                _minimapPieceArr[i].sprite = null;
                _minimapPieceArr[i].SetFogSprite(null);
            }
        }
        Dictionary<Object, MinimapPinStructure> minimapPinDic = WorldManager.Instance.MinimapSystem.GetMinimapPinDictionary;
        foreach (var element in minimapPinDic)
        {
            UMinimapPin pin = GetMinimapPin();
            pin.Enable(element.Value);
            _activatePinList.Add(pin);
        }
        WorldManager.Instance.MinimapSystem.OnAddPinCallback += OnAddMinimapPin;
        gameObject.SetActive(true);
    }
    public override void Disable()
    {
        for (int i = 0; i < _activatePinList.Count; ++i)
        {
            _activatePinList[i].Disable();
            _minimapPinPool.Register(_activatePinList[i]);
        }
        _activatePinList.Clear();
        WorldManager.Instance.MinimapSystem.OnAddPinCallback -= OnAddMinimapPin;
        gameObject.SetActive(false);
    }
    private void LateUpdate()
    {
        if (_character == null)
        {
            Player player = PlayerManager.Instance.Me;
            if (player == null)
                return;

            DynamicActor dynamicActor = player.Character;
            if (dynamicActor != null)
                _character = dynamicActor as Character;

            return;
        }
        OnUpdatePosition();
        OnUpdatePin();
    }
}
