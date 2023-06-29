public interface ITargetRequirementStrategy
{
    bool IsSkillUsable(Player owner, Skill skill);
}

public class NoTargetRequired : ITargetRequirementStrategy
{
    public bool IsSkillUsable(Player owner, Skill skill)
    {
        return true;
    }
}

public class TargetRequired : ITargetRequirementStrategy
{
    public TargetRequired(float requiredTargetDistance)
    {
        _requiredTargetDistance = requiredTargetDistance;
    }

    public bool IsSkillUsable(Player owner, Skill skill)
    {
        var finder = owner._targetFinder;
        return finder._isFindTarget && finder._nearestDistance <= _requiredTargetDistance;
    }

    float _requiredTargetDistance; // 表示技能释放所需的目标距离
}
