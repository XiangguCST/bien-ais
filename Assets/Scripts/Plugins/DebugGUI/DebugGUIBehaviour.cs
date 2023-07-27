using System.Collections.Generic;
using UnityEngine;

// DebugGUIBehaviour类
public class DebugGUIBehaviour : MonoBehaviour
{
    public Dictionary<string, IDebugItem> DebugItems = new Dictionary<string, IDebugItem>();

    private void OnGUI()
    {
        foreach (var item in DebugItems)
        {
            DisplayDebugItem(item.Value);
        }
    }

    private void DisplayDebugItem(IDebugItem item)
    {
        var keyStyle = new GUIStyle
        {
            normal = { textColor = item.Color },
            fontSize = (int)(item.FontSize * 1.2f),
            fontStyle = FontStyle.Bold,
            alignment = TextAnchor.MiddleLeft  // 设置左对齐
        };
        var valueStyle = new GUIStyle
        {
            normal = { textColor = item.Color },
            fontSize = item.FontSize,
            alignment = TextAnchor.MiddleCenter
        };

        item.Display(keyStyle, valueStyle);
    }
}
