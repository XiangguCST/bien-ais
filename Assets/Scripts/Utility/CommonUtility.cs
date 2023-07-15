using System;
using System.Collections;
using UnityEngine;

public class CommonUtility
{
    public static void SetCharacterColor(Character other, Color newColor)
    {
        var mpb = new MaterialPropertyBlock();
        var meshRenderer = other.GetComponentInChildren<MeshRenderer>();
        meshRenderer.GetPropertyBlock(mpb);
        mpb.SetColor(fillColorProperty, newColor);
        meshRenderer.SetPropertyBlock(mpb);
    }

    public static string fillColorProperty = "_FillColor";

    public static string GetHotKeyString(KeyCode hotKey)
    {
        // 设置快捷键显示
        switch (hotKey)
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
                return (hotKey - KeyCode.Keypad0).ToString();
            case KeyCode.UpArrow:
               return  "↑";
            case KeyCode.DownArrow:
               return  "↓";
            case KeyCode.LeftArrow:
               return  "←";
            case KeyCode.RightArrow:
               return  "→";
            default:
               return  hotKey.ToString();
        }
    }

    /// <summary>
    /// 控制角色与其他角色的碰撞。
    /// 注意：你需要在Unity中创建名为"Character"和"IgnoreCollisionLayer"的两个Layer，
    /// </summary>
    /// <param name="character">角色</param>
    /// <param name="bEnable">是否启用碰撞</param>
    public static void EnableCollision(Character character, bool bEnable = true)
    {
        if (character == null) return;

        if(!_hasInitCollision)
        {
            // 获取Layer的ID
            int defaultLayer = LayerMask.NameToLayer("Character");
            int ignoreCollisionLayer = LayerMask.NameToLayer("IgnoreCollisionLayer");

            // 设置Layer之间的碰撞关系
            Physics2D.IgnoreLayerCollision(defaultLayer, ignoreCollisionLayer, true);
            _hasInitCollision = true;
        }

        if (bEnable)
            // 如果启用碰撞，将角色的Layer设置为Default
            character.gameObject.layer = LayerMask.NameToLayer("Character");
        else
            // 如果禁用碰撞，将角色的Layer设置为IgnoreCollisionLayer
            character.gameObject.layer = LayerMask.NameToLayer("IgnoreCollisionLayer");
    }
    private static bool _hasInitCollision = false;
}
