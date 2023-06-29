using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 技能接口
public interface ISkill
{
    bool IsSkillUsable();
    void ActivateEffect();
}

// 技能类
public class Skill : ISkill
{
    public Skill(string name, string animName, int energyCost, int energyRecover,
        float rate,float cooldownTime, float castTime, float damageDelay, float globalCooldownTime,
        IStatusRemovalStrategy statusRemovalStrategy, IStatusAdditionStrategy statusAdditionStrategy,
        ITargetRequirementStrategy targetRequirementStrategy, IMovementStrategy movementStrategy, IHitCheckStrategy hitCheckStrategy)
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
        _statusRemovalStrategy = statusRemovalStrategy;
        _statusAdditionStrategy = statusAdditionStrategy;
        _targetRequirementStrategy = targetRequirementStrategy;
        _movementStrategy = movementStrategy;
        _hitCheckStrategy = hitCheckStrategy;

        _cooldownTimer = 0f;
        _damageDelayTimer = 0f;
        _bIsCooldown = false;
    }


    // 技能是否可用
    public bool IsSkillUsable()
    {
        if (!_statusRemovalStrategy.IsSkillUsable(_owner, this))
            return false;
        if (!_targetRequirementStrategy.IsSkillUsable(_owner, this))
            return false;
        if (_bIsCooldown || _skillbar._isGlobalCooldown || _skillbar._isCasting)
            return false;
        if (_owner._energy < _energyCost)
            return false;
        return true;
    }

    public void ActivateEffect()
    {
        // 技能消耗
        _owner.ConsumeEnergy(_energyCost);

        // 释放技能
        _castTimer = _castTime;
        _damageDelayTimer = _damageDelay;
        _skillbar._isCasting = true;
        _bDealDamage = false;
        CoroutineRunner.Instance.StartCoroutine(CastRoutine());

        // 技能冷却
        _cooldownTimer = _cooldownTime;
        _bIsCooldown = true;
        CoroutineRunner.Instance.StartCoroutine(CooldownRoutine());

        // 触发技能效果的逻辑
        _owner.OnSkillEffect(this);
        _owner.TriggerAnimator(_animName);
    }


    // 技能释放
    private IEnumerator CastRoutine()
    {
        // 技能释放前
        _movementStrategy.BeforeSkillCast(_owner, this);
        _statusRemovalStrategy.BeforeSkillCast(_owner, this);

        PlayerCollisionManager.Instance.DisableCollision(_owner);
        while (_castTimer > 0f)
        {
            _castTimer -= Time.deltaTime;

            _damageDelayTimer -= Time.deltaTime;
            if (!_bDealDamage && _damageDelayTimer <= 0)
            {
                if (CheckHit())
                {
                    DealDamage();
                }
                _bDealDamage = true;
            }

            // 技能释放过程中
            _movementStrategy.OnSkillCasting(_owner, this);
            _statusRemovalStrategy.OnSkillCasting(_owner, this);

            yield return null;
        }

        // 技能释放结束
        _skillbar._isCasting = false;
        _movementStrategy.AfterSkillCast(_owner, this);
        _statusRemovalStrategy.AfterSkillCast(_owner, this);

        PlayerCollisionManager.Instance.EnableCollision(_owner);
    }

    // 技能冷却
    private IEnumerator CooldownRoutine()
    {
        while (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            
            yield return null;
        }

        _bIsCooldown = false;
    }

    // 命中判定
    bool CheckHit()
    {
        return _hitCheckStrategy.CheckHit(_owner, this);
    }

    // 处理伤害
    void DealDamage()
    {
        _owner.ConsumeEnergy(-_energyRecover);
        var target = _owner._targetFinder._nearestEnemy;
        int rawDamage = (int)(_owner._attr.atk * _rate);
        int damage = (int)Random.Range(0.7f * rawDamage, 1.3f * rawDamage);
        _statusAdditionStrategy.OnDealDamage(_owner, target, this);
        target.TakeDamage(damage);
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

    public float _cooldownTimer; // 冷却计时器
    public float _castTimer; // 技能释放计时器
    public float _damageDelayTimer; // 伤害延迟判定计时器
    public bool _bIsCooldown; // 是否冷却中
    public bool _bDealDamage; // 是否进行伤害判定
    public SkillBar _skillbar; // 技能栏
    public Player _owner; // 技能释放者

    public IMovementStrategy _movementStrategy; // 技能移动策略
    public IStatusRemovalStrategy _statusRemovalStrategy; // 解除异常状态策略
    public IStatusAdditionStrategy _statusAdditionStrategy; // 附加异常状态策略
    public ITargetRequirementStrategy _targetRequirementStrategy; // 技能释放是否需要目标策略
    public IHitCheckStrategy _hitCheckStrategy; // 命中判定策略
}


