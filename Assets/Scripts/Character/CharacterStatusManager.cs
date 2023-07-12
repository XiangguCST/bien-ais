using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

/// <summary>
/// 角色状态管理类
/// </summary>
[Serializable]
public class CharacterStatusManager
{
    public CharacterStatusManager()
    {
        statusTimers = new ConcurrentDictionary<CharacterStatusType, float>();
        currentStatus = CharacterStatusType.None;
    }

    // 添加异常状态
    public void AddStatus(CharacterStatusType status, float duration)
    {
        if (status == CharacterStatusType.None) return;

        _owner.ShowStatus(status.GetDescription()); // 显示控制字体

        statusTimers.AddOrUpdate(status, duration, (_, existingDuration) => Math.Max(existingDuration, duration));
        UpdateCurrentStatus();
        OnStatusEffectApplied?.Invoke();
    }

    // 移除所有异常状态
    public void RemoveAllStatuses()
    {
        statusTimers.Clear();
        UpdateCurrentStatus();
    }

    // 更新异常状态
    public void UpdateStatus(float deltaTime)
    {
        List<CharacterStatusType> statusesToRemove = null;
        foreach (var item in statusTimers)
        {
            var status = item.Key;
            statusTimers[status] -= deltaTime;
            if (statusTimers[status] <= 0f)
            {
                if (statusesToRemove == null)
                {
                    statusesToRemove = new List<CharacterStatusType>();
                }
                statusesToRemove.Add(status);
            }
        }

        if (statusesToRemove != null)
        {
            foreach (var status in statusesToRemove)
            {
                statusTimers.TryRemove(status, out _);
            }
        }

        UpdateCurrentStatus();
    }

    // 获取当前异常状态
    public CharacterStatusType GetCurrentStatus()
    {
        return currentStatus;
    }

    // 更新当前异常状态
    private void UpdateCurrentStatus()
    {
        currentStatus = CharacterStatusType.None;
        foreach (var status in statusTimers.Keys)
        {
            if (currentStatus == CharacterStatusType.None || status < currentStatus)
            {
                currentStatus = status;
            }
        }

        if(currentStatus != CharacterStatusType.None)
        {
            // 异常状态角色应该停下
            _owner.Stand();
        }
        _owner.SetAnimatorInteger("state", (int)currentStatus);
    }
    private ConcurrentDictionary<CharacterStatusType, float> statusTimers;  // 异常状态计时器
    public CharacterStatusType currentStatus;  // 当前异常状态
    public Character _owner; // 当前角色
    public Action OnStatusEffectApplied; // 角色受到异常状态的事件
}

public enum CharacterStatusType
{
    [Description("无异常状态")]
    None,
    [Description("沉默状态")]
    Silence,
    [Description("眩晕")]
    Stun,
    [Description("虚弱")]
    Weakness,
    [Description("击倒")]
    Knockdown
}


