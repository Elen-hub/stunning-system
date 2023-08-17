using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class CropObject : StaticActor
{
    bool _isGrowthing;
    protected Data.CropData CropData => DataManager.Instance.CropTable[Index];
    [SerializeField] protected float _growthElapsedTime = 0f;
    protected override void OnSpawn()
    {
        base.OnSpawn();

        _growthElapsedTime = 0f;
        _isGrowthing = true;
    }
    protected virtual void OnGrowth()
    {
        _isGrowthing = false;
        Vector2Int pos = new Vector2Int() {
            x = Mathf.FloorToInt(transform.position.x),
            y = Mathf.FloorToInt(transform.position.y),
        };
        Destroy();
        ActorManager.Instance.SpawnServerObject(CropData.GrowthObjectIndex, pos, OwnerGUID).ContinueWith(result =>
        {
            NetworkManager.Instance.ObjectEventSender.NotifyDestroyObject(WorldID);
            NetworkManager.Instance.ObjectEventSender.NotifyInstallObject(result);
        });
    }
    void OnUpdateGrowth()
    {
        if (!_isGrowthing)
            return;

        if (!IsAlive)
            return;

        _growthElapsedTime += TimeManager.DeltaTime;
        if (_growthElapsedTime >= CropData.GrowthTime)
        {
            OnGrowth();
        }
    }
    protected override void Update()
    {
        base.Update();

#if UNITY_SERVER
        OnUpdateGrowth();
#endif
    }
}
