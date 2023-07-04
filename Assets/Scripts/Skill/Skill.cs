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
        bool canInterruptOtherSkills, bool canBeInterrupted,
        IStatusRemovalStrategy statusRemovalStrategy, IStatusAdditionStrategy statusAdditionStrategy,
        ITargetRequirementStrategy targetRequirementStrategy, IMovementStrategy movementStrategy, IHitCheckStrategy hitCheckStrategy,
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
        _canInterruptOtherSkills = canInterruptOtherSkills;
        _canBeInterrupted = canBeInterrupted;

        _statusRemovalStrategy = statusRemovalStrategy;
        _statusAdditionStrategy = statusAdditionStrategy;
        _targetRequirementStrategy = targetRequirementStrategy;
        _movementStrategy = movementStrategy;
        _hitCheckStrategy = hitCheckStrategy;
        _buffAdditionStrategy = buffAdditionStrategy;
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
    public bool _canInterruptOtherSkills; // 是否允许打断其他技能
    public bool _canBeInterrupted; // 是否允许被打断

    public IMovementStrategy _movementStrategy; // 技能移动策略
    public IStatusRemovalStrategy _statusRemovalStrategy; // 解除异常状态策略
    public IStatusAdditionStrategy _statusAdditionStrategy; // 附加异常状态策略
    public ITargetRequirementStrategy _targetRequirementStrategy; // 技能释放是否需要目标策略
    public IHitCheckStrategy _hitCheckStrategy; // 命中判定策略
    public IBuffAdditionStrategy _buffAdditionStrategy; // buff添加策略

}


