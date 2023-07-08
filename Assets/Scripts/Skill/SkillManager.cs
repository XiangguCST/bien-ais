using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void AttachSkill(KeyCode hotKey, Skill skill)
    {
        if (skill == null) return;
        var skillSlot = InputController.Instance.GetSkillSlotByHotKey(hotKey);
        if (skillSlot == null) return;

        var skillInstance = new SkillInstance(skill);
        skillInstance._owner = _owner;
        skillInstance._skillbar = this;

        // 保存到优先级列表中
        if (!_skills.ContainsKey(hotKey))
        {
            _skills[hotKey] = new List<SkillInstance>();
        }
        _skills[hotKey].Add(skillInstance);
        _skills[hotKey] = _skills[hotKey].OrderByDescending(s => s.SkillInfo._priority).ToList();
        skillSlot.SetSkill(GetHighestPrioritySkill(hotKey));
    }


    public SkillInstance GetCastingSkill()
    {
        return _castingSkill;
    }

    // 返回优先级最高的可用技能
    public SkillInstance GetHighestPrioritySkill(KeyCode hotKey)
    {
        SkillInstance highestPrioritySkill = null;

        if (_skills.ContainsKey(hotKey))
        {
            foreach (var skill in _skills[hotKey])
            {
                // 如果找到了可用的技能，就立即返回它
                if (skill.IsSkillUsable())
                {
                    return skill;
                }
                highestPrioritySkill = skill;
            }
        }
        // 如果所有的技能都不可用，就返回优先级最低的技能
        return highestPrioritySkill;
    }

    // 应用所有技能
    public void ApplyAllSkills()
    {
        foreach (var pair in _skills)
        {
            foreach (var skillInstance in pair.Value)
            {
                // 在技能字典中查找连锁技能
                foreach (var chainSkill in skillInstance.SkillInfo._chainSkills)
                {
                    var chainSkillKeyPair = _skills.FirstOrDefault(kvp => kvp.Value.Any(s => s.SkillInfo == chainSkill));
                    if (!chainSkillKeyPair.Equals(default(KeyValuePair<KeyCode, List<SkillInstance>>)))
                    {
                        var chainSkillInstance = chainSkillKeyPair.Value.First(s => s.SkillInfo == chainSkill);
                        skillInstance._chainSkills.Add(chainSkillInstance);
                    }
                }
            }
        }
    }

    public Character _owner;
    public bool _isGlobalCooldown; // 是否gcd结束
    public bool _isCasting; // 是否释放技能中
    private float globalCooldownTime; // gcd

    public SkillInstance _castingSkill = null;
    private Dictionary<KeyCode, List<SkillInstance>> _skills = new Dictionary<KeyCode, List<SkillInstance>>();
}