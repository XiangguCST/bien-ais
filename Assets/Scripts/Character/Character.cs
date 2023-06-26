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
        _meshRender = GetComponentInChildren<MeshRenderer>();

        InitAttribute();
        ApplyAttribute();
    }

    // 人物移动
    public void Move(float inputX)
    {
        _rb.velocity = new Vector2(inputX * _attr.speed * 0.03f, _rb.velocity.y);
        if(inputX < 0)
            _attr.dir = CharacterAttribute.Dir.eLeft;
        else if(inputX > 0)
            _attr.dir = CharacterAttribute.Dir.eRight;
        UpdateDirShow();
    }

    // 初始化属性（子类可设置不同属性）
    virtual protected void InitAttribute()
    {
        
    }

    // 应用属性
    void ApplyAttribute()
    {
        _attr.hp = _attr.maxHP;
        _attr.neiLi = _attr.maxNeiLi;
        UpdateDirShow();
    }

    // 刷新方向显示
    void UpdateDirShow()
    {
        Vector3 localScale = _meshRender.transform.localScale;
        if (_attr.dir == CharacterAttribute.Dir.eLeft)
        {
            localScale.x = -1;
        }
        else
        {
            localScale.x = 1;
        }
        _meshRender.transform.localScale = localScale;
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

    [SerializeField]
    public CharacterAttribute _attr = new CharacterAttribute();
    protected bool _bDie = false;

    protected Rigidbody2D _rb;
    protected CapsuleCollider2D _collider;
    protected Renderer _meshRender;
}

// 人物属性
[Serializable]
public struct CharacterAttribute
{
    public enum Dir
    {
        eLeft, // 朝左
        eRight, // 朝右
    }

    public int maxHP; // 最大血量
    public int maxNeiLi; // 最大内力
    public int hp; // 血量
    public int neiLi; // 内力
    public int atk; // 攻击力
    public float speed; // 移动速度
    public Dir dir; // 朝向
}