using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 资源加载类
/// </summary>
public class ResourcesLoader
{
    private static Dictionary<string, Object> cache = new Dictionary<string, Object>();

    public static T LoadResource<T>(string path) where T : Object
    {
        if (cache.ContainsKey(path))
        {
            return cache[path] as T;
        }
        else
        {
            T resource = Resources.Load<T>(path);
            if (resource != null)
            {
                cache[path] = resource;
            }
            else
            {
                Debug.LogWarning($"Resource at {path} not found!");
            }
            return resource;
        }
    }
}

