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
        _characterPool.OnSceneLoaded += Init;
    }

    public void Dispose()
    {
        // 取消订阅事件
        _characterPool.OnSceneLoaded -= Init;
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

/// <summary>
/// 角色对象池
/// </summary>
public class CharacterPool
{
    private static CharacterPool _instance;

    // 定义一个事件，在场景加载完成时触发
    public event Action OnSceneLoaded;

    // 单例模式
    public static CharacterPool Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CharacterPool();
            }
            return _instance;
        }
    }

    private List<Character> _characters = new List<Character>();

    private CharacterPool()
    {
        // 创建一个新的GameObject，附加一个SceneListener对象来侦听场景加载事件
        new GameObject("CharacterPoolScript", typeof(CharacterPoolScript));

        // 在新的CharacterPool实例创建时，注销旧实例的事件
        OnSceneLoaded = null;
    }

    // 添加角色到对象池
    public void AddCharacter(Character character)
    {
        if (!_characters.Contains(character))
        {
            _characters.Add(character);
        }
    }

    // 从对象池中移除角色
    public void RemoveCharacter(Character character)
    {
        if (_characters.Contains(character))
        {
            _characters.Remove(character);
        }
    }

    // 获取指定角色的所有敌人
    public List<Character> GetEnemies(Character character)
    {
        List<Character> enemies = new List<Character>();
        foreach (var chara in _characters)
        {
            if (chara != character)
            {
                enemies.Add(chara);
            }
        }
        return enemies;
    }

    // 清空对象池
    private void ClearPool()
    {
        _characters.Clear();
    }

    // 当场景加载时，获取所有的Character对象
    private class CharacterPoolScript : MonoBehaviour
    {
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            CharacterPool.Instance.ClearPool();
            var charactersInScene = FindObjectsOfType<Character>();
            foreach (Character character in charactersInScene)
            {
                CharacterPool.Instance.AddCharacter(character);
            }

            // 触发事件，通知场景已经加载完成
            CharacterPool.Instance.OnSceneLoaded?.Invoke();
        }
    }
}