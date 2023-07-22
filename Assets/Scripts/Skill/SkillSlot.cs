using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能格子
/// </summary>
public class SkillSlot : MonoBehaviour
{
    private void InitSkillSlot()
    {
        _imgIcon = transform.Find("Icon").GetComponent<Image>();
        _imgMask = transform.Find("Icon/Mask").GetComponent<Image>();
        _txtCoolDown = transform.Find("Icon/CoolDown").GetComponent<Text>();
        _txtHotKey = transform.Find("Icon/HotKey").GetComponent<Text>();
        _txtName = transform.Find("SkillName").GetComponent<Text>();
        _txtName.text = "";
        _txtCoolDown.text = "";

        // 创建一个提示UI控制器
        _tipUIController = TooltipUIController<SkillInfoUI>.GetInstance("Prefabs/UI/SkillInfoUI");

        // 添加鼠标检测器
        _uIMouseHoverDetector = new UIMouseHoverDetector(_imgIcon.gameObject);
        _uIMouseHoverDetector.OnMouseHoverEnter += HandleMouseHoverEnter;
        _uIMouseHoverDetector.OnMouseHoverExit += HandleMouseHoverExit;
        _uIMouseHoverDetector.OnMouseHovering += HandleMouseHovering;
        UpdateHotKeyDisplay();

        // 给图标创建一个新的材质实例
        Material newMat = Instantiate(_imgIcon.material);
        _imgIcon.material = newMat;

        _bInit = true;
    }

    private void HandleMouseHovering()
    {
        // 更新UI位置至鼠标位置
        _tipUIController.UpdateUIPositionToMouse();
    }

    private void HandleMouseHoverEnter()
    {
        if (_skill == null) return;

        // 显示UI
        _tipUIController.ShowTooltipUI();
        SkillInfoUI skillInfoUI = _tipUIController.GetTooltipUIScript();
        skillInfoUI.UpdateUI(this);
    }

    private void HandleMouseHoverExit()
    {
        // 隐藏UI
        _tipUIController.HideTooltipUI();
    }

    void UpdateHotKeyDisplay()
    {
        _txtHotKey.text = CommonUtility.GetHotKeyString(_hotKey);
    }

    private void Update()
    {
        if (InputController.Instance._isGameOver) return;

        if (!_bInit) InitSkillSlot();

        // 按照技能优先级更新技能显示
        UpdateActiveSkill();

        // 检测快捷键是否被按下
        if (Input.GetKey(_hotKey))
        {
            _imgIcon.material.SetColor("_OutlineColor", Color.cyan); // 按键按下时，边框变为天蓝色
            _imgIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f); // 缩小图标以模拟按下效果
        }
        else
        {
            _imgIcon.material.SetColor("_OutlineColor", Color.white); // 按键没有被按下时，边框变为白色
            _imgIcon.transform.localScale = Vector3.one; // 恢复图标大小
        }

        if (_skill == null)
        {
            _imgMask.fillAmount = 0;
            _imgIcon.material.SetFloat("_GrayScale", 1); // 技能不可用时，图标颜色为灰色
            return;
        }

        if (_skill.IsSkillUsableIgnoringCooldown())
        {
            _imgIcon.material.SetFloat("_GrayScale", 0); // 技能可用时，图标颜色为原色
        }
        else
        {
            _imgIcon.material.SetFloat("_GrayScale", 1); // 技能不可用时，图标颜色为灰色
        }

        if (_skill._bIsCooldown)
        {
            _imgMask.fillAmount = Mathf.Lerp(0, 1, _skill._cooldownTimer / _skill.SkillInfo._cooldownTime);
            _txtCoolDown.text = Mathf.Ceil(_skill._cooldownTimer).ToString();
        }
        else
        {
            _txtCoolDown.text = ""; // 技能冷却结束不显示冷却时间
        }
    }

    // 更新活跃技能
    private void UpdateActiveSkill()
    {
        if (_skill == null) return;

        SkillInstance highestPrioritySkill = _skill._skillManager.GetActiveSkill(_hotKey);
        if (_skill != highestPrioritySkill)
        {
            SetSkill(highestPrioritySkill);
            
        }
    }

    // 冷却完成技能图标发光效果
    IEnumerator GlowEffect()
    {
        float duration = 0.15f; // 发光效果持续时间
        float elapsed = 0; // 已经过时间

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            _imgIcon.material.SetFloat("_GlowIntensity", Mathf.Lerp(0, 1, t)); // 插值到最大发光
            yield return null;
        }

        elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            _imgIcon.material.SetFloat("_GlowIntensity", Mathf.Lerp(1, 0, t)); // 插值回不发光
            yield return null;
        }

        _imgIcon.material.SetFloat("_GlowIntensity", 0); // 确保发光强度被设置回0
    }

    public void SetSkill(SkillInstance skill)
    {
        if (!_bInit) InitSkillSlot();
        if (skill == null)
        {
            return;
        }

        // 如果已经设置过技能，则先取消之前的事件订阅
        if (_skill != null)
        {
            _skill.OnCooldownCompleted -= () => StartCoroutine(GlowEffect());
        }

        _skill = skill;
        _txtName.text = _skill.SkillInfo._name;
        _imgMask.fillAmount = _skill._bIsCooldown ? Mathf.Lerp(0, 1, _skill._cooldownTimer / _skill.SkillInfo._cooldownTime) : 0;
        _txtCoolDown.text = _skill._bIsCooldown ? Mathf.Ceil(_skill._cooldownTimer).ToString() : "";

        // 加载Sprite资源
        var sprite = ResourcesLoader.LoadResource<Sprite>($"Sprites/SkillIcons/{_skill.SkillInfo._name}");
        if (sprite != null)
        {
            _imgIcon.sprite = sprite; // 设置图片
        }
        else
        {
            Debug.LogWarning($"Sprite for {_skill.SkillInfo._name} not found!");
            var defSprite = ResourcesLoader.LoadResource<Sprite>($"Sprites/SkillIcons/defSkillIcon");
            _imgIcon.sprite = defSprite; // 设置图片
        }

        // 检查冷却时间是否大于5秒
        if (_skill.SkillInfo._cooldownTime > 5.0f)
        {
            _skill.OnCooldownCompleted += StartGlowEffect; // 订阅事件
        }
    }

    void StartGlowEffect()
    {
        if (_imgIcon != null)
        {
            StartCoroutine(GlowEffect());
        }
    }

    public void Activate()
    {
        if (_skill == null)
        {
            return;
        }
        if (_skill.IsSkillUsable())
        {
            // 调用技能效果的方法
            _skill.Activate();
        }
        else
        {
            //Debug.Log(_skill.SkillInfo._name + "暂时无法使用");
        }
    }

    public KeyCode _hotKey; // 绑定快捷键
    [SerializeField]
    public SkillInstance _skill; // 技能
    [HideInInspector]
    public Image _imgIcon; // 图标
    Image _imgMask; // 遮挡
    Text _txtCoolDown; // 冷却显示
    Text _txtHotKey; // 快捷键显示
    Text _txtName; // 技能名称
    UIMouseHoverDetector _uIMouseHoverDetector;
    bool _bInit = false;
    TooltipUIController<SkillInfoUI> _tipUIController;
}
