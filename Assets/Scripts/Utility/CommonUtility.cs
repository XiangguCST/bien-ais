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
}
