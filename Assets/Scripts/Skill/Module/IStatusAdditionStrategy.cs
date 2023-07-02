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
        if (_addStatus == CharacterStatusType.None) return;
        else if (_addStatus == CharacterStatusType.Stun) target.ShowStatus("眩晕");
        else if (_addStatus == CharacterStatusType.Weakness) target.ShowStatus("虚弱");
        else if (_addStatus == CharacterStatusType.Knockdown) target.ShowStatus("击倒");
        target._stateManager.AddStatus(_addStatus, _statusTime);

        // 打断对方正在释放的技能
        Player other = target as Player;
        if(other != null)
        {
            if(other._skillBar._isCasting)
            {
                var castingSkill = other._skillBar._castingSkill;
                if (castingSkill != null)
                    castingSkill.InterruptSkill();
            }
        }
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
