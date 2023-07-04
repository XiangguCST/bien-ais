using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色技能管理
/// </summary>
public class CharacterSkillMgr
{
    public CharacterSkillMgr(Character owner)
    {
        _isGlobalCooldown = false;
        _isCasting = false;
        globalCooldownTime = 0f;
        _owner = owner;
    }

    public void UpdateSkillBar(float deltaTime)
    {
        
    }

    // 开始gcd
    public void StartGlobalCooldown(float cooldownTime)
    {
        if (!_isGlobalCooldown)
        {
            _isGlobalCooldown = true;
            globalCooldownTime = cooldownTime;
            CoroutineRunner.StartCoroutine(GlobalCooldownRoutine());
        }
    }

    // gcd过程
    private IEnumerator GlobalCooldownRoutine()
    {
        yield return new WaitForSeconds(globalCooldownTime);
        _isGlobalCooldown = false;
    }

    // 添加技能
    public void AttachSkill(KeyCode hotKey, Skill skill)
    {
        if (skill == null) return;
        var skillSlot = InputController.Instance.GetSkillSlotByHotKey(hotKey);
        if (skillSlot == null) return;

        var skillInstance = new SkillInstance(skill);
        skillSlot.SetSkill(skillInstance);
        skillInstance._owner = _owner;
        skillInstance._skillbar = this;
    }

    public SkillInstance GetCastingSkill()
    {
        return _castingSkill;
    }

    public Character _owner;
    public bool _isGlobalCooldown; // 是否gcd结束
    public bool _isCasting; // 是否释放技能中
    private float globalCooldownTime; // gcd

    private Dictionary<KeyCode, SkillInstance> _skills = new Dictionary<KeyCode, SkillInstance>(); // 技能格子列表
    public SkillInstance _castingSkill = null;
}