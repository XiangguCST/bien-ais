using System;
using System.Collections.Generic;
using System.Linq;

// 基础组件接口（你的组件需要继承这个）
public interface IComponent
{
    // 这里不加方法
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
    private Dictionary<Type, List<IComponent>> _components = new Dictionary<Type, List<IComponent>>();

    /// <summary>
    /// 添加组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="component">组件实例</param>
    public void AddComponent<T>(T component) where T : IComponent
    {
        Type type = component.GetType();

        if (!_components.ContainsKey(type))
        {
            _components[type] = new List<IComponent>();
        }

        _components[type].Add(component);

        foreach (Type interfaceType in type.GetInterfaces().Where(i => typeof(IComponent).IsAssignableFrom(i)))
        {
            if (!_components.ContainsKey(interfaceType))
            {
                _components[interfaceType] = new List<IComponent>();
            }

            _components[interfaceType].Add(component);
        }

        Type baseType = type.BaseType;
        while (baseType != null && typeof(IComponent).IsAssignableFrom(baseType))
        {
            if (!_components.ContainsKey(baseType))
            {
                _components[baseType] = new List<IComponent>();
            }

            _components[baseType].Add(component);
            baseType = baseType.BaseType;
        }
    }

    /// <summary>
    /// 移除组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="component">组件实例</param>
    public void RemoveComponent<T>(T component) where T : IComponent
    {
        Type type = component.GetType();

        // 从具体类型的列表中移除
        if (_components.ContainsKey(type))
        {
            _components[type].Remove(component);
            if (_components[type].Count == 0)
                _components.Remove(type);
        }

        // 从基类和接口类型的列表中移除
        foreach (Type interfaceType in type.GetInterfaces().Where(i => typeof(IComponent).IsAssignableFrom(i)))
        {
            if (_components.ContainsKey(interfaceType))
            {
                _components[interfaceType].Remove(component);
                if (_components[interfaceType].Count == 0)
                    _components.Remove(interfaceType);
            }
        }
        Type baseType = type.BaseType;
        while (baseType != null && typeof(IComponent).IsAssignableFrom(baseType))
        {
            if (_components.ContainsKey(baseType))
            {
                _components[baseType].Remove(component);
                if (_components[baseType].Count == 0)
                    _components.Remove(baseType);
            }
            baseType = baseType.BaseType;
        }
    }



    /// <summary>
    /// 获取组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>组件实例</returns>
    public T GetComponent<T>() where T : IComponent
    {
        Type type = typeof(T);
        return _components.ContainsKey(type) ? (T)_components[type].FirstOrDefault() : default(T);
    }

    /// <summary>
    /// 获取所有相同类型的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <returns>组件实例列表</returns>
    public List<T> GetComponents<T>() where T : IComponent
    {
        Type type = typeof(T);
        return _components.ContainsKey(type) ? _components[type].Cast<T>().ToList() : new List<T>();
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
    /// <param name="component">组件实例</param>
    public static void RemoveComponent<T>(this IComponentContainer container, T component) where T : IComponent
    {
        container.GetManager().RemoveComponent(component);
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
    /// 获取容器中的所有相同类型的组件
    /// </summary>
    /// <typeparam name="T">组件类型</typeparam>
    /// <param name="container">组件容器</param>
    /// <returns>组件实例列表</returns>
    public static List<T> GetComponents<T>(this IComponentContainer container) where T : class, IComponent
    {
        return container.GetManager().GetComponents<T>();
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
