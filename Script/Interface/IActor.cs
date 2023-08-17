using UnityEngine;
public interface IActor : IComponent {
    Fusion.NetworkRunner MyRunner { get; }
    int Index { get; }
    ulong WorldID { get; }
    Vector2 Direction { get; }
    Vector2 Position { get; }
    Transform transform { get; }
    ActorStat ActorStat { get; }
    ActorBuff ActorBuff { get; }
    ActorController Controller { get; }
    Attachment Attachment { get; }
    eActorType ActorType { get; }
    float CurrentHP { get; set; }
    bool IsAlive { get; }
    bool IsExist { get; }
    int AllyNumber { get; set; }
    InOutState InOutState { get; }
    public eAllyType GetAllyType(int toAllyNumber);
    void Hit(AttackProperty attackProperty);
    void Destroy();
    Coroutine StartCoroutine(System.Collections.IEnumerator enumerator);
    void StopCoroutine(Coroutine routine);
    event System.Action OnDestroyCallback;
}