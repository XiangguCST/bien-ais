using UnityEngine;
using UnityEngine.EventSystems;

public class UIMouseHoverDetector
{
    public delegate void MouseHoverDelegate();
    public event MouseHoverDelegate OnMouseHoverEnter; // 鼠标进入时的事件
    public event MouseHoverDelegate OnMouseHoverExit;  // 鼠标离开时的事件
    public event MouseHoverDelegate OnMouseHovering;   // 鼠标悬停时的事件

    private bool isMouseHovering = false;
    private GameObject uiElement;
    private InnerMonoBehaviour innerMonoBehaviour; // 内部的MonoBehaviour对象

    public UIMouseHoverDetector(GameObject uiElement)
    {
        SetUIElement(uiElement);

        // 创建一个新的游戏对象，并且添加一个InnerMonoBehaviour组件
        GameObject parent = GameObject.Find("UIMouseHoverDetectors");
        if(parent == null)
        {
            parent = new GameObject("UIMouseHoverDetectors");
        }
        GameObject innerGameObject = new GameObject("UIMouseHoverDetectorMonoBehaviour");
        innerMonoBehaviour = innerGameObject.AddComponent<InnerMonoBehaviour>();
        innerGameObject.transform.SetParent(parent.transform);

        // 把检测函数赋给内部MonoBehaviour的委托
        innerMonoBehaviour.UpdateAction = CheckMouseHovering;
    }

    public void SetUIElement(GameObject uiElement)
    {
        // 确保传入的对象不为空
        if (uiElement != null)
        {
            // 如果之前设置过对象，则移除该对象的事件监听
            if (this.uiElement != null)
            {
                RemoveEventListeners(this.uiElement);
            }

            this.uiElement = uiElement;
            // 为新对象添加事件监听
            AddEventListeners(uiElement);
        }
    }

    public bool IsMouseHovering()
    {
        return isMouseHovering;
    }

    private void CheckMouseHovering()
    {
        // 如果鼠标正在悬停，则触发悬停事件
        if (isMouseHovering)
        {
            OnMouseHovering?.Invoke();
        }
    }

    private void AddEventListeners(GameObject uiElement)
    {
        EventTrigger trigger = uiElement.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = uiElement.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry enterEntry = new EventTrigger.Entry();
        enterEntry.eventID = EventTriggerType.PointerEnter;
        enterEntry.callback.AddListener((eventData) => OnPointerEnter());
        trigger.triggers.Add(enterEntry);

        EventTrigger.Entry exitEntry = new EventTrigger.Entry();
        exitEntry.eventID = EventTriggerType.PointerExit;
        exitEntry.callback.AddListener((eventData) => OnPointerExit());
        trigger.triggers.Add(exitEntry);
    }

    private void RemoveEventListeners(GameObject uiElement)
    {
        EventTrigger trigger = uiElement.GetComponent<EventTrigger>();
        if (trigger != null)
        {
            EventTrigger.Entry enterEntry = trigger.triggers.Find(entry => entry.eventID == EventTriggerType.PointerEnter);
            if (enterEntry != null)
            {
                trigger.triggers.Remove(enterEntry);
            }

            EventTrigger.Entry exitEntry = trigger.triggers.Find(entry => entry.eventID == EventTriggerType.PointerExit);
            if (exitEntry != null)
            {
                trigger.triggers.Remove(exitEntry);
            }
        }
    }

    private void OnPointerEnter()
    {
        isMouseHovering = true;
        OnMouseHoverEnter?.Invoke();
    }

    private void OnPointerExit()
    {
        isMouseHovering = false;
        OnMouseHoverExit?.Invoke();
    }

    // 内部的MonoBehaviour类，用于每帧检测鼠标是否在悬停
    private class InnerMonoBehaviour : MonoBehaviour
    {
        public System.Action UpdateAction;

        private void Update()
        {
            UpdateAction?.Invoke();
        }
    }
}
