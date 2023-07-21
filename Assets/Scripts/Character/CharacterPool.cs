using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YFrameWork;

/// <summary>
/// 角色对象池
/// </summary>
public class CharacterPool : MonoSingleton<CharacterPool>
{
    // 定义一个事件，在场景加载完成时触发
    public event Action OnCharacterPoolInited;

    private List<Character> _characters = new List<Character>();

    private CharacterPool()
    {

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

    private void Start()
    {
        Init();
    }

    private void Init()
    {
        CharacterPool.Instance.ClearPool();
        var charactersInScene = GameObject.FindObjectsOfType<Character>();
        foreach (Character character in charactersInScene)
        {
            CharacterPool.Instance.AddCharacter(character);
        }

        // 触发事件，通知角色池已经加载完成
        CharacterPool.Instance.OnCharacterPoolInited?.Invoke();
    }

}