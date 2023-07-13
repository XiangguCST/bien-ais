/// <summary>
/// 技能追加异常状态
/// </summary>
public class StatusAdditionEffect : ISkillEffect
{
    public StatusAdditionEffect(CharacterStatusType addStatus, float statusTime)
    {
        AddStatus = addStatus;
        StatusTime = statusTime;
    }

    public void OnDealDamage(Character owner, Character target, SkillInstance skill)
    {
        if (AddStatus == CharacterStatusType.None) return;
        target._stateManager.AddStatus(AddStatus, StatusTime);

        // 打断对方正在释放的技能
        Player other = target as Player;
        if (other != null)
        {
            other.InterruptSkill();
        }
    }

    public CharacterStatusType AddStatus { get; set; } // 附加异常状态
    public float StatusTime{ get; set; } // 异常状态时间
}

