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

    // 开始gcd
    public void StartGlobalCooldown(float cooldownTime)
    {
        if (!_isGlobalCooldown)
        {
            _isGlobalCooldown = true;
            globalCooldownTime = cooldownTime;
            CoroutineRunner.Instance.StartCoroutine(GlobalCooldownRoutine());
        }
    }

    // gcd过程
    private IEnumerator GlobalCooldownRoutine()
    {
        yield return new WaitForSeconds(globalCooldownTime);
        _isGlobalCooldown = false;
    }

    // 添加技能
    public void AddSkill(KeyCode hotKey, Skill skill)
    {
        if (_addList.ContainsKey(hotKey)) return;
        _addList.Add(hotKey, skill);
    }

    // 应用技能
    public void ApplySkills(Player owner)
    {
        foreach (var pair in _addList)
        {
            KeyCode hotKey = pair.Key;
            Skill skill = pair.Value;
            skill._owner = owner;
            skill._skillbar = this;
            // 在技能栏下新建格子
            GameObject slotGO = Instantiate(Resources.Load<GameObject>("UI/SkillSlot"));
            slotGO.transform.SetParent(this.transform); 
            SkillSlot slot = slotGO.GetComponent<SkillSlot>();
            slot.InitSlot(hotKey, skill);
            _skills.Add(slot);
        }
        _owner = owner;
        _owner._skillBar = this;
        _addList.Clear();
    }

    public void ActivateSkill(Skill skill)
    {
        Debug.Log("使用了" + skill._name);

        if (skill == null)
        {
            Debug.LogError("Skill not found: " + skill._name);
            return;
        }
        if (skill.IsSkillUsable())
        {
            _castingSkill = skill;
            // 调用技能效果的方法
            skill.ActivateEffect();
        }
        else
        {
            Debug.Log(skill._name + "暂时无法使用");
        }
    }

    public Skill GetCastingSkill()
    {
        return _castingSkill;
    }

    public bool _isGlobalCooldown; // 是否gcd结束
    public bool _isCasting; // 是否释放技能中
    private float globalCooldownTime; // gcd

    private Dictionary<KeyCode, Skill> _addList = new Dictionary<KeyCode, Skill>(); // 技能添加列表
    public List<SkillSlot> _skills = new List<SkillSlot>(); // 技能列表
    private Player _owner;
    public Skill _castingSkill;
}