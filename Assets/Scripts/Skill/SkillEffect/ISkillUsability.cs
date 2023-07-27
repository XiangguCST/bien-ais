using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 技能使用条件接口
/// </summary>
public interface ISkillUsability : ISkillEffect
{
    bool IsSkillUsable(SkillInstance skillInstance);
}

/// <summary>
/// 需要有目标
/// </summary>
public class TargetRequired : ISkillUsability
{
    public TargetRequired(float minDistance, float maxDistance)
    {
        MinDistance = minDistance;
        MaxDistance = maxDistance;
    }

    public bool IsSkillUsable(SkillInstance skill)
    {
        var finder = skill._owner._targetFinder;
        return finder.HasTarget() && finder.GetDistance() <= MaxDistance && finder.GetDistance() >= MinDistance;
    }

    public float MinDistance { get; set; } // 表示技能释放所需的最近距离
    public float MaxDistance { get; set; } // 表示技能释放所需的最远距离
}

/// <summary>
/// 需要目标Buff
/// </summary>
public class TargetBuffRequired : ISkillUsability
{
    public TargetBuffRequired(BuffType buffType)
    {
        _buffType = buffType;
    }

    public bool IsSkillUsable(SkillInstance skill)
    {
        var finder = skill._owner._targetFinder;
        var target = finder.GetTarget();
        return target && target._buffManager.HasBuff(_buffType);
    }

    float _requiredTargetDistance; // 表示技能释放所需的目标距离
    private BuffType _buffType; // buff类型
}

/// <summary>
/// 需要目标状态
/// </summary>
public class TargetStatusRequired : ISkillUsability
{
    public TargetStatusRequired(List<CharacterStatusType> requireStatus = null)
    {
        if (requireStatus == null)
            Status = new List<CharacterStatusType> { CharacterStatusType.Stun, CharacterStatusType.Weakness, CharacterStatusType.Knockdown };
        else
            Status = requireStatus;
    }

    public bool IsSkillUsable(SkillInstance skill)
    {
        var finder = skill._owner._targetFinder;
        var target = finder.GetTarget();
        return target && Status.Contains(target._stateManager.GetCurrentStatus()) && target._stateManager.IsPure();
    }

    public List<CharacterStatusType> Status { get; set; }
}

public class HouGunFanUsability : ISkillUsability
{
    public bool IsSkillUsable(SkillInstance skillInstance)
    {
        return skillInstance._owner._stateManager.GetStatusElapsedTime() <= 1f;
    }
}