using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YFrameWork;

public class Player : Character
{
    override protected void InitAttribute()
    {
        base.InitAttribute();
        _attr.maxHP = 50000;
        _attr.maxEnergy = 10;
        _attr.atk = 500;
        _attr.speed = 100;
        _targetFinder._maxFindDistance = 6f;
    }

    public void OnSkillEffect(Skill skill)
    {
        // 停止移动
        Stand();
    }

    override public void Move(CharacterDir dir)
    {
        if (_skillBar == null || !_skillBar._isCasting)
        {
            base.Move(dir);
        }
    }

    public void SetPlayerUI(PlayerUI ui)
    {
        _playerUI = ui;
    }

    public void InterruptSkill()
    {
        if (_skillBar._isCasting)
        {
            var castingSkill = _skillBar._castingSkill;
            if (castingSkill != null)
                castingSkill.InterruptSkill();
        }
    }

    public override void TakeDamage(int damage, bool bCritical = false)
    {
        base.TakeDamage(damage, bCritical);
        _playerUI.UpdateUI();
    }

    public override void ConsumeEnergy(int cost)
    {
        base.ConsumeEnergy(cost);
        _playerUI.UpdateUI();
    }

    public SkillBar _skillBar;
    PlayerUI _playerUI;
}
