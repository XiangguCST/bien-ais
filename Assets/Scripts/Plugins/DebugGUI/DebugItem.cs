using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// DebugItem是一个包含获取字符串函数、颜色和字体大小设置的类。
/// </summary>
public class DebugItem
{
    public Func<string> Getter;  // 获取显示字符串的函数
    public Color Color = Color.black;  // 显示颜色，默认为黑色
    public int FontSize = 18;  // 字体大小，默认为18

    public DebugItem(Func<string> getter)
    {
        Getter = getter;
    }
}