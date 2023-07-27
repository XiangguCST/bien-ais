using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 角色状态管理类
/// </summary>
public class CharacterStatusManager
{
    private struct StatusInfo
    {
        public float Duration;  // 异常状态持续时间
        public float StartTime;  // 异常状态开始时间
    }

    // 添加异常状态
    public void AddStatus(CharacterStatusType status, float duration)
    {
        if (status == CharacterStatusType.None) return;

        StatusInfo newStatusInfo = new StatusInfo
        {
            Duration = duration,
            StartTime = gameTime,
        };

        statusInfos[status] = newStatusInfo;
        UpdateCurrentStatus();
        OnCharacterStatusEffect?.Invoke(status);
    }

    // 移除所有异常状态
    public void RemoveAllStatuses()
    {
        statusInfos.Clear();
        UpdateCurrentStatus();
    }

    // 更新异常状态
    public void Update(float deltaTime)
    {
        gameTime += deltaTime;

        List<CharacterStatusType> statusesToRemove = new List<CharacterStatusType>();

        foreach (var statusInfo in statusInfos)
        {
            StatusInfo updatedStatusInfo = new StatusInfo
            {
                Duration = statusInfo.Value.Duration - deltaTime,
                StartTime = statusInfo.Value.StartTime
            };

            if (updatedStatusInfo.Duration <= 0)
            {
                statusesToRemove.Add(statusInfo.Key);
            }
            else
            {
                statusInfos[statusInfo.Key] = updatedStatusInfo;
            }
        }

        foreach (var status in statusesToRemove)
        {
            statusInfos.TryRemove(status, out _);
        }

        UpdateCurrentStatus();
        DebugGUI.AddDebugTotalObject(_owner.name,this,Color.green);
        DebugGUI.AddDebugItem(_owner.name + "_hp", _owner._hp.ToString(), Color.blue);
        DebugGUI.AddDebugItem(_owner.name + "_energy", _owner._energy.ToString(), Color.red);
    }

    // 获取上一次添加异常状态经过的时间
    public float GetStatusElapsedTime()
    {
        return GetCurrentStatus() == CharacterStatusType.None ? 0 : gameTime - lastStatusStartTime;
    }

    // 更新当前异常状态
    private void UpdateCurrentStatus()
    {
        var newStatus = CharacterStatusType.None;
        foreach (var status in statusInfos.Keys)
        {
            if (newStatus == CharacterStatusType.None || status < newStatus)
            {
                newStatus = status;
            }
        }
        if (newStatus != currentStatus)
        {
            currentStatus = newStatus;
            if (statusInfos.ContainsKey(currentStatus))
            {
                lastStatusStartTime = statusInfos[currentStatus].StartTime;
            }
            OnStatusChanged?.Invoke();
        }
    }

    // 获取当前异常状态
    public CharacterStatusType GetCurrentStatus()
    {
        return currentStatus;
    }

    private ConcurrentDictionary<CharacterStatusType, StatusInfo> statusInfos = new ConcurrentDictionary<CharacterStatusType, StatusInfo>();  // 存储每种状态的信息
    private float lastStatusStartTime; // 最后一次添加异常状态的开始时间
    private float gameTime; // 游戏运行时间
    private CharacterStatusType currentStatus;  // 当前异常状态

    public Character _owner; // 当前角色
    public Action<CharacterStatusType> OnCharacterStatusEffect; // 角色受到异常状态的事件
    public Action OnStatusChanged; // 角色异常状态发生变化的事件
}
