using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 技能栏
/// </summary>
public class SkillBar : MonoBehaviour
{
    public SkillBar()
    {
        _isGlobalCooldown = false;
        _isCasting = false;
        globalCooldownTime = 0f;
    }

    public void InitSkillBar()
    {
        // 获取所有技能格子
        var slots = GetComponentsInChildren<SkillSlot>();
        foreach (var slot in slots)
        {
            _slots[slot._hotKey] = slot;
        }

        _owner._skillBar = this;
    }

    public void UpdateSkillBar(float deltaTime)
    {
        foreach (var pair in _slots)
        {
            var slot = pair.Value;
            if (Input.GetKeyDown(slot._hotKey))
            {
                slot.Activate();
            }
        }
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
        if (!_slots.ContainsKey(hotKey)) return;
        _slots[hotKey].SetSkill(skill);
        skill._owner = _owner;
        skill._skillbar = this;
    }

    public Skill GetCastingSkill()
    {
        return _castingSkill;
    }

    public Player _owner;
    public bool _isGlobalCooldown; // 是否gcd结束
    public bool _isCasting; // 是否释放技能中
    private float globalCooldownTime; // gcd

    private Dictionary<KeyCode, SkillSlot> _slots = new Dictionary<KeyCode, SkillSlot>(); // 技能格子列表
    public Skill _castingSkill = null;
}