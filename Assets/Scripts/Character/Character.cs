using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YFrameWork;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CapsuleCollider2D))]
public class Character : MonoBehaviour
{
    public void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _collider = GetComponent<CapsuleCollider2D>();
        _animator = GetComponentInChildren<Animator>();

        InitAttribute();
        ApplyAttribute();
    }

    // 人物移动
    public void Move(CharacterDir dir)
    {
        float inputX = 0;
        if (dir == CharacterDir.eLeft)
            inputX = -1;
        else if (dir == CharacterDir.eRight)
            inputX = 1;
        _rb.velocity = new Vector2(inputX * _attr.speed * 0.05f, _rb.velocity.y);
        _dir = dir;
        UpdateDirShow();
        _bIsMoving = true;
        UpdateAnimation();
    }

    // 人物站立（停止移动）
    public void Stand()
    {
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        _bIsMoving = false;
        UpdateAnimation();
    }

    // 初始化属性（子类可设置不同属性）
    virtual protected void InitAttribute()
    {
        _bIsMoving = false;
        if (_animator.transform.localScale.x < 0)
            _dir = CharacterDir.eLeft;
        else if (_animator.transform.localScale.x > 0)
            _dir = CharacterDir.eRight;
    }

    // 应用属性
    void ApplyAttribute()
    {
        _hp = _attr.maxHP;
        _neiLi = _attr.maxNeiLi;
        UpdateDirShow();
    }

    // 更新动画显示
    void UpdateAnimation()
    {
        _animator.SetBool("isMoving", _bIsMoving);
    }

    // 刷新方向显示
    void UpdateDirShow()
    {
        Vector3 localScale = _animator.transform.localScale;
        if (_dir == CharacterDir.eLeft)
        {
            localScale.x = -1;
        }
        else
        {
            localScale.x = 1;
        }
        _animator.transform.localScale = localScale;
    }

    virtual public void Start()
    {
        _rb.freezeRotation = true;
    }

    virtual public void GetDamage(Character other)
    {
       
    }

    virtual protected void Die()
    {
        Destroy(gameObject);
    }

    public int _hp; // 血量
    public int _neiLi; // 内力
    public CharacterDir _dir; // 朝向
    public bool _bIsMoving;// 是否移动中
    protected bool _bDie = false; // 是否死亡

    [SerializeField]
    public CharacterAttribute _attr = new CharacterAttribute(); // 角色属性

    protected Rigidbody2D _rb;
    protected CapsuleCollider2D _collider;
    protected Animator _animator;
}

// 人物朝向
public enum CharacterDir
{
    eLeft, // 朝左
    eRight, // 朝右
}

// 人物属性
[Serializable]
public struct CharacterAttribute
{
    public int maxHP; // 最大血量
    public int maxNeiLi; // 最大内力
    public int atk; // 攻击力
    public float speed; // 移动速度
}