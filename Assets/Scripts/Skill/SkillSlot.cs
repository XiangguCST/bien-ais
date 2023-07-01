using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能格子
/// </summary>
public class SkillSlot : MonoBehaviour
{
    private void Start()
    {
        _imgMask = transform.Find("Mask").GetComponent<Image>();
    }

    private void Update()
    {
        if (InputController.Instance._isGameOver) return;

        if (_skill == null)
        {
            _imgMask.fillAmount = 0;
            return;
        }
        if (_skill._bIsCooldown)
        {
            // 更新技能格子显示
            _imgMask.fillAmount = Mathf.Lerp(0, 1, _skill._cooldownTimer / _skill._cooldownTime);
        }
        else
        {
            _imgMask.fillAmount = _skill.IsSkillUsable() ? 0 : 1;
        }
    }
    public void SetSkill(Skill skill)
    {
        _skill = skill;
    }

    public Skill GetSkill()
    {
        return _skill;
    }

    public void Activate()
    {
        if (_skill == null)
        {
            return;
        }
        if (_skill.IsSkillUsable())
        {
            // 调用技能效果的方法
            _skill.Activate();
        }
        else
        {
            Debug.Log(_skill._name + "暂时无法使用");
        }
    }

    public KeyCode _hotKey; // 绑定快捷键
    [SerializeField]
    public Skill _skill; // 技能
    Image _imgMask; // 遮挡
}
