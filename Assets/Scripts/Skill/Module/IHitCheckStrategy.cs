public interface IHitCheckStrategy
{
    bool CheckHit(Player owner, Skill skill);
}

public class RangeHitCheck : IHitCheckStrategy
{
    public RangeHitCheck(float range)
    {
        _range = range;
    }

    public bool CheckHit(Player owner, Skill skill)
    {
        var finder = owner._targetFinder;
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

    public bool CheckHit(Player owner, Skill skill)
    {
        var finder = owner._targetFinder;
        if (!finder._isFindTarget)
            return false;
        return _range < 0 || finder._nearestDistance <= _range;
    }
    private float _range; // 范围(小于0表示无限范围)
}
