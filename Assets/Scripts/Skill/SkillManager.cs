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

    /// <summary>
    /// 停止释放技能
    /// </summary>
    public void InterruptSkill()
    {
        if (_isCasting && _castingSkill != null)
        {
            _castingSkill.InterruptSkill();
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

    /// <summary>
    /// 绑定切换技能
    /// </summary>
    public void AttachToggleSkill(KeyCode hotKey, Skill skill, KeyCode toggleKey, bool bDoubleClick = false)
    {
        var skillInstance = AttachSkill(hotKey, skill, bDoubleClick);
        if (skillInstance == null)
            return;
        skillInstance._bIsToggleSkill = true;
        skillInstance._toggleKey = toggleKey;
    }

    /// <summary>
    /// 绑定技能
    /// </summary>
    public SkillInstance AttachSkill(KeyCode hotKey, Skill skill, bool bDoubleClick = false)
    {
        if (skill == null) return null;
        var skillSlot = InputController.Instance.GetSkillSlotByHotKey(hotKey);
        if (skillSlot == null) return null;

        var skillInstance = new SkillInstance(skill);
        skillInstance._owner = _owner;
        skillInstance._bDoubleClick = bDoubleClick;
        skillInstance._skillManager = this;

        // 保存到优先级列表中
        if (!_skills.ContainsKey(hotKey))
        {
            _skills[hotKey] = new List<SkillInstance>();
        }
        _skills[hotKey].Add(skillInstance);
        _skills[hotKey] = _skills[hotKey].OrderByDescending(s => s.SkillInfo._usabilityPriority).ToList();
        skillSlot.SetSkill(GetActiveSkill(hotKey));
        return skillInstance;
    }


    public SkillInstance GetCastingSkill()
    {
        return _castingSkill;
    }

    /// <summary>
    /// 获取当前活跃技能:
    /// 如果替换键被按下，返回优先级最高的可用替换技能，
    /// 如果没有可用的替换技能，就返回优先级最低的替换技能（如果不存在替换技能就按照非替换技能的逻辑处理）
    /// 如果替换键没有被按下，返回优先级最高的可用非替换技能，
    /// 如果没有可用的非替换技能，就返回优先级最低的非替换技能
    /// </summary>
    /// <param name="hotKey"></param>
    /// <returns></returns>
    public SkillInstance GetActiveSkill(KeyCode hotKey)
    {
        // 如果hotKey不存在，立即返回null
        if (!_skills.ContainsKey(hotKey))
            return null;

        // 将所有技能分为替换技能和非替换技能两部分
        var toggleSkills = _skills[hotKey].Where(skill => skill._bIsToggleSkill).ToList();
        var nonToggleSkills = _skills[hotKey].Where(skill => !skill._bIsToggleSkill).ToList();

        // 如果替换键被按下且有替换技能
        if (toggleSkills.Any() && Input.GetKey(toggleSkills[0]._toggleKey))
            return GetUsableSkill(toggleSkills) ?? GetLowestPrioritySkill(toggleSkills);

        // 替换键没有被按下
        return GetUsableSkill(nonToggleSkills) ?? GetLowestPrioritySkill(nonToggleSkills);
    }

    private SkillInstance GetUsableSkill(List<SkillInstance> skills)
    {
        return skills.OrderByDescending(skill => skill.SkillInfo._usabilityPriority)
                     .FirstOrDefault(skill => skill.IsSkillUsable());
    }

    private SkillInstance GetLowestPrioritySkill(List<SkillInstance> skills)
    {
        return skills.Any() ? skills.OrderBy(skill => skill.SkillInfo._usabilityPriority).First() : null;
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

    /// <summary>
    /// 禁用所有连锁技能
    /// </summary>
    public void DisableAllChainSkills()
    {
        foreach (var pair in _skills)
        {
            foreach (var skill in pair.Value)
            {
                var strategy = skill.SkillInfo._chainStrategy;
                if (strategy.IsChainSkill() && !strategy.CanUseOtherSkillsBefore())
                {
                    skill.DisableChainSkill();
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