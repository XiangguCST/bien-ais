public interface IHitCheckStrategy
{
    bool CheckHit(Character owner, Character target, SkillInstance skill, out bool bNeedCheckHit);
}

/// <summary>
/// 无需命中判定（通常为位移技能）
/// </summary>
public class DoNotHitCheck : IHitCheckStrategy
{
    public bool CheckHit(Character owner, Character target, SkillInstance skill, out bool bNeedCheckHit)
    {
        bNeedCheckHit = false;
        return true;
    }
}

public class RangeHitCheck : IHitCheckStrategy
{
    public RangeHitCheck(float range)
    {
        _range = range;
    }

    public bool CheckHit(Character owner, Character target, SkillInstance skill, out bool bNeedCheckHit)
    {
        var finder = owner._targetFinder;
        bNeedCheckHit = true;
        return _range < 0 || finder._nearestDistance <= _range;
    }
    private float _range; // 范围(小于0表示无限范围)
}

public class FaceTargetHitCheck : IHitCheckStrategy
{
    public FaceTargetHitCheck(float range)
    {
        _range = range;
    }

    public bool CheckHit(Character owner, Character target, SkillInstance skill, out bool bNeedCheckHit)
    {
        bNeedCheckHit = true;
        var finder = owner._targetFinder;
        if (!finder._isFindTarget)
            return false;
        return _range < 0 || finder._nearestDistance <= _range;
    }
    private float _range; // 范围(小于0表示无限范围)
}
