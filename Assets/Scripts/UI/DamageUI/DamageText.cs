using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// 伤害文本类
public class DamageText : MonoBehaviour
{
    public DamageCanvas _owner; // 文本所属的画布
    private Text _txtDamage; // 文本组件
    private Vector2 _velocity; // 文本的运动速度
    private Vector3 _initialScale; // 文本的初始大小
    private float _timer = 0; // 计时器
    private const float DISPLAY_DURATION = 1f; // 斜抛效果文本显示的时间
    private const float SCALEEFFECT_DISPLAY_DURATION = 1.5f; // 放大效果文本显示的时间
    private float _scaleEffectWaitTime = 1f; // 放大效果等待的时间
    private const float GRAVITY = -4f; // 模拟重力，调整为适当的值以控制掉落速度

    public void Awake()
    {
        _txtDamage = GetComponent<Text>();
        _initialScale = _txtDamage.rectTransform.localScale; // 记录初始大小
    }

    // 设置文本颜色
    public void SetColor(Color color)
    {
        _txtDamage.color = color;
    }

    // 显示伤害
    public void ShowDamage(int damage, bool bCritical = false)
    {
        _txtDamage.text = damage.ToString() + (bCritical ? "暴击" : "");
        _txtDamage.enabled = true;
        _owner._showCnt++;
        _timer = 0;

        // 决定伤害文本的显示方式
        if (!bCritical || UnityEngine.Random.value < 0.66f)
        {
            // 斜抛运动方式
            _velocity = new Vector2(UnityEngine.Random.Range(-0.5f, 0.5f), UnityEngine.Random.Range(1.5f, 2f));
            Invoke("HideSelf", DISPLAY_DURATION);
        }
        else
        {
            // 放大然后消失的方式
            _velocity = Vector2.zero;
            _txtDamage.rectTransform.anchoredPosition += new Vector2(0, 1);
            Invoke("HideSelf", SCALEEFFECT_DISPLAY_DURATION);
        }
        _txtDamage.rectTransform.localScale = _initialScale;

    }

    // 判断文本是否正在显示
    public bool IsVisible()
    {
        return _txtDamage.enabled;
    }

    // 隐藏文本
    private void HideSelf()
    {
        _txtDamage.enabled = false;
        _owner._showCnt--;
    }

    // 每帧更新
    private void Update()
    {
        if (IsVisible())
        {
            _timer += Time.deltaTime;

            if (_velocity != Vector2.zero)
            {
                // 斜抛运动方式
                var percent = _timer / DISPLAY_DURATION;
                var nextPos = (Vector2)_txtDamage.rectTransform.localPosition + _velocity * Time.deltaTime;
                nextPos.y += 0.5f * GRAVITY * Time.deltaTime * Time.deltaTime;
                _velocity.y += GRAVITY * Time.deltaTime;
                _txtDamage.rectTransform.localPosition = nextPos;

                // 文本接近显示结束时开始逐渐透明
                if (percent >= 0.75f)
                {
                    var color = _txtDamage.color;
                    color.a = Mathf.Lerp(1, 0, (percent - 0.75f) / 0.25f);
                    _txtDamage.color = color;
                }
            }
            else
            {
                var percent = _timer / SCALEEFFECT_DISPLAY_DURATION;
                float kickTime = _scaleEffectWaitTime *0.5f;

                // 放大然后消失的方式
                if (_timer > _scaleEffectWaitTime)
                {
                    var scalePercent = (_timer - _scaleEffectWaitTime) / (SCALEEFFECT_DISPLAY_DURATION - _scaleEffectWaitTime);
                    _txtDamage.rectTransform.localScale = Vector3.Lerp(_initialScale * 1.4f, _initialScale * 2f, scalePercent);
                }
                else if(_timer < kickTime)
                {
                    var scalePercent = _timer / kickTime;

                    // 根据时间百分比计算大小变化百分比
                    float scaleChangePercent;
                    if (scalePercent < 0.5f)
                    {
                        scaleChangePercent = Mathf.Lerp(1.5f, 1.6f, scalePercent * 2f);
                    }
                    else
                    {
                        scaleChangePercent = Mathf.Lerp(1.6f, 1.4f, (scalePercent - 0.5f) * 2f);
                    }

                    _txtDamage.rectTransform.localScale = _initialScale * scaleChangePercent;
                }
            }
        }
    }
}