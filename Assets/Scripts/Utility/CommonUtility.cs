using UnityEngine;

public class CommonUtility
{
    public static bool IsCrit(float critChange = 0.2f)
    {
        // 生成一个0到1之间的随机数
        float randomValue = UnityEngine.Random.value;

        // 检查随机数是否小于暴击几率
        bool isCrit = randomValue < critChange;

        return isCrit;
    }
}
