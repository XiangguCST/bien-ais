public interface IHitCheckStrategy : ISkillEffect
{
    bool CheckHit(Character owner, Character target, SkillInstance skill);
}

public class RangeHitCheckStrategy : IHitCheckStrategy
{
    public RangeHitCheckStrategy(float range)
    {
        Range = range;
    }

    public bool CheckHit(Character owner, Character target, SkillInstance skill)
    {
        var finder = owner._targetFinder;
        return Range < 0 || finder._nearestDistance <= Range;
    }
    public float Range { get; set; } // 范围(小于0表示无限范围)
}

public class FaceTargetHitCheckStrategy : IHitCheckStrategy
{
    public FaceTargetHitCheckStrategy(float distance)
    {
        Distance = distance;
    }

    public bool CheckHit(Character owner, Character target, SkillInstance skill)
    {
        var finder = owner._targetFinder;
        if (!finder._isFindTarget)
            return false;
        return Distance < 0 || finder._nearestDistance <= Distance;
    }
    public float Distance{ get; set; }// 范围(小于0表示无限范围)
}
