public interface IHitCheckStrategy : ISkillEffect
{
    bool CheckHit(Character owner, Character target, SkillInstance skill);

    bool IsAOESkill();
}

public class RangeHitCheckStrategy : IHitCheckStrategy
{
    public RangeHitCheckStrategy(float range, bool isAOE = false)
    {
        Range = range;
        IsAOE = isAOE;
    }

    public bool CheckHit(Character owner, Character target, SkillInstance skill)
    {
        var finder = owner._targetFinder;
        return Range < 0 || finder._nearestDistance <= Range;
    }

    public bool IsAOESkill()
    {
        return IsAOE;
    }

    public float Range { get; set; } // 范围(小于0表示无限范围)
    public bool IsAOE { get; set; }
}

public class FaceTargetHitCheckStrategy : IHitCheckStrategy
{
    public FaceTargetHitCheckStrategy(float distance, bool isAOE = false)
    {
        Distance = distance;
        IsAOE = isAOE;
    }

    public bool CheckHit(Character owner, Character target, SkillInstance skill)
    {
        var finder = owner._targetFinder;
        if (!finder._isFindTarget)
            return false;
        return Distance < 0 || finder._nearestDistance <= Distance;
    }

    public bool IsAOESkill()
    {
        return IsAOE;
    }

    public float Distance{ get; set; }// 范围(小于0表示无限范围)
    public bool IsAOE { get; set; }
}
