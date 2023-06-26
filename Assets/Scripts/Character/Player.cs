using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YFrameWork;

public class Player : Character
{
    override protected void InitAttribute()
    {
        _attr.maxHP = 50000;
        _attr.maxNeiLi = 10;
        _attr.atk = 500;
        _attr.speed = 100;
    }

}
