using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface IComponentTest : IComponent
{
    string GetName();
}

public interface IComponentA : IComponentTest { }
public interface IComponentB : IComponentTest { }
public interface IComponentC : IComponentTest { }
public interface IComponentD : IComponentTest { }
public interface IComponentE : IComponentTest { }

public class ComponentA : IComponentA
{
    public virtual string GetName() => "ComponentA";
}

public class ComponentB : IComponentB
{
    public string GetName() => "ComponentB";
}

public class ComponentC : IComponentC, IComponentD
{
    public string GetName() => "ComponentC";
}

public class ComponentD : ComponentA, IComponentE
{
    public override string GetName() => "ComponentD";
}

public class ComponentSystemTestScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ComponentSystemTestFrame.Main();
    }

    // Update is called once per frame
    void Update()
    {

    }
}

public class ComponentSystemTestFrame
{
    public static void Main()
    {
        Debug.Log("测试组一：基本的添加和获取");

        var container = new ComponentContainerImpl();

        var componentA = new ComponentA();
        var componentB = new ComponentB();
        var componentC = new ComponentC();
        var componentD = new ComponentD();

        container.AddComponent(componentA);
        container.AddComponent(componentB);
        container.AddComponent(componentC);
        container.AddComponent(componentD);

        Debug.Log("获取ComponentA：" + container.GetComponent<ComponentA>().GetName());  // 应输出：ComponentA
        Debug.Log("获取ComponentB：" + container.GetComponent<ComponentB>().GetName());  // 应输出：ComponentB
        Debug.Log("获取ComponentC：" + container.GetComponent<ComponentC>().GetName());  // 应输出：ComponentC
        Debug.Log("获取ComponentD：" + container.GetComponent<ComponentD>().GetName());  // 应输出：ComponentD

        Debug.Log("测试组二：复杂的获取和移除");

        var components = container.GetComponents<IComponentTest>();
        Debug.Log("获取所有IComponentTest：" + string.Join(", ", components.Select(c => c.GetName())));  // 应输出：ComponentA, ComponentB, ComponentC, ComponentD

        var componentsA = container.GetComponents<IComponentA>();
        Debug.Log("获取所有IComponentA：" + string.Join(", ", componentsA.Select(c => c.GetName())));  // 应输出：ComponentA, ComponentD

        var componentsB = container.GetComponents<IComponentB>();
        Debug.Log("获取所有IComponentB：" + string.Join(", ", componentsB.Select(c => c.GetName())));  // 应输出：ComponentB

        container.RemoveComponent(componentB);
        Debug.Log("移除后，获取ComponentB：" + (container.GetComponent<ComponentB>() != null));  // 应输出：false

        container.RemoveComponent(componentD);
        var componentsDA = container.GetComponents<IComponentA>();
        Debug.Log("移除后，获取所有IComponentA：" + string.Join(", ", componentsDA.Select(c => c.GetName())));  // 应输出：ComponentA

        Debug.Log("测试组三：HasComponent<T>()");

        bool hasComponentA = container.HasComponent<ComponentA>();
        Debug.Log("是否包含ComponentA：" + hasComponentA);  // 应输出：true

        bool hasComponentB = container.HasComponent<ComponentB>();
        Debug.Log("是否包含ComponentB：" + hasComponentB);  // 应输出：false

        bool hasComponentC = container.HasComponent<ComponentC>();
        Debug.Log("是否包含ComponentC：" + hasComponentC);  // 应输出：true

        bool hasComponentD = container.HasComponent<ComponentD>();
        Debug.Log("是否包含ComponentD：" + hasComponentD);  // 应输出：false

        bool hasIComponentD = container.HasComponent<IComponentD>();
        Debug.Log("是否包含IComponentD：" + hasIComponentD);  // 应输出：true
    }
}

