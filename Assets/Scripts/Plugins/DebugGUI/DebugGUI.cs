using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
/// DebugGUI静态类提供了一个简单的API，用于在屏幕上显示调试信息。
/// </summary>
public static class DebugGUI
{
    private static DebugGUIBehaviour debugGuiBehaviour;

    private static DebugGUIBehaviour GetDebugGUIBehaviour()
    {
        if (debugGuiBehaviour == null)
        {
            var gameObject = new GameObject("DebugGUI") { hideFlags = HideFlags.HideAndDontSave };
            debugGuiBehaviour = gameObject.AddComponent<DebugGUIBehaviour>();
        }

        return debugGuiBehaviour;
    }

    public static void AddDebugItem(string keyName, string valInfo, Color color = default)
    {
        var item = new DebugItem(() => valInfo) { Color = color == default ? Color.black : color };
        GetDebugGUIBehaviour().DebugItems[keyName] = item;
    }

    public static void AddDebugItem(string keyName, Func<string> valGetter, Color color = default)
    {
        var item = new DebugItem(valGetter) { Color = color == default ? Color.black : color };
        GetDebugGUIBehaviour().DebugItems[keyName] = item;
    }

    public static void AddDebugTotalObject(string keyName, object obj, Color color = default)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj));

        var type = obj.GetType();
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.FieldType.BaseType == typeof(MulticastDelegate)) continue;  // 忽略委托类型
            if (typeof(IDictionary).IsAssignableFrom(field.FieldType)) continue;  // 忽略字典类型
            if (typeof(IList).IsAssignableFrom(field.FieldType)) continue;  // 忽略列表类型

            var value = field.GetValue(obj);
            AddDebugItem($"{keyName}.{field.Name}", () => value?.ToString() ?? "null", color);
        }

        foreach (var property in properties)
        {
            if (!property.CanRead) continue;
            if (property.PropertyType.BaseType == typeof(MulticastDelegate)) continue;  // 忽略委托类型
            if (typeof(IDictionary).IsAssignableFrom(property.PropertyType)) continue;  // 忽略字典类型
            if (typeof(IList).IsAssignableFrom(property.PropertyType)) continue;  // 忽略列表类型

            var value = property.GetValue(obj);
            AddDebugItem($"{keyName}.{property.Name}", () => value?.ToString() ?? "null", color);
        }
    }
}





// 示例：
//// 添加一个固定的字符串调试条目
//DebugGUI.AddDebugItem("Version", "1.0.0");

//// 添加一个获取字符串的函数调试条目，显示红色文本
//DebugGUI.AddDebugItem("Status", () => StatusManager.GetCurrentStatus().ToString(), Color.red);

//// 添加一个获取字符串的函数调试条目，显示默认颜色的文本
//DebugGUI.AddDebugItem("Status Elapsed Time", () => StatusManager.GetStatusElapsedTime().ToString());

//// 监控 Example 对象的所有字段和属性的变化
// DebugGUI.AddDebugTotalObject("example", new Example());


