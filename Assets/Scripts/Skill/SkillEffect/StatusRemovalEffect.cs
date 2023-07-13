using System.Collections.Generic;
using UnityEngine;

public class StatusRemovalEffect : ISkillEffect
{
    public StatusRemovalEffect(List<CharacterStatusType> requireStatus = null)
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
