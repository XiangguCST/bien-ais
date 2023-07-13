using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBuffAdditionStrategy
{
    void BeforeSkillCast(Character owner, SkillInstance skill);
    BuffType? BuffType { get; }
}

public class AddBuffDuration : IBuffAdditionStrategy
{
    public AddBuffDuration(BuffType buffType, float duration = -1)
    {
        _buffType = buffType;
        _duration = duration;
    }

    public void BeforeSkillCast(Character owner, SkillInstance skill)
    {
        owner._buffManager.AddBuffTime(_buffType, _duration == -1 ? skill.SkillInfo._castTime : _duration);
    }

    public BuffType? BuffType => _buffType;

    private BuffType _buffType;
    public float _duration;
}

public class AddBuffCount : IBuffAdditionStrategy
{
    public AddBuffCount(BuffType buffType, int count)
    {
        _buffType = buffType;
        _count = count;
    }

    public void BeforeSkillCast(Character owner, SkillInstance skill)
    {
        owner._buffManager.AddBuffCount(_buffType, _count);
    }

    public BuffType? BuffType => _buffType;

    BuffType _buffType;
    int _count;
}

public class DoNotAddBuff : IBuffAdditionStrategy
{
    public void BeforeSkillCast(Character owner, SkillInstance skill)
    {
    }

    public BuffType? BuffType => null;
}
