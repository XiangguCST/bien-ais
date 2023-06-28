using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 搜索敌人类
/// </summary>
public class TargetFinder : MonoBehaviour
{
    public void Init()
    {
        _owner = GetComponent<Character>();

        InitUI();
        InitTarget();
    }

    // 是否面朝目标
    bool IsFaceToTarget()
    {
        var dirX = _nearestEnemy.transform.position.x - transform.position.x;
        return (_owner._dir == CharacterDir.eLeft && dirX < 0
            || _owner._dir == CharacterDir.eRight && dirX > 0);
    }

    void InitUI()
    {
        GameObject uiGO = Instantiate(Resources.Load<GameObject>("UI/TargetUI"));
        _targetUI = uiGO.GetComponent<TargetUI>();
        _targetUI.Init();
    }

    void UpdateTarget()
    {
        _targetDistance = Vector2.Distance(transform.position, _nearestEnemy.transform.position);
        _isFindTarget = IsFaceToTarget() && _targetDistance <= _maxFindDistance;
        if(_isFindTarget)
        {
            _targetUI.UpdateDistance(_targetDistance);
        }
        else
        {
            _targetUI.Hide();
        }
    }

    void InitTarget()
    {
        if (_enemys.Count == 0)
            return;
        Character target = _enemys[0];
        float minDistance = Vector2.Distance(transform.position, _enemys[0].transform.position);
        foreach (var enemy in _enemys)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
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

    void Update()
    {
        if (_nearestEnemy == null) return;
        
        UpdateTarget();
    }

    public bool _isFindTarget; // 是否发现目标
    public float _targetDistance; // 距离目标距离
    public float _maxFindDistance; // 最大发现目标距离

    public Character _owner; 
    public Character _nearestEnemy; // 最近敌人
    public List<Character> _enemys = new List<Character>(); // 敌人列表
    private TargetUI _targetUI;
}
