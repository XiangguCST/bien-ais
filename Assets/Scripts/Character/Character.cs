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
        InitCharacter();
    }

    public void InitCharacter()
    {
        lock (_lockObject)
        {
            if (_bInit) return;
            _rb = GetComponent<Rigidbody2D>();
            _collider = GetComponent<CapsuleCollider2D>();
            _animator = GetComponentInChildren<Animator>();
            _skillMgr = new CharacterSkillMgr(this);
            _targetFinder = new TargetFinder(this);
            _stateManager.OnCharacterStatusEffect += OnCharacterStatusEffect; 
            _stateManager._owner = this;
            lastPosition = transform.position;
            CommonUtility.SetCharacterColor(this, _defaultColor);
            var damageCanvas = Resources.Load<GameObject>("Prefabs/UI/DamageCanvas");
            _damageCanvas = Instantiate(damageCanvas).GetComponent<DamageCanvas>();
            _damageCanvas.transform.SetParent(transform);
            _damageCanvas.transform.localPosition = Vector2.zero;
            _damageCanvas._owner = this;

            InitAttribute();
            ApplyAttribute();
            _bInit = true;
        }
    }

    /// <summary>
    /// 响应角色受到异常状态
    /// </summary>
    private void OnCharacterStatusEffect()
    {
        Stand();// 异常状态角色应该停下
        ShowStatus(_stateManager.GetCurrentStatus().GetDescription()); // 显示控制字体
        _skillMgr.InterruptSkill(); // 打断技能释放
        _animator.SetTrigger("status");
        _skillMgr.DisableAllChainSkills(); // 角色受到异常状态禁用所有连锁技能
    }

    private void Update()
    {
        if (InputController.Instance._isGameOver) return;

        _targetFinder.UpdateTarget();
        _stateManager.UpdateStatus(Time.deltaTime);
        _buffManager.UpdateBuffs(Time.deltaTime);

        if (transform.position != lastPosition)
        {
            OnCharacterPositionChanged(lastPosition, transform.position);
        }
    }

    /// <summary>
    /// 绑定切换技能
    /// </summary>
    public void AttachToggleSkill(KeyCode hotKey, Skill skill, KeyCode toggleKey, bool bDoubleClick = false)
    {
        _skillMgr.AttachToggleSkill(hotKey, skill, toggleKey, bDoubleClick);
    }

    public void AttachSkill(KeyCode keypad1, Skill skill, bool bDoubleClick = false)
    {
        _skillMgr.AttachSkill(keypad1, skill, bDoubleClick);
    }

    public void ApplyAllSkills()
    {
        _skillMgr.ApplyAllSkills();
    }

    protected void OnCharacterPositionChanged(Vector3 lastPosition, Vector3 nowPosition)
    {
        transform.position = WorldLimit.CheckLimitPos(nowPosition);
    }

    // 人物移动
    virtual public void Move(CharacterDir dir)
    {
        if (_bDie)
            return;

        if (_skillMgr != null && _skillMgr._isCasting)
        {
             if(!_skillMgr._castingSkill.SkillInfo.HasComponent<AllowMovementDuringSkill>())
            {
                return;
            }
        }

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

    /// <summary>
    /// 翻转方向
    /// </summary>
    public void FlipDirection()
    {
        switch (GetDirection())
        {
            case CharacterDir.eLeft:
                SetDirection(CharacterDir.eRight);
                break;
            case CharacterDir.eRight:
            default:
                SetDirection(CharacterDir.eLeft);
                break;
        }
    }

    /// <summary>
    /// 设置朝向
    /// </summary>
    public void SetDirection(CharacterDir dir)
    {
        _dir = dir;
        UpdateDirShow(); // 翻转方向后，更新方向的显示
    }

    /// <summary>
    /// 获取朝向
    /// </summary>
    public CharacterDir GetDirection()
    {
        return _dir;
    }

    virtual public void Start()
    {
        _rb.freezeRotation = true;
    }

    virtual protected void Die()
    {
        _bDie = true;
        InputController.Instance.SetGameOver();
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        _targetFinder.OnDestroy();
    }

    virtual public void TakeDamage(int damage, bool bCritical = false)
    {
        _damageCanvas.ShowDamage(damage, bCritical);
        _hp = Mathf.Clamp(_hp - damage, 0, _attr.maxHP);
        if (_hp <= 0)
        {
            Die();
        }
    }

    virtual public void ShowStatus(string statusTip)
    {
        _damageCanvas.ShowStatus(statusTip);
    }

    // 消耗内力
    virtual public void ConsumeEnergy(int cost)
    {
        _energy = Mathf.Clamp(_energy - cost, 0, _attr.maxEnergy);
    }

    public void OnSkillEffect(SkillInstance skill)
    {
        // 停止移动
        Stand();
    }

    public int _hp; // 血量
    public int _energy; // 内力
    protected CharacterDir _dir; // 朝向
    public bool _bIsMoving;// 是否移动中
    protected bool _bDie = false; // 是否死亡
    private Vector3 lastPosition;
    public Color _defaultColor = Color.black; // 角色默认颜色
    [SerializeField]
    public CharacterAttribute _attr = new CharacterAttribute(); // 角色属性

    protected Rigidbody2D _rb;
    protected CapsuleCollider2D _collider;
    protected CharacterSkillMgr _skillMgr;
    protected Animator _animator;
    protected DamageCanvas _damageCanvas;
    public TargetFinder _targetFinder;
    public CharacterStatusManager _stateManager = new CharacterStatusManager();
    public CharacterBuffManager _buffManager = new CharacterBuffManager();

    private object _lockObject = new object();
    private bool _bInit = false;
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