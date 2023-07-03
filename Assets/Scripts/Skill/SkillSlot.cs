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
        // 确保Material正确加载
        _grayScaleMaterial = Resources.Load<Material>("Materials/matGray");
        if (_grayScaleMaterial == null)
        {
            Debug.LogError("Failed to load GrayScale Material. Please make sure the material exists in Resources/Materials/ folder.");
        }

        _imgIcon = transform.Find("Icon").GetComponent<Image>();
        _imgOutline = transform.Find("Icon/OutLine").GetComponent<Image>();
        _imgMask = transform.Find("Icon/Mask").GetComponent<Image>();
        _txtCoolDown = transform.Find("Icon/CoolDown").GetComponent<Text>();
        _txtHotKey = transform.Find("Icon/HotKey").GetComponent<Text>();
        _txtName = transform.Find("SkillName").GetComponent<Text>();
        _txtName.text = "";
        UpdateHotKeyDisplay();

        _bInit = true;
    }

    void UpdateHotKeyDisplay()
    {
        // 设置快捷键显示
        switch (_hotKey)
        {
            case KeyCode.Keypad0:
            case KeyCode.Keypad1:
            case KeyCode.Keypad2:
            case KeyCode.Keypad3:
            case KeyCode.Keypad4:
            case KeyCode.Keypad5:
            case KeyCode.Keypad6:
            case KeyCode.Keypad7:
            case KeyCode.Keypad8:
            case KeyCode.Keypad9:
                _txtHotKey.text = (_hotKey - KeyCode.Keypad0).ToString();
                break;
            case KeyCode.UpArrow:
                _txtHotKey.text = "↑";
                break;
            case KeyCode.DownArrow:
                _txtHotKey.text = "↓";
                break;
            case KeyCode.LeftArrow:
                _txtHotKey.text = "←";
                break;
            case KeyCode.RightArrow:
                _txtHotKey.text = "→";
                break;
            default:
                _txtHotKey.text = _hotKey.ToString();
                break;
        }
    }

    private void Update()
    {
        if (InputController.Instance._isGameOver) return;

        if (!_bInit) InitSkillSlot();

        // 检测快捷键是否被按下
        if (Input.GetKey(_hotKey))
        {
            Color newColor = Color.cyan; 
            newColor.a = _imgOutline.color.a;
            _imgOutline.color = newColor;// 按键按下时，边框变为天蓝色
            _imgIcon.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f); // 缩小图标以模拟按下效果
        }
        else
        {
            Color newColor = Color.white; 
            newColor.a = _imgOutline.color.a;
            _imgOutline.color = newColor; // 按键没有被按下时，边框变为白色
            _imgIcon.transform.localScale = Vector3.one; // 恢复图标大小
        }

        if (_skill == null)
        {
            _imgMask.fillAmount = 0;
            _imgIcon.material = _grayScaleMaterial; // 技能不可用时，图标颜色为灰色
            return;
        }

        if (_skill.IsSkillUsableIgnoringCooldown())
        {
            _imgIcon.material = null; // 技能可用时，图标颜色为原色
        }
        else
        {
            _imgIcon.material = _grayScaleMaterial; // 技能不可用时，图标颜色为灰色
        }

        if (_skill._bIsCooldown)
        {
            // 更新技能格子显示
            _imgMask.fillAmount = Mathf.Lerp(0, 1, _skill._cooldownTimer / _skill._cooldownTime);
            _txtCoolDown.text = Mathf.Ceil(_skill._cooldownTimer).ToString(); // 显示冷却时间，如果需要保留小数，可适当调整
        }
        else
        {
            _txtCoolDown.text = ""; // 技能可用不显示冷却时间
        }
    }

    public void SetSkill(Skill skill)
    {
        if (!_bInit) InitSkillSlot();
        if (skill == null)
        {
            return;
        }
        _skill = skill;
        _txtName.text = _skill._name;
    }

    public Skill GetSkill()
    {
        return _skill;
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
            Debug.Log(_skill._name + "暂时无法使用");
        }
    }

    public KeyCode _hotKey; // 绑定快捷键
    [SerializeField]
    public Skill _skill; // 技能
    Image _imgOutline; // 外边框
    Image _imgIcon; // 图标
    Image _imgMask; // 遮挡
    Text _txtCoolDown; // 冷却显示
    Text _txtHotKey; // 快捷键显示
    Text _txtName; // 技能名称
    public Material _grayScaleMaterial; // 灰度材质
    bool _bInit = false;
}
