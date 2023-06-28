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
        _targetFinder._owner = this;
        _stateManager._owner = this;

        InitAttribute();
        ApplyAttribute();
    }

    private void Update()
    {
        _targetFinder.UpdateTarget();
        _stateManager.UpdateStatus(Time.deltaTime);
    }

    // 人物移动
    virtual public void Move(CharacterDir dir)
    {
        if (_bDie)
            return;
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
        if (_bDie)
            return;
        _rb.velocity = new Vector2(0, _rb.velocity.y);
        _bIsMoving = false;
        UpdateAnimation();
    }

    // 初始化属性（子类可重写以设置更多属性）
    virtual protected void InitAttribute()
    {
        _bIsMoving = false;
        if (_animator.transform.localScale.x < 0)
            _dir = CharacterDir.eLeft;
        else if (_animator.transform.localScale.x > 0)
            _dir = CharacterDir.eRight;

        _targetFinder.Init();
    }

    // 应用属性
    void ApplyAttribute()
    {
        _hp = _attr.maxHP;
        _energy = _attr.maxEnergy;
        UpdateDirShow();
    }

    // 更新动画显示
    void UpdateAnimation()
    {
        _animator.SetBool("isMoving", _bIsMoving);
    }

    public void TriggerAnimator(string param)
    {
        _animator.SetTrigger(param);
    }

    public void SetAnimatorInteger(string name, int value)
    {
        _animator.SetInteger(name, value);
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

    virtual protected void Die()
    {
        _bDie = true;
        InputController.Instance.SetGameOver(true);
        Destroy(gameObject);
    }

    virtual public void TakeDamage(int damage)
    {
        _hp = Mathf.Clamp(_hp - damage, 0, _attr.maxHP);
        if (_hp <= 0)
        {
            Die();
        }
    }

    // 消耗内力
    virtual public void ConsumeEnergy(int cost)
    {
        _energy = Mathf.Clamp(_energy - cost, 0, _attr.maxEnergy);
    }

    public int _hp; // 血量
    public int _energy; // 内力
    public CharacterDir _dir; // 朝向
    public bool _bIsMoving;// 是否移动中
    protected bool _bDie = false; // 是否死亡

    [SerializeField]
    public CharacterAttribute _attr = new CharacterAttribute(); // 角色属性

    protected Rigidbody2D _rb;
    protected CapsuleCollider2D _collider;
    protected Animator _animator;
    public TargetFinder _targetFinder = new TargetFinder();
    [SerializeField]
    public CharacterStatusManager _stateManager = new CharacterStatusManager(); 
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
    public int maxEnergy; // 最大内力
    public int atk; // 攻击力
    public float speed; // 移动速度
}