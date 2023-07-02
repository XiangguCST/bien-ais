using System.Collections;
using UnityEngine;

public class HurtEffectController
{
    // 显示受伤效果
    static public void ShowHurtEffect(Character other)
    {
        CoroutineRunner.StartCoroutine(FlashRoutine(other));
    }

    static IEnumerator FlashRoutine(Character other)
    {
        var  mpb = new MaterialPropertyBlock();
        var meshRenderer = other.GetComponentInChildren<MeshRenderer>();
        meshRenderer.GetPropertyBlock(mpb);

        if (flashCount < 0) flashCount = DefaultFlashCount;
        Color oriColor = other._defaultColor;

        for (int i = 0; i < flashCount; i++)
        {
            for (float t = 0; t <= 1; t += Time.deltaTime / interval)
            {
                Color currentColor = Color.Lerp(oriColor, flashColor, t);
                mpb.SetColor(fillColorProperty, currentColor);
                if(meshRenderer != null)
                    meshRenderer.SetPropertyBlock(mpb);
                yield return null;
            }

            for (float t = 0; t <= 1; t += Time.deltaTime / interval)
            {
                Color currentColor = Color.Lerp(flashColor, oriColor, t);
                mpb.SetColor(fillColorProperty, currentColor);
                if(meshRenderer != null)
                    meshRenderer.SetPropertyBlock(mpb);
                yield return null;
            }
        }
        mpb.SetColor(fillColorProperty, oriColor);
        if (meshRenderer != null)
            meshRenderer.SetPropertyBlock(mpb);
        yield return null;
    }

    public static string fillColorProperty = "_FillColor";

    static int DefaultFlashCount = 2;

    static public int flashCount = DefaultFlashCount;
    static public Color flashColor = Color.red;
    static public float interval = 1f / 10f;
}
