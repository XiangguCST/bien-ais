﻿using System;
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
        if (_skillManager == null) return false;
        if (_skillManager._isCasting)
        {
            SkillInstance castingSkill = _skillManager.GetCastingSkill();
            var castingPriority = castingSkill.SkillInfo._interruptPriority;
            var curPriority = SkillInfo._interruptPriority;
            if (curPriority <= castingPriority)
            {
                return false;
            }
        }

        //　技能可用性判断策略
        var usabilitys = SkillInfo._usabilitys;
        foreach (var usability in usabilitys)
        {
            if (!usability.IsSkillUsable(this))
                return false;
        }

        // 连锁技能在可用时间戳之前才能用
        if (SkillInfo._isChainSkill && Time.time >= _chainSkillEnableUntil)
            return false;

        if (!SkillInfo._statusRemovalStrategy.IsSkillUsable(this) ||
            _owner._energy < SkillInfo._energyCost)
            return false;

        return true;
    }

    // 简化后的IsSkillUsable()方法
    public bool IsSkillUsable()
    {
        if (!IsSkillUsableIgnoringCooldown() || _bIsCooldown || _skillManager._isGlobalCooldown)
            return false;
        return true;
    }

    /// <summary>
    /// 释放技能
    /// </summary>
    public void Activate()
    {
        // 打断正在释放的技能
        if (_skillManager._isCasting)
        {
            SkillInstance skill = _skillManager.GetCastingSkill();
            if (skill != null)
                skill.InterruptSkill();
        }

        // 技能消耗
        _owner.ConsumeEnergy(SkillInfo._energyCost);

        // 技能冷却
        _cooldownTimer = SkillInfo._cooldownTime;
        _bIsCooldown = true;
        CoroutineRunner.StartCoroutine(CooldownRoutine());

        // 释放技能
        _castTimer = SkillInfo._castTime;
        _damageDelayTimer = SkillInfo._damageDelay;
        _bDealDamage = false;
        _castCoroutine = CoroutineRunner.StartCoroutine(CastRoutine());

        // 触发技能效果的逻辑
        _owner.OnSkillEffect(this);
        _owner.TriggerAnimator(SkillInfo._animName);

        // 禁用所有连锁技能
        _skillManager.DisableAllChainSkills();

        // 启动连锁技能
        foreach (var chainSkill in _chainSkills)
        {
            chainSkill._chainSkillEnableUntil = Time.time + 3;
        }
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
        _skillManager._isCasting = true;
        _skillManager._castingSkill = this;
        PlayerCollisionManager.Instance.DisableCollision(_owner);
    }

    private void AfterSkillCast()
    {
        if (_skillManager == null)
            return;
        _skillManager._isCasting = false;
        _skillManager._castingSkill = null;
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

    // 检查目标是否有特定的buff并处理(返回是否能造成伤害)
    private bool CheckAndHandleBuffs(Character target)
    {
        if (target._buffManager.HasBuff(BuffType.ImmunityAll))
        {
            HandleImmunityAll(target);
            return false;
        }
        else if (target._buffManager.HasBuff(BuffType.ShadowClone))
        {
            return HandleShadowClone(target);
        }
        return true;
    }

    /// <summary>
    /// 处理命中抵抗buff
    /// </summary>
    /// <param name="target"></param>
    private void HandleImmunityAll(Character target)
    {
        target._buffManager.DecreaseBuffCount(BuffType.ImmunityAll, 1);
        target.ShowStatus("抗性");
    }

    private bool HandleShadowClone(Character target)
    {
        target._buffManager.DecreaseBuffCount(BuffType.ShadowClone, 1);
        // 打断对方正在释放的技能
        Player other = target as Player;
        if (other != null)
        {
            other.InterruptSkill();
        }
        // 是否无视防御
        if (SkillInfo._bBreakDefense)
        {
            return true;
        }
        else
        {
            // 打中替身虚弱2秒
            HandleHitShadowClone();
            return false;
        }
    }

    /// <summary>
    /// 处理命中替身buff
    /// </summary>
    private void HandleHitShadowClone()
    {
        if (_owner._buffManager.HasBuff(BuffType.ImmunityAll))
        {
            _owner._buffManager.DecreaseBuffCount(BuffType.ImmunityAll, 1);
            _owner.ShowStatus("抗性");
        }
        else
        {
            _owner._stateManager.AddStatus(CharacterStatusType.Weakness, 2.0f);
            _owner.ConsumeEnergy(4);
            _owner._targetFinder._nearestEnemy.ConsumeEnergy(-4);
        }
    }

    /// <summary>
    /// 命中判定
    /// </summary>
    /// <returns></returns>
    bool CheckHit()
    {
        var target = _owner._targetFinder._nearestEnemy;
        bool bNeedCheckHit = true;
        if (!SkillInfo._hitCheckStrategy.CheckHit(_owner, target, this, out bNeedCheckHit))
            return false;
        if (!bNeedCheckHit)
            return true;

        return CheckAndHandleBuffs(target);
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

    /// <summary>
    /// 禁用连锁技能
    /// </summary>
    public void DisableChainSkill()
    {
        _chainSkillEnableUntil = 0f;
    }


    public Skill SkillInfo { get; set; }

    public float _cooldownTimer; // 冷却计时器
    public float _castTimer; // 技能释放计时器
    public float _damageDelayTimer; // 伤害延迟判定计时器
    public bool _bIsCooldown; // 是否冷却中
    public bool _bDealDamage; // 是否进行伤害判定
    public CharacterSkillMgr _skillManager; // 技能栏
    public Character _owner; // 技能释放者
    public Action OnCooldownCompleted; // 冷却完成的事件
    public List<SkillInstance> _chainSkills = new List<SkillInstance>(); // 连锁技能列表
    private float _chainSkillEnableUntil = 0f; // 记录连锁技能的可用时间戳
    private Coroutine _castCoroutine;
}
