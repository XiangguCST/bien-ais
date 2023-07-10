using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 双击检测类
/// </summary>
public class DoubleClickDetector
{
    private Dictionary<KeyCode, float> lastClickTimes = new Dictionary<KeyCode, float>();
    private float interval;

    public DoubleClickDetector(float interval)
    {
        this.interval = interval;
    }

    public bool IsDoubleClick(KeyCode key)
    {
        if (!lastClickTimes.ContainsKey(key))
        {
            lastClickTimes[key] = -interval;
        }

        if (Input.GetKeyDown(key))
        {
            if (Time.time - lastClickTimes[key] < interval)
            {
                lastClickTimes[key] = -interval;
                return true;
            }
            else
            {
                lastClickTimes[key] = Time.time;
            }
        }
        return false;
    }
}
