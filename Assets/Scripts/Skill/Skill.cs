using System.Collections;
using UnityEngine;

// 技能类
public class Skill
{
    public Skill(SkillBar skillbar, string name, float rate, float range, float cooldownTime, float castTime, float damageDelay, float globalCooldownTime)
    {
        _skillbar = skillbar;
        _name = name;
        _rate = rate;
        _range = range;
        _cooldownTime = cooldownTime;
        _castTime = castTime;
        _damageDelay = damageDelay;
        _globalCooldownTime = globalCooldownTime;
        _cooldownTimer = 0f;
        _damageDelayTimer = 0f;
        _bIsCooldown = false;
    }

    public void ActivateEffect()
    {
        // 触发技能效果的逻辑
        _owner.OnSkillEffect(this);

        // 释放技能
        _castTimer = _castTime;
        _damageDelayTimer = _damageDelay;
        _skillbar._isCasting = true;
        CoroutineRunner.Instance.StartCoroutine(CastRoutine());

        // 技能冷却
        _cooldownTimer = _cooldownTime;
        _bIsCooldown = true;
        CoroutineRunner.Instance.StartCoroutine(CooldownRoutine());

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
            if(_damageDelayTimer <= 0)
            {
                if(CheckHit())
                {
                    DealDamage();
                }
            }
            yield return null;
        }

        _bIsCooldown = false;
    }

    bool CheckHit()
    {
        var finder = _owner._targetFinder;
        if (!finder._isFindTarget)
            return false;
        if (_range > 0 &&　finder._targetDistance > _range)
            return false;
        return true;
    }

    // 处理伤害判定
    void DealDamage()
    {
        var target = _owner._targetFinder._target;
        int damage = (int)(_owner._attr.atk * _rate);
        target.TakeDamage(damage);
    }

    public string _name; // 技能名称
    public float _rate; // 技能倍率
    public float _range; // 范围(小于0表示无限范围)
    public float _cooldownTime; // 冷却时间
    public float _castTime; // 释放时间
    public float _damageDelay; // 伤害判定延迟
    public float _globalCooldownTime; // gcd


    public float _cooldownTimer; // 冷却计时器
    public float _castTimer; // 技能释放计时器
    public float _damageDelayTimer; // 伤害延迟判定计时器
    public bool _bIsCooldown; // 是否冷却中

    SkillBar _skillbar; // 技能栏
    public Player _owner;
}
