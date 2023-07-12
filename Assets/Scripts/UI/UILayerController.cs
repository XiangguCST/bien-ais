using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class UILayerController : MonoBehaviour
{
    public enum LayerPriority
    {
        Lowest,
        Low,
        Medium,
        High,
        Highest
    }

    public LayerPriority _layerPriority;
    private Canvas _canvas;

    private void Awake()
    {
        SetupLayer();
    }

    private void OnValidate()
    {
        SetupLayer();
    }

    private void SetupLayer()
    {
        _canvas = GetComponent<Canvas>();
        if (_canvas == null)
        {
            _canvas = gameObject.AddComponent<Canvas>();
        }

        _canvas.overrideSorting = true;
        _canvas.sortingOrder = (int)_layerPriority;

        // 判断如果物体下存在Button组件，则添加GraphicRaycaster组件
        if (GetComponentInChildren<Button>() != null)
        {
            GraphicRaycaster raycaster = GetComponent<GraphicRaycaster>();
            if (raycaster == null)
            {
                gameObject.AddComponent<GraphicRaycaster>();
            }
        }
    }
}
