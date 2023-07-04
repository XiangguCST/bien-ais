using System.Collections;
using UnityEngine;

public class HurtEffectController
{
    static public void ShowHurtEffect(Character other)
    {
        CoroutineRunner.StartCoroutine(FlashRoutine(other));
    }

    static IEnumerator FlashRoutine(Character other)
    {
        var mpb = new MaterialPropertyBlock();
        var meshRenderer = other.GetComponentInChildren<MeshRenderer>();
        meshRenderer.GetPropertyBlock(mpb);

        if (flashCount < 0) flashCount = DefaultFlashCount;
        Color oriColor = other._defaultColor;

        float dynamicInterval = interval; // 初始闪烁间隔
        for (int i = 0; i < flashCount; i++)
        {
            // 变红
            for (float t = 0; t <= 1; t += Time.deltaTime / dynamicInterval)
            {
                Color currentColor = Color.Lerp(oriColor, flashColor, t);
                mpb.SetColor(fillColorProperty, currentColor);
                if (meshRenderer != null)
                    meshRenderer.SetPropertyBlock(mpb);
                yield return null;
            }

            // 恢复颜色
            for (float t = 0; t <= 1; t += Time.deltaTime / dynamicInterval)
            {
                Color currentColor = Color.Lerp(flashColor, oriColor, t);
                mpb.SetColor(fillColorProperty, currentColor);
                if (meshRenderer != null)
                    meshRenderer.SetPropertyBlock(mpb);
                yield return null;
            }

            // 每个闪烁周期结束后，增加间隔时间，使闪烁频率减少
            dynamicInterval *= 1.5f;
        }

        mpb.SetColor(fillColorProperty, oriColor);
        if (meshRenderer != null)
            meshRenderer.SetPropertyBlock(mpb);
        yield return null;
    }

    public static string fillColorProperty = "_FillColor";

    static int DefaultFlashCount = 3;

    static public int flashCount = DefaultFlashCount;
    static public Color flashColor = Color.red;
    static public float interval = 1f / 60f;
}
