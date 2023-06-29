using System.Collections.Generic;

public interface IStatusRemovalStrategy
{
    bool IsSkillUsable(Player owner, Skill skill);
    void BeforeSkillCast(Player owner, Skill skill);
    void OnSkillCasting(Player owner, Skill skill);
    void AfterSkillCast(Player owner, Skill skill);
}

public class RemoveAllStatuses : IStatusRemovalStrategy
{
    public RemoveAllStatuses()
    {
        _requireStatus = new List<CharacterStatusType> { CharacterStatusType.Stun, CharacterStatusType.Weakness, CharacterStatusType.Knockdown };
    }


    public void BeforeSkillCast(Player owner, Skill skill)
    {
        // 移除异常状态
        owner._stateManager.RemoveAllStatuses();
    }

    public void OnSkillCasting(Player owner, Skill skill)
    {
        // 技能释放过程中不做任何操作
    }

    public void AfterSkillCast(Player owner, Skill skill)
    {
        // 技能释放结束后不做任何操作
    }

    public bool IsSkillUsable(Player owner, Skill skill)
    {
        return _requireStatus.Contains(owner._stateManager.GetCurrentStatus());
    }

    List<CharacterStatusType> _requireStatus;
}

public class DoNotRemoveStatuses : IStatusRemovalStrategy
{
    public DoNotRemoveStatuses()
    {
        _requireStatus = new List<CharacterStatusType> { CharacterStatusType.None};
    }


    public void BeforeSkillCast(Player owner, Skill skill)
    {
        // 移除异常状态
        owner._stateManager.RemoveAllStatuses();
    }

    public void OnSkillCasting(Player owner, Skill skill)
    {
        // 技能释放过程中不做任何操作
    }

    public void AfterSkillCast(Player owner, Skill skill)
    {
        // 技能释放结束后不做任何操作
    }

    public bool IsSkillUsable(Player owner, Skill skill)
    {
        return _requireStatus.Contains(owner._stateManager.GetCurrentStatus());
    }

    List<CharacterStatusType> _requireStatus;
}