public abstract class PatternCondition
{
    public ePatternCondition Condition;
    public abstract bool IsMatchCondition(DynamicActor owner);
}
public class HPPatternCondition : PatternCondition
{
    public float MinRangeRate;
    public float MaxRangeRate;
    public override bool IsMatchCondition(DynamicActor owner)
    {
        return true;
    }
}
public class DistancePatternCondition : PatternCondition
{
    public float MinRange;
    public float MaxRange;
    public override bool IsMatchCondition(DynamicActor owner)
    {
        if (!owner.IsAlive) return false;
        if (owner.Controller.Target == null) return false;
        if (!owner.Controller.Target.IsAlive) return false;
        float sqrDistance = (owner.Controller.Target.Position - owner.Position).sqrMagnitude;
        return sqrDistance >= MinRange * MinRange && sqrDistance <= MaxRange * MaxRange;
    }
}