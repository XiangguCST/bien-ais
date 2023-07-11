using System.Collections.Generic;
using UnityEngine;

public interface IStatusRemovalStrategy
{
    bool IsSkillUsable(SkillInstance skill);
    void BeforeSkillCast(Character owner, SkillInstance skill);
    void OnSkillCasting(Character owner, SkillInstance skill);
    void AfterSkillCast(Character owner, SkillInstance skill);
    void InterruptSkill(Character owner, SkillInstance skill);
}

public class RemoveStatuses : IStatusRemovalStrategy
{
    public RemoveStatuses(List<CharacterStatusType> requireStatus = null)
    {
        if (requireStatus == null)
            _requireStatus = new List<CharacterStatusType> { CharacterStatusType.Stun, CharacterStatusType.Weakness, CharacterStatusType.Knockdown };
        else
            _requireStatus = requireStatus;
    }


    public void BeforeSkillCast(Character owner, SkillInstance skill)
    {
        // 移除异常状态
        owner._stateManager.RemoveAllStatuses();
    }

    public void OnSkillCasting(Character owner, SkillInstance skill)
    {
        // 技能释放过程中不做任何操作
    }

    public void AfterSkillCast(Character owner, SkillInstance skill)
    {
        // 技能释放结束后不做任何操作
    }

    public bool IsSkillUsable(SkillInstance skill)
    {
        return _requireStatus.Contains(skill._owner._stateManager.GetCurrentStatus());
    }

    public void InterruptSkill(Character owner, SkillInstance skill)
    {
    }

    List<CharacterStatusType> _requireStatus;
}

public class DoNotRemoveStatuses : IStatusRemovalStrategy
{
    public DoNotRemoveStatuses()
    {
        _requireStatus = new List<CharacterStatusType> { CharacterStatusType.None};
    }

    public void InterruptSkill(Character owner, SkillInstance skill)
    {
    }

    public void BeforeSkillCast(Character owner, SkillInstance skill)
    {
        // 移除异常状态
        owner._stateManager.RemoveAllStatuses();
    }

    public void OnSkillCasting(Character owner, SkillInstance skill)
    {
        // 技能释放过程中不做任何操作
    }

    public void AfterSkillCast(Character owner, SkillInstance skill)
    {
        // 技能释放结束后不做任何操作
    }

    public bool IsSkillUsable(SkillInstance skill)
    {
        return _requireStatus.Contains(skill._owner._stateManager.GetCurrentStatus());
    }

    List<CharacterStatusType> _requireStatus;
}