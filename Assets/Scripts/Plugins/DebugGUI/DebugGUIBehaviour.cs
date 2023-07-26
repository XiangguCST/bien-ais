using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// DebugGUIBehaviour类是用于在GUI上显示调试信息的Unity MonoBehaviour。
/// </summary>
public class DebugGUIBehaviour : MonoBehaviour
{
    public Dictionary<string, DebugItem> DebugItems = new Dictionary<string, DebugItem>();  // 调试条目列表

    private void OnGUI()
    {
        foreach (var item in DebugItems)
        {
            var boxColor = GetBackgroundColor(item.Value.Color);
            var boxBackground = CreateBox(2, 2, boxColor);
            var boxStyle = new GUIStyle("box") { normal = { background = boxBackground } };

            GUILayout.BeginVertical(boxStyle);
            DisplayDebugItem(item.Key, item.Value);
            GUILayout.EndVertical();
        }
    }

    private Texture2D CreateBox(int width, int height, Color col)
    {
        var pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
            pix[i] = col;

        var result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();

        return result;
    }

    // 获得与前景色对比度适合的背景色
    public static Color GetBackgroundColor(Color color)
    {
        // Convert the color to HSL
        float h, s, l;
        Color.RGBToHSV(color, out h, out s, out l);

        // Calculate the luminance (perceived brightness) of the color
        float luminance = (0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b);

        // Choose the appropriate contrast color based on luminance
        Color contrastColor;
        if (luminance < 0.5f)
        {
            // For dark colors, use a light color as contrast
            contrastColor = new Color(1f, 1f, 1f); // White
        }
        else
        {
            // For light colors, use a dark color as contrast
            contrastColor = new Color(0f, 0f, 0f); // Black
        }

        // You can also adjust the brightness of the contrast color if needed
        // For example, you can make the contrast color slightly brighter or darker
        // contrastColor *= 0.9f; // Slightly darker
        // contrastColor *= 1.1f; // Slightly brighter

        return contrastColor;
    }

    private void DisplayDebugItem(string key, DebugItem item)
    {
        var keyStyle = new GUIStyle
        {
            normal = { textColor = item.Color },
            fontSize = (int)(item.FontSize * 1.2f),
            fontStyle = FontStyle.Bold
        };
        var valueStyle = new GUIStyle
        {
            normal = { textColor = item.Color },
            fontSize = item.FontSize
        };

        GUILayout.BeginHorizontal();
        GUILayout.Label($"{key}:", keyStyle);
        GUILayout.Space(20);  // 增加key和value之间的间距

        GUILayout.Label($"{item.Getter()}", valueStyle);
        GUILayout.EndHorizontal();
    }
}
