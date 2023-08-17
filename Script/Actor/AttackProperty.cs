
using Network;
public struct AttackProperty : IPacket
{
    public eDamageType DamageType;
    public ulong CasterID;
    public ulong TargetID;
    public bool IsCritical;
    public float Damage;
    #region Network
    public int GetByteSize =>
        ReliableHelper.ShortSize +
        ReliableHelper.ULongSize +
        ReliableHelper.ULongSize +
        ReliableHelper.BooleanSize +
        ReliableHelper.FloatSize;
    public void EnqueueByte()
    {
        BaseEventSender.CopyBytes((short)DamageType);
        BaseEventSender.CopyBytes(CasterID);
        BaseEventSender.CopyBytes(TargetID);
        BaseEventSender.CopyBytes(IsCritical);
        BaseEventSender.CopyBytes(Damage);
    }
    #endregion
    public AttackProperty(eDamageType damageType, ulong casterID, ulong targetID, bool isCritical, float damage)
    {
        DamageType = damageType;
        CasterID = casterID;
        TargetID = targetID;
        IsCritical = isCritical;
        Damage = damage;
    }
}