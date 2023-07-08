using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 技能类
[Serializable]
public class Skill 
{
    public Skill(string name, string animName, int energyCost, int energyRecover,
        float rate,float cooldownTime, float castTime, float damageDelay, float globalCooldownTime,
        SkillPriority priority, bool canInterruptOtherSkills, bool canBeInterrupted, bool bBreakDefense,
        IStatusRemovalStrategy statusRemovalStrategy, IStatusAdditionStrategy statusAdditionStrategy,
        IMovementStrategy movementStrategy, IHitCheckStrategy hitCheckStrategy,
        IBuffAdditionStrategy buffAdditionStrategy)
    {
        _name = name;
        _animName = animName;
        _energyCost = energyCost;
        _energyRecover = energyRecover;
        _rate = rate;
        _cooldownTime = cooldownTime;
        _castTime = castTime;
        _damageDelay = damageDelay;
        _globalCooldownTime = globalCooldownTime;
        _priority = priority;
        _canInterruptOtherSkills = canInterruptOtherSkills;
        _canBeInterrupted = canBeInterrupted;
        _bBreakDefense = bBreakDefense;

        _statusRemovalStrategy = statusRemovalStrategy;
        _statusAdditionStrategy = statusAdditionStrategy;
        _movementStrategy = movementStrategy;
        _hitCheckStrategy = hitCheckStrategy;
        _buffAdditionStrategy = buffAdditionStrategy;
    }

    /// <summary>
    /// 添加技能使用条件
    /// </summary>
    public Skill AddSkillUsability(ISkillUsability usabilitys)
    {
        if(usabilitys != null)
            _usabilitys.Add(usabilitys);
        return this;
    }

    public Skill AddChainSkill(Skill skill)
    {
        if (skill != null)
        {
            skill._isChainSkill = true;
            _chainSkills.Add(skill);
        }
        return this;
    }

    public string _name; // 技能名称
    public string _animName; // 技能动画名称
    public int _energyCost; // 技能消耗的能量值
    public int _energyRecover; // 技能回复的能量值
    
    public float _rate; // 技能倍率
    public float _cooldownTime; // 冷却时间
    public float _castTime; // 释放时间
    public float _damageDelay; // 伤害判定延迟
    public float _globalCooldownTime; // gcd
    public SkillPriority _priority; // 技能优先级
    public bool _canInterruptOtherSkills; // 是否允许打断其他技能
    public bool _canBeInterrupted; // 是否允许被打断
    public bool _bBreakDefense; // 是否无视防御

    public IMovementStrategy _movementStrategy; // 技能移动策略
    public IStatusRemovalStrategy _statusRemovalStrategy; // 解除异常状态策略
    public IStatusAdditionStrategy _statusAdditionStrategy; // 附加异常状态策略
    public IHitCheckStrategy _hitCheckStrategy; // 命中判定策略
    public IBuffAdditionStrategy _buffAdditionStrategy; // buff添加策略
    public List<ISkillUsability> _usabilitys = new List<ISkillUsability>(); // 技能使用条件列表
    public List<Skill> _chainSkills = new List<Skill>(); // 连锁技能列表
    public bool _isChainSkill = false; // 是否为连锁技能
}

public enum SkillPriority
{
    Normal = 0,    // 普通技能
    Conditional,   // 条件技能
    Chain          // 连锁技能
}

