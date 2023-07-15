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
        if(SkillInfo.HasComponent<ISkillUsability>())
        {
            var usabilitys = SkillInfo.GetComponents<ISkillUsability>();
            foreach (var usability in usabilitys)
            {
                if (!usability.IsSkillUsable(this))
                    return false;
            }
        }

        // 连锁技能在可用时间戳之前才能用
        if (SkillInfo._chainStrategy.IsChainSkill() && Time.time >= _chainSkillEnableUntil)
            return false;

        if (SkillInfo.HasComponent<StatusRemovalEffect>())
        {
            var statusRemove = SkillInfo.GetComponent<StatusRemovalEffect>();
            if (!statusRemove.IsSkillUsable(this))
                return false;
        }
        else
        {
            // 判断是否角色处于异常状态
            if (_owner._stateManager.GetCurrentStatus() != CharacterStatusType.None)
            {
                return false;
            }
        }

        if (_owner._energy < SkillInfo._energyCost)
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
            chainSkill._chainSkillEnableUntil = Time.time + chainSkill.SkillInfo._chainStrategy.GetEnableUntilTime();
        }
    }

    // 技能释放
    private IEnumerator CastRoutine()
    {
        // 技能释放前
        if(SkillInfo.HasComponent<IMovementEffect>())
        {
            SkillInfo.GetComponent<IMovementEffect>().BeforeMove(_owner, this);
        }
        if (SkillInfo.HasComponent<StatusRemovalEffect>())
        {
            SkillInfo.GetComponent<StatusRemovalEffect>().OnRemoveStatusEffect(_owner, this);
        }
        if (SkillInfo.HasComponent<IBuffAdditionEffect>())
        {
            SkillInfo.GetComponent<IBuffAdditionEffect>().BeforeSkillCast(_owner, this);
        }
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
            if (SkillInfo.HasComponent<IMovementEffect>())
            {
                SkillInfo.GetComponent<IMovementEffect>().OnMoving(_owner, this);
            }

            yield return null;
        }

        // 技能释放结束
        if (SkillInfo.HasComponent<IMovementEffect>())
        {
            SkillInfo.GetComponent<IMovementEffect>().AfterMove(_owner, this);
        }
        AfterSkillCast();
    }

    private void BeforeSkillCast()
    {
        _skillManager._isCasting = true;
        _skillManager._castingSkill = this;
        CommonUtility.EnableCollision(_owner, false);
    }

    private void AfterSkillCast()
    {
        if (_skillManager == null)
            return;
        _skillManager._isCasting = false;
        _skillManager._castingSkill = null;
        CommonUtility.EnableCollision(_owner, true);
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
        // 不需要攻击判定直接返回
        if (!SkillInfo.HasComponent<IHitCheckStrategy>())
            return true;

        var target = _owner._targetFinder._nearestEnemy;
        var hitCheck = SkillInfo.GetComponent<IHitCheckStrategy>();
        if (!hitCheck.CheckHit(_owner, target, this))
            return false;
        return CheckAndHandleBuffs(target);
    }


    // 处理伤害
    void DealDamage()
    {
        _owner.ConsumeEnergy(-SkillInfo._energyRecover);
        var target = _owner._targetFinder._nearestEnemy;
        int damage = SkillDamageCalculator.CalcDamage(this);
        
        // 暴击判定
        bool bCrit;
        damage = SkillDamageCalculator.CritTest(damage, out bCrit);

        if(SkillInfo.HasComponent<StatusAdditionEffect>())
        {
            var statusAdd = SkillInfo.GetComponent<StatusAdditionEffect>();
            statusAdd.OnDealDamage(_owner, target, this);
        }
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
        CommonUtility.EnableCollision(_owner, true);
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
    public bool _bDoubleClick = false;  // 是否需要双击才能激活
    public bool _bIsToggleSkill = false; // 是否为替换技能
    public KeyCode _toggleKey; // 替换键
    public CharacterSkillMgr _skillManager; // 技能栏
    public Character _owner; // 技能释放者
    public Action OnCooldownCompleted; // 冷却完成的事件
    public List<SkillInstance> _chainSkills = new List<SkillInstance>(); // 连锁技能列表
    private float _chainSkillEnableUntil = 0f; // 记录连锁技能的可用时间戳
    private Coroutine _castCoroutine;
}

public class SkillDamageCalculator
{
    /// <summary>
    /// 判定是否暴击
    /// </summary>
    /// <returns></returns>
    public static int CritTest(int damage, out bool bCrit)
    {
        // 生成一个0到1之间的随机数
        float randomValue = UnityEngine.Random.value;

        // 检查随机数是否小于暴击几率
        bCrit = randomValue < _critChange;
        if(bCrit)
        {
            damage = (int)(damage * _critRate);
        }
        return damage;
    }

    /// <summary>
    /// 计算伤害
    /// </summary>
    /// <param name="skillInstance"></param>
    /// <returns></returns>
    internal static int CalcDamage(SkillInstance skillInstance)
    {
        int lowDamage;
        int highDamage;
        GetDamageRange(skillInstance, out lowDamage, out highDamage);
        return UnityEngine.Random.Range(lowDamage, highDamage);
    }

    public static void GetDamageRange(SkillInstance skillInstance, out int lowDamage, out int highDamage)
    {
        int rawDamage = GetRawDamage(skillInstance);
        lowDamage = (int)(_lowRate * rawDamage);
        highDamage = (int)(_highRate * rawDamage);
    }

    private static int GetRawDamage(SkillInstance skillInstance)
    {
        return (int)(skillInstance._owner._attr.atk * skillInstance.SkillInfo._rate);
    }

    public static readonly float _lowRate = 0.7f; // 最低伤害倍率
    public static readonly float _highRate = 1.3f; // 最高伤害倍率
    public static readonly float _critChange = 0.2f; // 暴击率
    public static readonly float _critRate = 3f; // 爆伤倍率
}