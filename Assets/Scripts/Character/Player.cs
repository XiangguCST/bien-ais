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
        _attr.maxNeiLi = 10;
        _attr.atk = 500;
        _attr.speed = 100;
    }

    public void OnSkillEffect(Skill skill)
    {
        // 停止移动
        Stand();

        if (skill._name == "普攻")
        {
            _animator.SetTrigger("attack");
        }
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

    public override void TakeDamage(int damage)
    {
        base.TakeDamage(damage);
        _playerUI.UpdateUI();
    }

    public SkillBar _skillBar;
    PlayerUI _playerUI;
}
