using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 搜索敌人类
/// </summary>
public class TargetFinder 
{
    public void Init()
    {
        InitUI();
        InitTarget();
    }

    // 是否面朝目标
    bool IsFaceToTarget()
    {
        var dirX = _nearestEnemy.transform.position.x - _owner.transform.position.x;
        return (_owner._dir == CharacterDir.eLeft && dirX < 0
            || _owner._dir == CharacterDir.eRight && dirX > 0);
    }

    void InitUI()
    {
        GameObject uiGO = GameObject.Instantiate(Resources.Load<GameObject>("UI/TargetUI"));
        _targetUI = uiGO.GetComponent<TargetUI>();
        _targetUI.Init();
    }

    public void UpdateTarget()
    {
        if (_nearestEnemy == null) return;

        _nearestDistance = Vector2.Distance(_owner.transform.position, _nearestEnemy.transform.position);
        _isFindTarget = IsFaceToTarget() && _nearestDistance <= _maxFindDistance;
        if(_isFindTarget)
        {
            _targetUI.UpdateDistance(_nearestDistance);
        }
        else
        {
            _targetUI.Hide();
        }
    }

    void InitTarget()
    {
        // 找到场景中所有的敌人
        var enemys = GameObject.FindObjectsOfType<Character>();
        foreach (var enemy in enemys)
        {
            if (enemy != _owner)
                _enemys.Add(enemy);
        }

        if (_enemys.Count == 0)
            return;
        Character target = _enemys[0];
        float minDistance = Vector2.Distance(_owner.transform.position, _enemys[0].transform.position);
        foreach (var enemy in _enemys)
        {
            float distance = Vector2.Distance(_owner.transform.position, enemy.transform.position);
            if (distance < minDistance)
            {
                target = enemy;
                minDistance = distance;
            }
        }
        _nearestEnemy = target;
        _targetUI.SetTarget(_nearestEnemy.transform);
        UpdateTarget();
    }

    public float _maxFindDistance; // 最大发现目标距离

    public bool _isFindTarget; // 是否发现目标
    public float _nearestDistance; // 距离目标距离

    public Character _owner; 
    public Character _nearestEnemy; // 最近敌人
    public List<Character> _enemys = new List<Character>(); // 敌人列表
    private TargetUI _targetUI;
}
