using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 搜索敌人类
/// </summary>
public class TargetFinder
{
    public readonly float MaxFindDistance = 16f; // 最大发现目标距离
    private bool _isFindTarget; // 是否发现目标
    private float _nearestDistance; // 距离目标距离

    private Character _owner; // 当前角色
    private Character _nearestEnemy; // 最近敌人
    private TargetUI _targetUI;
    private CharacterPool _characterPool; // 角色对象池

    public TargetFinder(Character owner)
    {
        _owner = owner;
        _characterPool = CharacterPool.Instance;

        // 订阅事件
        _characterPool.OnCharacterPoolInited += Init;
    }

    public void Dispose()
    {
        // 取消订阅事件
        _characterPool.OnCharacterPoolInited -= Init;
    }


    public void Init()
    {
        InitUI();
        InitTarget();
    }

    // 判断是否面朝目标
    private bool IsFaceToTarget()
    {
        if (_nearestEnemy == null)
            return false;

        var dirX = _nearestEnemy.transform.position.x - _owner.transform.position.x;
        return (_owner.GetDirection() == CharacterDir.eLeft && dirX < 0)
            || (_owner.GetDirection() == CharacterDir.eRight && dirX > 0);
    }

    // 初始化UI
    private void InitUI()
    {
        GameObject uiGO = GameObject.Instantiate(Resources.Load<GameObject>("Prefabs/UI/TargetUI"));
        _targetUI = uiGO.GetComponent<TargetUI>();
        _targetUI.Init();
    }

    // 更新目标
    public void UpdateTarget()
    {
        if (_nearestEnemy == null)
            return;

        _nearestDistance = Vector2.Distance(_owner.transform.position, _nearestEnemy.transform.position);
        _isFindTarget = IsFaceToTarget() && _nearestDistance <= MaxFindDistance;
        if (_isFindTarget)
        {
            _targetUI.UpdateDistance(_nearestDistance);
        }
        else
        {
            _targetUI.Hide();
        }
    }

    // 初始化目标
    private void InitTarget()
    {
        // 从角色池中获取所有敌人
        List<Character> enemys = _characterPool.GetEnemies(_owner);
        if (enemys.Count == 0)
            return;

        // 寻找最近的敌人
        Character target = enemys[0];
        float minDistance = Vector2.Distance(_owner.transform.position, enemys[0].transform.position);
        foreach (var enemy in enemys)
        {
            float distance = Vector2.Distance(_owner.transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                target = enemy;
                minDistance = distance;
            }
        }

        // 设置最近的敌人和UI目标
        _nearestEnemy = target;
        _targetUI.SetTarget(_nearestEnemy.transform);
        UpdateTarget();
    }

    // 是否有目标
    public bool HasTarget()
    {
        return _isFindTarget;
    }

    // 获取目标
    public Character GetTarget()
    {
        return _nearestEnemy;
    }

    // 获取目标距离
    public float GetDistance()
    {
        return _nearestDistance;
    }
}

