using System;
using System.Collections.Generic;
using System.Linq;

// 基础组件接口
public interface IComponent
{
    // 这里可以添加接口方法
}

// 容器接口，用于管理和获取组件管理器
public interface IComponentContainer
{
    /// <summary>
    /// 获取组件管理器
    /// </summary>
    /// <returns>组件管理器实例</returns>
    ComponentManager GetManager();
}

// 组件管理器，用于管理所有类型的组件
public class ComponentManager
{
    private Dictionary<Type, IComponent> _components = new Dictionary<Type, IComponent>();

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="component">组件实例</param>
    public void AddComponent<T>(T component) where T : IComponent
    {
        Type type = component.GetType();
        // 将组件添加到其特定类型列表
        _components[type] = component;

        // 将组件添加到其基本类型和接口类型列表
        foreach (Type interfaceType in type.GetInterfaces().Where(i => typeof(IComponent).IsAssignableFrom(i)))
        {
            _components[interfaceType] = component;
        }
        Type baseType = type.BaseType;
        while (baseType != null && typeof(IComponent).IsAssignableFrom(baseType))
        {
            _components[baseType] = component;
            baseType = baseType.BaseType;
        }
    }

    /// <summary>
    /// 移除组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    public void RemoveComponent<T>() where T : IComponent
    {
        Type type = typeof(T);
        _components.Remove(type);
    }

    /// <summary>
    /// 获取组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>组件实例</returns>
    public T GetComponent<T>() where T : IComponent
    {
        Type type = typeof(T);
        return _components.ContainsKey(type) ? (T)_components[type] : default(T);
    }

    /// <summary>
    /// 检查是否存在组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>存在返回true，否则返回false</returns>
    public bool HasComponent<T>() where T : IComponent
    {
        Type type = typeof(T);
        return _components.ContainsKey(type);
    }
}

// 实现容器接口的辅助类
public class ComponentContainerImpl : IComponentContainer
{
    private ComponentManager _manager = new ComponentManager();

    /// <summary>
    /// 获取组件管理器
    /// </summary>
    /// <returns>组件管理器实例</returns>
    public ComponentManager GetManager()
    {
        return _manager;
    }
}

public static class ComponentContainerExtensions
{
    /// <summary>
    /// 添加组件到容器
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="container">组件容器</param>
    /// <param name="component">组件实例</param>
    public static void AddComponent<T>(this IComponentContainer container, T component) where T : IComponent
    {
        container.GetManager().AddComponent(component);
    }

    /// <summary>
    /// 从容器中移除组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="container">组件容器</param>
    public static void RemoveComponent<T>(this IComponentContainer container) where T : IComponent
    {
        container.GetManager().RemoveComponent<T>();
    }

    /// <summary>
    /// 获取容器中的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="container">组件容器</param>
    /// <returns>组件实例</returns>
    public static T GetComponent<T>(this IComponentContainer container) where T : class, IComponent
    {
        return container.GetManager().GetComponent<T>();
    }

    /// <summary>
    /// 检查容器中是否存在组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="container">组件容器</param>
    /// <returns>存在返回true，否则返回false</returns>
    public static bool HasComponent<T>(this IComponentContainer container) where T : IComponent
    {
        return container.GetManager().HasComponent<T>();
    }
}

/*
 * 示例：技能类，具体业务代码
 *
public class Skill : IComponentContainer
{
    private IComponentContainer _container = new ComponentContainerImpl();

    /// <summary>
    /// 获取组件管理器
    /// </summary>
    /// <returns>组件管理器实例</returns>
    public ComponentManager GetManager()
    {
        return _container.GetManager();
    }
}
*/
