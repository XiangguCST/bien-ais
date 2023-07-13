using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能连锁策略
/// </summary>
public interface IChainStrategy
{
    /// <summary>
    /// 是否是连锁技能
    /// </summary>
    /// <returns></returns>
    bool IsChainSkill();

    /// <summary>
    /// 持续时间
    /// </summary>
    /// <returns></returns>
    float GetEnableUntilTime();

    /// <summary>
    /// 释放前能否使用其他技能
    /// </summary>
    /// <returns></returns>
    bool CanUseOtherSkillsBefore();
}


/// <summary>
/// 无策略
/// </summary>
public class NonChainSkillStrategy : IChainStrategy
{
    public bool CanUseOtherSkillsBefore()
    {
        throw new System.NotImplementedException();
    }

    public List<Skill> GetChainSkills()
    {
        throw new System.NotImplementedException();
    }

    public float GetEnableUntilTime()
    {
        throw new System.NotImplementedException();
    }

    public bool IsChainSkill()
    {
        return false;
    }
}

public class ChainSkillStrategy: IChainStrategy
{
    public ChainSkillStrategy(bool bUseOtherSkillsBefore = false, float enableTime = 3.0f)
    {
        _enableTime = enableTime;
        _bUseOtherSkillsBefore = bUseOtherSkillsBefore;
    }

    public bool IsChainSkill()
    {
        return true;
    }

    public float GetEnableUntilTime()
    {
        return _enableTime;
    }

    public bool CanUseOtherSkillsBefore()
    {
        return _bUseOtherSkillsBefore;
    }

    public float _enableTime;
    public bool _bUseOtherSkillsBefore;
}