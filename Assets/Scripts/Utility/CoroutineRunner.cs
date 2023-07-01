using System.Collections;
using UnityEngine;

/// <summary>
/// 携程运行器（在不继承Mono类中使用携程） 
/// </summary>
public class CoroutineRunner
{
    private static CoroutineRunner instance;
    private MonoBehaviour coroutineOwner;

    private CoroutineRunner()
    {
        // 创建一个空的GameObject并将其附加到场景中，以获得MonoBehaviour
        var coroutineObject = new UnityEngine.GameObject("CoroutineRunner");
        UnityEngine.Object.DontDestroyOnLoad(coroutineObject);
        coroutineOwner = coroutineObject.AddComponent<CoroutineOwner>();
    }

    public static CoroutineRunner Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new CoroutineRunner();
            }
            return instance;
        }
    }

    public Coroutine StartCoroutine(IEnumerator routine)
    {
        return coroutineOwner.StartCoroutine(routine);
    }

    public void StopCoroutine(Coroutine routine)
    {
        if(routine != null)
            coroutineOwner.StopCoroutine(routine);
    }

    private class CoroutineOwner : MonoBehaviour { }
}

/* 示例代码
public class MyCustomClass
{
    private CoroutineRunner coroutineRunner;

    public MyCustomClass()
    {
        coroutineRunner = CoroutineRunner.Instance;
    }

    public void StartMyCoroutine()
    {
        coroutineRunner.StartCoroutine(MyCoroutine());
    }

    private IEnumerator MyCoroutine()
    {
        // 协程逻辑
        yield return new WaitForSeconds(2f);
        Debug.Log("Coroutine completed");
    }
}
 */
