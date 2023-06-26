using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 技能格子
/// </summary>
public class SkillSlot : MonoBehaviour
{
    private void Start()
    {
        _imgMask = transform.Find("Mask").GetComponent<Image>();
        _txtCooldown = transform.Find("Text").GetComponent<Text>();
    }

    private void Update()
    {
        if (_skill == null) return;
        if(_skill._bIsCooldown)
        {
            _imgMask.fillAmount = Mathf.Lerp(0, 1, _skill._cooldownTimer / _skill._cooldownTime);
            _txtCooldown.text = _skill._cooldownTimer.ToString("f1");
        }
    }

    // 初始化技能格子
    public void InitSlot(KeyCode hotKey, Skill skill)
    {
        if (skill == null)
            Debug.Log("技能不存在，未成功绑定到技能格子");

        _skill = skill;
        _hotKey = hotKey;
    }

    public Skill _skill; // 技能
    public KeyCode _hotKey; // 绑定快捷键
    Image _imgMask; // 遮挡
    Text _txtCooldown; // 冷却时间
}
