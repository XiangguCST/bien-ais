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
            var keyStyle = new GUIStyle
            {
                normal = { textColor = item.Value.Color },
                fontSize = (int)(item.Value.FontSize * 1.2f),
                fontStyle = FontStyle.Bold
            };

            var valueStyle = new GUIStyle
            {
                normal = { textColor = item.Value.Color },
                fontSize = item.Value.FontSize
            };

            item.Value.Display(keyStyle, valueStyle);
        }
    }

    //private Texture2D CreateBox(int width, int height, Color col)
    //{
    //    var pix = new Color[width * height];
    //    for (int i = 0; i < pix.Length; i++)
    //        pix[i] = col;

    //    var result = new Texture2D(width, height);
    //    result.SetPixels(pix);
    //    result.Apply();

    //    return result;
    //}

    //public static Color GetBackgroundColor(Color color)
    //{
    //    // Convert the color to HSL
    //    float h, s, l;
    //    Color.RGBToHSV(color, out h, out s, out l);

    //    // Calculate the luminance (perceived brightness) of the color
    //    float luminance = (0.2126f * color.r + 0.7152f * color.g + 0.0722f * color.b);

    //    // Choose the appropriate contrast color based on luminance
    //    Color contrastColor;
    //    if (luminance < 0.5f)
    //    {
    //        // For dark colors, use a light color as contrast
    //        contrastColor = new Color(1f, 1f, 1f); // White
    //    }
    //    else
    //    {
    //        // For light colors, use a dark color as contrast
    //        contrastColor = new Color(0f, 0f, 0f); // Black
    //    }

    //    // You can also adjust the brightness of the contrast color if needed
    //    // For example, you can make the contrast color slightly brighter or darker
    //    // contrastColor *= 0.9f; // Slightly darker
    //    // contrastColor *= 1.1f; // Slightly brighter

    //    return contrastColor;
    //}

}
