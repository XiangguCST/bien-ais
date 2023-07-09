using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

public class CharacterBuffManager
{
    public CharacterBuffManager()
    {
        buffTimers = new ConcurrentDictionary<BuffType, float>();
        buffCounts = new ConcurrentDictionary<BuffType, int>();
    }

    public void AddBuffTime(BuffType buffType, float duration)
    {
        if (buffTimers.TryGetValue(buffType, out float remainingDuration))
        {
            duration = Math.Max(duration, remainingDuration);
        }

        buffTimers.AddOrUpdate(buffType, duration, (_, existingDuration) => Math.Max(existingDuration, duration));
    }

    public void AddBuffCount(BuffType buffType, int count)
    {
        buffCounts.AddOrUpdate(buffType, count, (_, existingCount) => existingCount + count);
    }

    public void DecreaseBuffCount(BuffType buffType, int count)
    {
        if (buffCounts.TryGetValue(buffType, out int existingCount))
        {
            int newCount = Math.Max(existingCount - count, 0);
            if (newCount == 0)
            {
                buffCounts.TryRemove(buffType, out _);
            }
            else
            {
                buffCounts[buffType] = newCount;
            }
        }
    }

    public bool HasBuff(BuffType buffType)
    {
        return buffTimers.ContainsKey(buffType) || buffCounts.ContainsKey(buffType);
    }

    public void UpdateBuffs(float deltaTime)
    {
        List<BuffType> buffsToRemove = null;
        foreach (var kvp in buffTimers)
        {
            var buffType = kvp.Key;
            buffTimers[buffType] -= deltaTime;
            if (buffTimers[buffType] <= 0f)
            {
                if (buffsToRemove == null)
                {
                    buffsToRemove = new List<BuffType>();
                }
                buffsToRemove.Add(buffType);
            }
        }

        if (buffsToRemove != null)
        {
            foreach (var buffType in buffsToRemove)
            {
                buffTimers.TryRemove(buffType, out _);
            }
        }
    }

    private ConcurrentDictionary<BuffType, float> buffTimers; // buff计时器
    private ConcurrentDictionary<BuffType, int> buffCounts; // buff计数器
}

public enum BuffType
{
    ImmunityAll,           // 免疫所有
    Evasion,               // 闪避
    ImmunityStatusEffect,   // 免疫异常状态
    ShadowClone, // 替身
}







