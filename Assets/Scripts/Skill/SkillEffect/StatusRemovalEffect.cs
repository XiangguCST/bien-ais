using System.Collections.Generic;
using UnityEngine;

public class OwnerStatusRemovalEffect : ISkillEffect
{
    public OwnerStatusRemovalEffect(List<CharacterStatusType> requireStatus = null)
    {
        if (requireStatus == null)
            RemovalStatus = new List<CharacterStatusType> { CharacterStatusType.Stun, CharacterStatusType.Weakness, CharacterStatusType.Knockdown };
        else
            RemovalStatus = requireStatus;
    }

    public void OnRemoveStatusEffect(Character owner, SkillInstance skill)
    {
        // 移除异常状态
        owner._stateManager.RemoveAllStatuses();
    }

    public bool IsSkillUsable(SkillInstance skill)
    {
        return RemovalStatus.Contains(skill._owner._stateManager.GetCurrentStatus());
    }

    public List<CharacterStatusType> RemovalStatus { get; set; }
}

public class TargetStatusRemovalEffect : ISkillEffect
{
    public TargetStatusRemovalEffect(List<CharacterStatusType> requireStatus = null)
    {
        if (requireStatus == null)
            RemovalStatus = new List<CharacterStatusType> { CharacterStatusType.Stun, CharacterStatusType.Weakness, CharacterStatusType.Knockdown };
        else
            RemovalStatus = requireStatus;
    }

    public void OnRemoveStatusEffect(Character owner, SkillInstance skill)
    {
        var target = owner._targetFinder.GetTarget();

        // 移除异常状态
        target._stateManager.RemoveAllStatuses();
    }

    public List<CharacterStatusType> RemovalStatus { get; set; }
}
