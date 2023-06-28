using System.Collections;
using UnityEngine;

// 技能类
public class Skill
{
    public Skill(string name, string animName, int energyCost, int energyRecover,
        CharacterStatusType addStatus, float statusTime,
        float rate, bool bRequiredTarget, float requiredTargetDistance,
        float range, float cooldownTime, float castTime, float damageDelay, float globalCooldownTime)
    {
        _name = name;
        _animName = animName;
        _energyCost = energyCost;
        _energyRecover = energyRecover;
        _addStatus = addStatus;
        _statusTime = statusTime;
        _rate = rate;
        _bRequiredTarget = bRequiredTarget;
        _requiredTargetDistance = requiredTargetDistance;
        _range = range;
        _cooldownTime = cooldownTime;
        _castTime = castTime;
        _damageDelay = damageDelay;
        _globalCooldownTime = globalCooldownTime;
        _cooldownTimer = 0f;
        _damageDelayTimer = 0f;
        _bIsCooldown = false;
    }

    // 技能是否可用
    public bool IsSkillUsable()
    {
        if (_bIsCooldown || _skillbar._isGlobalCooldown || _skillbar._isCasting)
            return false;
        if(_bRequiredTarget)
        {
            var finder = _owner._targetFinder;
            if (!finder._isFindTarget)
                return false;
            if ( finder._targetDistance > _requiredTargetDistance)
                return false;
        }

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
        while (_castTimer > 0f)
        {
            _castTimer -= Time.deltaTime;
            yield return null;
        }

        _skillbar._isCasting = false;
    }

    // 技能冷却
    private IEnumerator CooldownRoutine()
    {
        while (_cooldownTimer > 0f)
        {
            _cooldownTimer -= Time.deltaTime;
            _damageDelayTimer -= Time.deltaTime;
            if(!_bDealDamage  && _damageDelayTimer <= 0)
            {
                if(CheckHit())
                {
                    DealDamage();
                }
                _bDealDamage = true;
            }
            yield return null;
        }

        _bIsCooldown = false;
    }

    // 命中判定
    bool CheckHit()
    {
        var finder = _owner._targetFinder;
        if (!finder._isFindTarget)
            return false;
        if (_range > 0 &&　finder._targetDistance > _range)
            return false;
        return true;
    }

    // 处理伤害
    void DealDamage()
    {
        _owner.ConsumeEnergy(-_energyRecover);
        var target = _owner._targetFinder._nearestEnemy;
        int rawDamage = (int)(_owner._attr.atk * _rate);
        int damage = (int)Random.Range(0.7f * rawDamage, 1.3f * rawDamage);
        target._stateManager.AddStatus(_addStatus, _statusTime);
        target.TakeDamage(damage);
    }

    public string _name; // 技能名称
    public string _animName; // 技能动画名称
    public int _energyCost; // 技能消耗的能量值
    public int _energyRecover; // 技能回复的能量值
    public CharacterStatusType _addStatus; // 附加异常状态
    public float _statusTime; // 异常状态时间
    public float _rate; // 技能倍率
    public bool _bRequiredTarget; // 技能释放是否需要目标
    public float _requiredTargetDistance; // 表示技能释放所需的目标距离
    public float _range; // 范围(小于0表示无限范围)
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
}
