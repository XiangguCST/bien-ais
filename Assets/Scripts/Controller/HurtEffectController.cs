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
        Color oriColor = mpb.GetColor(fillColorProperty);
        float oriPhase = mpb.GetFloat(fillPhaseProperty);

        var wait = new WaitForSeconds(interval);

        for (int i = 0; i < flashCount; i++)
        {
            mpb.SetColor(fillColorProperty, flashColor);
            mpb.SetFloat(fillPhaseProperty, 0.5f);
            if(meshRenderer != null)
                meshRenderer.SetPropertyBlock(mpb);
            yield return wait;

            mpb.SetColor(fillColorProperty, oriColor);
            mpb.SetFloat(fillPhaseProperty, oriPhase);
            if(meshRenderer != null)
                meshRenderer.SetPropertyBlock(mpb);
            yield return wait;
        }

        yield return null;
    }

    public static string fillPhaseProperty = "_FillPhase";
    public static string fillColorProperty = "_FillColor";

    static int DefaultFlashCount = 2;

    static public int flashCount = DefaultFlashCount;
    static public Color flashColor = Color.red;
    static public float interval = 1f / 20f;
}
