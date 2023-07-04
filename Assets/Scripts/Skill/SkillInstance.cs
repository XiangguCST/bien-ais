using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能实例类(包含技能的状态和冷却)
/// </summary>
public class SkillInstance
{
    public SkillInstance(Skill skill)
    {
        SkillInfo = skill;
        Reset();
    }

    public void Reset()
    {
        _cooldownTimer = SkillInfo._cooldownTime;
        _castTimer = SkillInfo._castTime;
        _damageDelayTimer = SkillInfo._damageDelay;
        _bIsCooldown = false;
        _bDealDamage = false;
        _castCoroutine = null;
    }

    // 新增方法: IsSkillUsableIgnoringCooldown()
    public bool IsSkillUsableIgnoringCooldown()
    {
        if (_skillbar == null || _skillbar._isCasting && !SkillInfo._canInterruptOtherSkills) return false;
        if (_skillbar._isCasting && SkillInfo._canInterruptOtherSkills)
        {
            SkillInstance skill = _skillbar.GetCastingSkill();
            if (!skill.SkillInfo._canBeInterrupted)
            {
                return false;
            }
        }

        if (!SkillInfo._statusRemovalStrategy.IsSkillUsable(this) ||
            !SkillInfo._targetRequirementStrategy.IsSkillUsable(this) ||
            _owner._energy < SkillInfo._energyCost)
            return false;

        return true;
    }

    // 简化后的IsSkillUsable()方法
    public bool IsSkillUsable()
    {
        if (!IsSkillUsableIgnoringCooldown() || _bIsCooldown || _skillbar._isGlobalCooldown)
            return false;
        return true;
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    public void Activate()
    {
        // 打断正在释放的技能
        if (_skillbar._isCasting)
        {
            SkillInstance skill = _skillbar.GetCastingSkill();
            if (skill != null)
                skill.InterruptSkill();
        }

        // 技能消耗
        _owner.ConsumeEnergy(SkillInfo._energyCost);

        // 释放技能
        _castTimer = SkillInfo._castTime;
        _damageDelayTimer = SkillInfo._damageDelay;
        _bDealDamage = false;
        _castCoroutine = CoroutineRunner.StartCoroutine(CastRoutine());

        // 技能冷却
        _cooldownTimer = SkillInfo._cooldownTime;
        _bIsCooldown = true;
        CoroutineRunner.StartCoroutine(CooldownRoutine());

        // 触发技能效果的逻辑
        _owner.OnSkillEffect(this);
        _owner.TriggerAnimator(SkillInfo._animName);
    }

    // 技能释放
    private IEnumerator CastRoutine()
    {
        // 技能释放前
        SkillInfo._movementStrategy.BeforeSkillCast(_owner, this);
        SkillInfo._statusRemovalStrategy.BeforeSkillCast(_owner, this);
        SkillInfo._buffAdditionStrategy.BeforeSkillCast(_owner, this);
        BeforeSkillCast();
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
            SkillInfo._movementStrategy.OnSkillCasting(_owner, this);
            SkillInfo._statusRemovalStrategy.OnSkillCasting(_owner, this);

            yield return null;
        }

        // 技能释放结束
        SkillInfo._movementStrategy.AfterSkillCast(_owner, this);
        SkillInfo._statusRemovalStrategy.AfterSkillCast(_owner, this);
        AfterSkillCast();
    }

    private void BeforeSkillCast()
    {
        _skillbar._isCasting = true;
        _skillbar._castingSkill = this;
        PlayerCollisionManager.Instance.DisableCollision(_owner);
    }

    private void AfterSkillCast()
    {
        if (_skillbar == null)
            return;
        _skillbar._isCasting = false;
        _skillbar._castingSkill = null;
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
        OnCooldownCompleted?.Invoke(); // 当冷却时间结束，触发事件
    }

    // 命中判定
    bool CheckHit()
    {
        var target = _owner._targetFinder._nearestEnemy;
        if (!SkillInfo._hitCheckStrategy.CheckHit(_owner, target, this))
            return false;

        // 打中抵抗
        if (target._buffManager.HasBuff(BuffType.ImmunityAll))
        {
            target._buffManager.DecreaseBuffCount(BuffType.ImmunityAll, 1);
            target.ShowStatus("抗性");
            return false;
        }
        // 打中替身
        else if (target._buffManager.HasBuff(BuffType.ShadowClone))
        {
            target._buffManager.DecreaseBuffCount(BuffType.ShadowClone, 1);
            // 打断对方正在释放的技能
            Player other = target as Player;
            if (other != null)
            {
                other.InterruptSkill();
            }
            // 打中替身虚弱2秒
            if (_owner._buffManager.HasBuff(BuffType.ImmunityAll))
            {
                _owner._buffManager.DecreaseBuffCount(BuffType.ImmunityAll, 1);
                _owner.ShowStatus("抗性");
            }
            else
            {
                _owner._stateManager.AddStatus(CharacterStatusType.Weakness, 2.0f);
                _owner.ConsumeEnergy(4);
                target.ConsumeEnergy(-4);
            }
            return false;
        }

        return true;
    }

    // 处理伤害
    void DealDamage()
    {
        _owner.ConsumeEnergy(-SkillInfo._energyRecover);
        var target = _owner._targetFinder._nearestEnemy;
        int rawDamage = (int)(_owner._attr.atk * SkillInfo._rate);
        int damage = (int)UnityEngine.Random.Range(0.7f * rawDamage, 1.3f * rawDamage);
        // 暴击
        bool bCrit = CommonUtility.IsCrit(0.2f);
        if (bCrit)
        {
            damage *= 3;
        }
        SkillInfo._statusAdditionStrategy.OnDealDamage(_owner, target, this);
        if (damage > 0)
        {
            target.TakeDamage(damage, bCrit);
            HurtEffectController.ShowHurtEffect(target);
        }
    }

    public void InterruptSkill()
    {
        if (_castCoroutine != null)
            CoroutineRunner.StopCoroutine(_castCoroutine);
        AfterSkillCast();
        SkillInfo._movementStrategy.InterruptSkill(_owner, this);
        SkillInfo._statusRemovalStrategy.InterruptSkill(_owner, this);
        PlayerCollisionManager.Instance.EnableCollision(_owner);
    }

    public Skill SkillInfo { get; set; }

    public float _cooldownTimer; // 冷却计时器
    public float _castTimer; // 技能释放计时器
    public float _damageDelayTimer; // 伤害延迟判定计时器
    public bool _bIsCooldown; // 是否冷却中
    public bool _bDealDamage; // 是否进行伤害判定
    public CharacterSkillMgr _skillbar; // 技能栏
    public Character _owner; // 技能释放者
    public Action OnCooldownCompleted; // 冷却完成的事件
    private Coroutine _castCoroutine;
}
