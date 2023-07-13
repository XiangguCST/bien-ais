public interface IStatusAdditionStrategy
{
    void OnDealDamage(Character owner, Character target, SkillInstance skill);
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

    public void OnDealDamage(Character owner, Character target, SkillInstance skill)
    {
        if (_addStatus == CharacterStatusType.None) return;
        target._stateManager.AddStatus(_addStatus, _statusTime);

        // 打断对方正在释放的技能
        Player other = target as Player;
        if(other != null)
        {
            other.InterruptSkill();
        }
    }

    public CharacterStatusType _addStatus; // 附加异常状态
    public float _statusTime; // 异常状态时间
}

/// <summary>
/// 技能不追加任何异常状态效果
/// </summary>
public class DoNotAddStatusEffect : IStatusAdditionStrategy
{
    public void OnDealDamage(Character owner, Character target, SkillInstance skill)
    {
    }
}
