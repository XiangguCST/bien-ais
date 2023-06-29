public interface IStatusAdditionStrategy
{
    void OnDealDamage(Player owner, Character target, Skill skill);
}

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

public class DoNotAddStatusEffect : IStatusAdditionStrategy
{
    public void OnDealDamage(Player owner, Character target, Skill skill)
    {
    }
}
