public interface IStatusAdditionStrategy
{
    void OnDealDamage(Player owner, Character target, Skill skill);
}

/// <summary>
/// 技能追加异常状态
/// </summary>
public class AddStatusEffect : IStatusAdditionStrategy
{
    public AddStatusEffect(CharacterStatusType addStatus, float statusTime)
    {
        _addStatus = addStatus;
        _statusTime = statusTime;
    }

    public void OnDealDamage(Player owner, Character target, Skill skill)
    {
        target._stateManager.AddStatus(_addStatus, _statusTime);
    }

    CharacterStatusType _addStatus; // 附加异常状态
    float _statusTime; // 异常状态时间
}

/// <summary>
/// 技能不追加任何异常状态效果
/// </summary>
public class DoNotAddStatusEffect : IStatusAdditionStrategy
{
    public void OnDealDamage(Player owner, Character target, Skill skill)
    {
    }
}
