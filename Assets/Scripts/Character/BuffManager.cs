using System.Collections.Generic;
using UnityEngine;

public abstract class Buff
{
    public string AbilityType { get; protected set; } // Buff的能力类型
    public DurationBuffType Type { get; protected set; } // Buff的持续类型
    public bool IsExpired { get; protected set; } // Buff是否已过期

    public abstract void Update();
}

public class DurationBuff : Buff
{
    private float duration;
    private float elapsedTime;

    public DurationBuff(string abilityType, float duration)
    {
        AbilityType = abilityType;
        Type = DurationBuffType.Duration;
        this.duration = duration;
        elapsedTime = 0f;
        IsExpired = false;
    }

    public override void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= duration)
        {
            IsExpired = true;
        }
    }
}

public class CountBuff : Buff
{
    private int remainingCount;

    public CountBuff(string abilityType, int count)
    {
        AbilityType = abilityType;
        Type = DurationBuffType.Count;
        remainingCount = count;
        IsExpired = false;
    }

    public override void Update()
    {
        // 不需要在此更新计数的逻辑，因为次数用完后Buff会自动消失
    }

    public void ConsumeCount()
    {
        remainingCount--;
        if (remainingCount <= 0)
        {
            IsExpired = true;
        }
    }
}

public class BuffManager
{
    private List<Buff> buffs; // 存储角色的Buff

    public BuffManager()
    {
        buffs = new List<Buff>();
    }

    // 添加Buff
    public void AddBuff(Buff buff)
    {
        buffs.Add(buff);
    }

    // 根据能力类型判断是否存在对应Buff
    public bool HasBuff(string abilityType)
    {
        foreach (var buff in buffs)
        {
            if (buff.AbilityType == abilityType)
            {
                return true;
            }
        }
        return false;
    }

    // 根据能力类型获取对应的Buff
    public Buff GetBuff(string abilityType)
    {
        foreach (var buff in buffs)
        {
            if (buff.AbilityType == abilityType)
            {
                return buff;
            }
        }
        return null;
    }

    // 减少能力类型对应Buff的次数
    public void DecreaseBuffCount(string abilityType)
    {
        foreach (var buff in buffs)
        {
            if (buff.AbilityType == abilityType && buff.Type == DurationBuffType.Count)
            {
                var countBuff = buff as CountBuff;
                countBuff?.ConsumeCount();
                break;
            }
        }
    }

    // 更新Buff状态
    public void UpdateBuffs()
    {
        for (int i = buffs.Count - 1; i >= 0; i--)
        {
            buffs[i].Update();

            if (buffs[i].IsExpired)
            {
                buffs.RemoveAt(i);
            }
        }
    }
}

public enum DurationBuffType
{
    Duration, // 表示根据持续时间的Buff
    Count // 表示根据固定次数的Buff
}

public enum AbilityBuffType
{
    Dodge, // 闪避能力类型的Buff
    Resistance // 抵抗能力类型的Buff
}
