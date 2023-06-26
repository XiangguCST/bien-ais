using System.Collections;
using UnityEngine;

// 技能类
public class Skill

{
    public Skill(SkillBar manager, string name, float cooldownTime, float duringTime, float globalCooldownTime)
    {
        _manager = manager;
        _name = name;
        _cooldownTime = cooldownTime;
        _castTime = duringTime;
        _globalCooldownTime = globalCooldownTime;
        _cooldownTimer = 0f;
        _bIsCooldown = false;
    }

    public void ActivateEffect()
    {
        // 触发技能效果的逻辑
        Debug.Log("Skill activated: " + _name);
        // ...

        // 设置技能释放时间
        _castTimer = _castTime;
        _manager._isCasting = true;

        // 启动技能持续计时器
        CoroutineRunner.Instance.StartCoroutine(CastRoutine());
    }

    
    // 技能释放
    private IEnumerator CastRoutine()
    {
        while (_castTimer > 0f)
        {
            _castTimer -= Time.deltaTime;
            yield return null;
        }

        _manager._isCasting = false;

        // 技能释放结束后开始计算冷却时间
        _cooldownTimer = _cooldownTime;
        _bIsCooldown = true;

        // 启动技能冷却计时器
        CoroutineRunner.Instance.StartCoroutine(CooldownRoutine());
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

    public string _name; // 技能名称
    public float _cooldownTime; // 冷却时间
    public float _castTime; // 释放时间
    public float _globalCooldownTime; // gcd

    public float _cooldownTimer; // 冷却计时器
    public float _castTimer; // 技能释放计时器
    public bool _bIsCooldown; // 是否冷却中

    SkillBar _manager;
}
