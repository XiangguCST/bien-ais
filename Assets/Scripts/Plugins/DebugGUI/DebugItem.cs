﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


// DebugItem接口
public interface IDebugItem
{
    string Key { get; }
    Func<string> Getter { get; }
    Color Color { get; }
    int FontSize { get; }
    List<IDebugItem> Children { get; }

    void Display(GUIStyle keyStyle, GUIStyle valueStyle, Color? boxColor = null);
}

// DebugItem基类
public class DebugItemBase : IDebugItem
{
    public string Key { get; }
    public Func<string> Getter { get; }
    public Color Color { get; set; }
    public int FontSize { get; set; }
    public List<IDebugItem> Children { get; }

    public DebugItemBase(string key, Func<string> getter, Color color = default, int fontSize = 18)
    {
        Key = key;
        Getter = getter;
        Color = color == default ? Color.black : color;
        FontSize = fontSize;
        Children = new List<IDebugItem>();
    }

    // 创建带背景颜色的 box 样式
    protected GUIStyle CreateBoxStyle(Color color)
    {
        var boxColor = GetBackgroundColor(color);
        var boxBackground = CreateBox(2, 2, boxColor);
        var boxStyle = new GUIStyle("box") { normal = { background = boxBackground } };

        return boxStyle;
    }

    // 根据给定的颜色计算背景颜色
    protected Color GetBackgroundColor(Color color)
    {
        float h, s, l;
        Color.RGBToHSV(color, out h, out s, out l);

        // 减小亮度以得到更深的颜色
        l = Mathf.Clamp01(l - 0.2f);
        return Color.HSVToRGB(h, s, l);
    }

    // 根据给定的颜色生成一个较深的颜色
    protected Color GetDarkerColor(Color color)
    {
        if (color == Color.black) // 如果原色是黑色
        {
            return Color.gray; // 返回浅灰色
        }

        float h, s, v;
        Color.RGBToHSV(color, out h, out s, out v);

        v = Mathf.Clamp01(v - 0.2f);
        return Color.HSVToRGB(h, s, v);
    }


    // 创建一个指定颜色的 Texture2D
    protected Texture2D CreateBox(int width, int height, Color col)
    {
        var pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        var result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    // 根据颜色的亮度计算对比色
    protected Color GetContrastColor(Color color)
    {
        float luminance = 0.299f * color.r + 0.587f * color.g + 0.114f * color.b;
        return luminance < 0.5f ? Color.white : Color.black;
    }

    public virtual void Display(GUIStyle keyStyle, GUIStyle valueStyle, Color? boxColor = null)
    {
        Color actualBoxColor = boxColor ?? GetContrastColor(Color);
        GUILayout.BeginVertical(CreateBoxStyle(actualBoxColor));
        GUILayout.BeginHorizontal();
        GUILayout.Label($"{Key}:", keyStyle);
        GUILayout.Space(20);
        string value = Getter();
        if (!string.IsNullOrEmpty(value))
        {
            Color valueBoxColor = GetDarkerColor(actualBoxColor);
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace(); // 添加一个可伸缩的空间
            GUILayout.BeginVertical(CreateBoxStyle(valueBoxColor));
            GUILayout.Label(value, valueStyle);
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace(); // 添加一个可伸缩的空间
            GUILayout.EndVertical();
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }
}

// 对象调试项
public class DebugItemObject : DebugItemBase
{
    public DebugItemObject(string key, object obj, Color color = default, int fontSize = 18)
        : base(key, () => "", color, fontSize)
    {
        // 获取obj的字段和属性信息
        var type = obj.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.FieldType.BaseType == typeof(MulticastDelegate)) continue;  // 忽略委托类型
            if (typeof(IDictionary).IsAssignableFrom(field.FieldType)) // 判断是否为字典类型
            {
                var dic = field.GetValue(obj) as IDictionary;
                if (dic != null)
                    Children.Add(new DebugItemDictionary($"{Key}.{field.Name}", dic, color));
            }
            else if (typeof(IList).IsAssignableFrom(field.FieldType)) // 判断是否为列表类型
            {
                var list = field.GetValue(obj) as IList;
                if (list != null)
                    Children.Add(new DebugItemList($"{Key}.{field.Name}", list, color));
            }
            else
            {
                var value = field.GetValue(obj);
                Children.Add(new DebugItemBase($"{Key}.{field.Name}", () => value?.ToString() ?? "null", color, fontSize));
            }
        }

        foreach (var property in properties)
        {
            if (!property.CanRead) continue;
            if (property.PropertyType.BaseType == typeof(MulticastDelegate)) continue;  // 忽略委托类型
            if (typeof(IDictionary).IsAssignableFrom(property.PropertyType)) // 判断是否为字典类型
            {
                var dic = property.GetValue(obj) as IDictionary;
                if (dic != null)
                    Children.Add(new DebugItemDictionary($"{Key}.{property.Name}", dic, color));
            }
            else if (typeof(IList).IsAssignableFrom(property.PropertyType)) // 判断是否为列表类型
            {
                var list = property.GetValue(obj) as IList;
                if (list != null)
                    Children.Add(new DebugItemList($"{Key}.{property.Name}", list, color));
            }
            else
            {
                var value = property.GetValue(obj);
                Children.Add(new DebugItemBase($"{Key}.{property.Name}", () => value?.ToString() ?? "null", color, fontSize));
            }
        }
    }

    public override void Display(GUIStyle keyStyle, GUIStyle valueStyle, Color? boxColor = null)
    {
        Color actualBoxColor = boxColor ?? GetContrastColor(Color);

        GUILayout.BeginVertical(CreateBoxStyle(actualBoxColor));

        // 显示自身信息
        base.Display(keyStyle, valueStyle, actualBoxColor);

        // 显示子项信息
        Color darkerColor = GetDarkerColor(actualBoxColor);
        foreach (var child in Children)
        {
            child.Display(keyStyle, valueStyle, darkerColor);
        }

        GUILayout.EndVertical();
    }
}

// 列表调试项
public class DebugItemList : DebugItemBase
{
    public DebugItemList(string key, IList list, Color color = default, int fontSize = 18)
        : base(key, () => $"Count: {list.Count}", color, fontSize)
    {
        for (var i = 0; i < list.Count; i++)
        {
            var value = list[i];
            Children.Add(new DebugItemBase($"{Key}[{i}]", () => value?.ToString() ?? "null", color, fontSize));
        }
    }

    public override void Display(GUIStyle keyStyle, GUIStyle valueStyle, Color? boxColor = null)
    {
        Color actualBoxColor = boxColor ?? GetContrastColor(Color);

        GUILayout.BeginVertical(CreateBoxStyle(actualBoxColor));

        // 显示自身信息
        base.Display(keyStyle, valueStyle, actualBoxColor);

        // 显示子项信息
        Color darkerColor = GetDarkerColor(actualBoxColor);
        foreach (var child in Children)
        {
            child.Display(keyStyle, valueStyle, darkerColor);
        }

        GUILayout.EndVertical();
    }
}

// 字典调试项
public class DebugItemDictionary : DebugItemBase
{
    public DebugItemDictionary(string key, IDictionary dic, Color color = default, int fontSize = 18)
        : base(key, () => $"Count: {dic.Count}", color, fontSize)
    {
        foreach (DictionaryEntry pair in dic)
        {
            Children.Add(new DebugItemBase($"{Key}[{pair.Key}]", () => pair.Value?.ToString() ?? "null", color, fontSize));
        }
    }

    public override void Display(GUIStyle keyStyle, GUIStyle valueStyle, Color? boxColor = null)
    {
        Color actualBoxColor = boxColor ?? GetContrastColor(Color);

        GUILayout.BeginVertical(CreateBoxStyle(actualBoxColor));

        // 显示自身信息
        base.Display(keyStyle, valueStyle, actualBoxColor);

        // 显示子项信息
        Color darkerColor = GetDarkerColor(actualBoxColor);
        foreach (var child in Children)
        {
            child.Display(keyStyle, valueStyle, darkerColor);
        }

        GUILayout.EndVertical();
    }
}