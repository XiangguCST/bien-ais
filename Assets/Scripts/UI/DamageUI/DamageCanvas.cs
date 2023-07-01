using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 伤害显示画布
public class DamageCanvas : MonoBehaviour
{
    // 伤害文本Prefab路径
    private const string DAMAGE_TEXT_PREFAB_PATH = "Prefabs/UI/DamageText";

    public Character _owner; // 画布所属角色
    public int _showCnt = 0; // 当前显示的伤害文本数目
    public Color criticalDamageColor = new Color(1, 0, 0.5f); // 暴击伤害颜色
    public Color normalDamageColor = new Color(1, 0.5f, 0); // 普通伤害颜色

    // 伤害文本列表
    private List<DamageText> _damageTxtList = new List<DamageText>();

    public void ShowDamage(int damage, bool bCritical = false)
    {
        var txtDamage = GetFreeDamageText();
        float originX = UnityEngine.Random.Range(-0.1f, 0.1f);
        // 设定伤害文本的起始位置
        txtDamage.transform.localPosition = new Vector2(originX, 2);

        // 根据是否为暴击设定不同的颜色
        txtDamage.SetColor(bCritical ? criticalDamageColor : normalDamageColor);

        // 显示伤害
        txtDamage.ShowDamage(damage, bCritical);
    }

    // 获取一个未显示的伤害文本对象，如果当前没有未显示的对象则创建一个新的对象
    DamageText GetFreeDamageText()
    {
        var freeText = _damageTxtList.Find(c => !c.IsVisible());
        if (freeText == null)
        {
            freeText = CreateDamageText();
            _damageTxtList.Add(freeText);
        }
        return freeText;
    }

    // 创建一个新的伤害文本对象
    DamageText CreateDamageText()
    {
        var txtDamageSrc = Resources.Load<GameObject>(DAMAGE_TEXT_PREFAB_PATH);
        var txtDamage = Instantiate(txtDamageSrc, this.transform).GetComponent<DamageText>();
        txtDamage._owner = this;
        txtDamage.GetComponent<RectTransform>().localPosition = Vector2.zero;
        return txtDamage;
    }
}
