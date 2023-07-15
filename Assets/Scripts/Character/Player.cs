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
    }

    public void InterruptSkill()
    {
        if (_skillMgr._isCasting)
        {
            var castingSkill = _skillMgr._castingSkill;
            if (castingSkill != null)
                castingSkill.InterruptSkill();
        }
    }

    public override void TakeDamage(int damage, bool bCritical = false)
    {
        base.TakeDamage(damage, bCritical);
        OnStatusBarsChanged?.Invoke(this);
    }

    public override void ConsumeEnergy(int cost)
    {
        base.ConsumeEnergy(cost);
        OnStatusBarsChanged?.Invoke(this);
    }

    public event Action<Character> OnStatusBarsChanged;
}
