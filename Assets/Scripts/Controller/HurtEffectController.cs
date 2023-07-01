using System.Collections;
using UnityEngine;

public class HurtEffectController
{
    // 显示受伤效果
    static public void ShowHurtEffect(Character other)
    {
        if (other == null) return;

        Renderer renderer = other.GetComponentInChildren<Renderer>();
        CoroutineRunner.StartCoroutine(HurtEffectRoutine(renderer.material));
    }

    // 受伤效果的协程
    static IEnumerator HurtEffectRoutine(Material material)
    {
        if (material == null) yield return 0;

        // 受伤效果的总持续时间
        float duration = 0.2f;
        // 受伤时的填充阶段值
        float hurtValue = 0.75f;
        // 受伤前的填充阶段值
        float originalValue = material.GetFloat("_FillPhase");
        // 当前的持续时间
        float currentDuration = 0;

        // 受伤效果闪烁阶段
        while (currentDuration < duration / 2f)
        {
            currentDuration += Time.deltaTime;
            // 计算插值值
            float lerpValue = currentDuration / (duration / 2f);
            // 平滑过渡填充阶段值
            material.SetFloat("_FillPhase", Mathf.Lerp(originalValue, hurtValue, lerpValue));

            yield return null;
        }

        // 受伤效果淡出阶段
        while (currentDuration < duration)
        {
            currentDuration += Time.deltaTime;
            // 计算插值值
            float lerpValue = currentDuration / duration;
            // 平滑过渡填充阶段值
            material.SetFloat("_FillPhase", Mathf.Lerp(hurtValue, originalValue, lerpValue));

            yield return null;
        }

        // 重置填充阶段值到原始值
        material.SetFloat("_FillPhase", originalValue);

        yield return 0;
    }
}
