using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network;

public class ItemObject : MonoBehaviour, IPacket
{
    [SerializeField] Item _item;
    public Item GetItem => _item;
    [SerializeField] ulong _worldID;
    public ulong WorldID => _worldID;
    SpriteRenderer _spriteRenderer;
    Collider2D _overlapCollider;
    Coroutine _coroutine;
    Vector3 _spawnPosition;
    Vector3 _hoveringPosition;

    bool _isVisible;
    bool _isAlive;
    public bool IsPickupProcess;

    float _hoveringElapsedTime = 0f;
    const float _hoveringTargetTime = 1f;
    bool _isHoveringReverseState = false;
    public void Initialize()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _overlapCollider = GetComponent<Collider2D>();
    }
    public void Spawn(ulong worldID, Item item, Vector2 worldPosition)
    {
        _isAlive = false;
        _overlapCollider.enabled = false;
        _worldID = worldID;
        _item = item;
#if !UNITY_SERVER
        _spriteRenderer.sprite = item.ItemData.Icon;
#endif
        _spawnPosition = worldPosition + Vector2.up * 0.1f;
        _hoveringPosition = worldPosition + Vector2.up * 0.4f;
        transform.position = worldPosition;
        IsPickupProcess = false;
        gameObject.SetActive(true);
        if (_coroutine != null)
            StopCoroutine(_coroutine);

        _coroutine = StartCoroutine(IEDropAnimation());
    }
    public void Pickup(Transform target)
    {
        StartCoroutine(IEPickupAnimation(target));
    }
    public IEnumerator IEDropAnimation()
    {
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = transform.position + Vector3.up * 1.5f;
        StartCoroutine(CoroutineUtility.ScaleLerp(transform, Vector3.zero, Vector3.one, 0.2f));
        float cycleAmount = 0f;
        while (cycleAmount < 1f)
        {
            yield return null;
            cycleAmount += TimeManager.DeltaTime * 2f;
            transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.Sin(Mathf.PI * cycleAmount));
        }

        _isAlive = true;
        _overlapCollider.enabled = true;
        _coroutine = null;
        _hoveringElapsedTime = 0f;
        _isHoveringReverseState = false;
    }
    public IEnumerator IEPickupAnimation(Transform target)
    {
        _overlapCollider.enabled = false;
        Vector3 startPosition = transform.position;
        float speed = (transform.position - target.position).magnitude;
        StartCoroutine(CoroutineUtility.ScaleLerp(transform, Vector3.one, Vector3.zero, 0.5f));
        float elapsedTime = 0f;
        const float targetTime = 0.5f;
        while (elapsedTime < targetTime)
        {
            yield return null;
            elapsedTime += TimeManager.DeltaTime * speed;
            transform.position = Vector3.Lerp(startPosition, target.position, elapsedTime / targetTime);
        }
        Destroy();
    }
    protected void OnBecameVisible() => _isVisible = true;
    protected void OnBecameInvisible() => _isVisible = false;
    void OnUpdateHovering()
    {
        if (_isAlive && _isVisible)
        {
            _hoveringElapsedTime += TimeManager.DeltaTime;
            transform.position = Vector3.Lerp(_isHoveringReverseState ? _spawnPosition : _hoveringPosition, _isHoveringReverseState ? _hoveringPosition : _spawnPosition, _hoveringElapsedTime / _hoveringTargetTime);
            if (_hoveringElapsedTime > _hoveringTargetTime)
            {
                _hoveringElapsedTime = 0f;
                _isHoveringReverseState = !_isHoveringReverseState;
            }
        }
    }
    void LateUpdate()
    {
        OnUpdateHovering();
    }
    public void Destroy()
    {
        ItemManager.Instance.RemoveWorld(this);
        ItemManager.Instance.RegistObjectMemoryPool(this);
        gameObject.SetActive(false);
    }
    #region Network
    public int GetByteSize => _item.GetByteSize + ReliableHelper.ULongSize + ReliableHelper.Vector2Size;
    public void EnqueueByte()
    {
        _item.EnqueueByte();
        BaseEventSender.CopyBytes(_worldID);
        BaseEventSender.CopyBytes((Vector2)transform.position);
    }
#endregion
}
