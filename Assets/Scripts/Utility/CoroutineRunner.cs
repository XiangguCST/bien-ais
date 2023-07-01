using System.Collections;
using UnityEngine;

public class CoroutineRunner
{
    private static CoroutineOwner coroutineOwner;

    static CoroutineRunner()
    {
        // Create an empty GameObject and attach it to the scene to get a CoroutineOwner
        var coroutineObject = new GameObject("CoroutineRunner");
        Object.DontDestroyOnLoad(coroutineObject);
        coroutineOwner = coroutineObject.AddComponent<CoroutineOwner>();
    }

    // Wrap the IEnumerator version of StartCoroutine
    public static Coroutine StartCoroutine(IEnumerator routine)
    {
        return coroutineOwner.StartCoroutine(routine);
    }

    // Wrap the string method name version of StartCoroutine
    public static Coroutine StartCoroutine(string methodName)
    {
        return coroutineOwner.StartCoroutine(methodName);
    }

    // Wrap the string method name and value version of StartCoroutine
    public static Coroutine StartCoroutine(string methodName, object value)
    {
        return coroutineOwner.StartCoroutine(methodName, value);
    }

    // Wrap StopAllCoroutines
    public static void StopAllCoroutines()
    {
        coroutineOwner.StopAllCoroutines();
    }

    // Wrap the IEnumerator version of StopCoroutine
    public static void StopCoroutine(IEnumerator routine)
    {
        coroutineOwner.StopCoroutine(routine);
    }

    // Wrap the Coroutine version of StopCoroutine
    public static void StopCoroutine(Coroutine routine)
    {
        coroutineOwner.StopCoroutine(routine);
    }

    // Wrap the string method name version of StopCoroutine
    public static void StopCoroutine(string methodName)
    {
        coroutineOwner.StopCoroutine(methodName);
    }

    // This class is only used for its MonoBehaviour
    private class CoroutineOwner : MonoBehaviour { }
}
