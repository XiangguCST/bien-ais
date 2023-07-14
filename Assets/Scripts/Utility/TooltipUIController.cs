using UnityEngine;
using UnityEngine.SceneManagement;

public class TooltipUIController<T> where T : MonoBehaviour
{
    private static TooltipUIController<T> instance;
    private T tooltipUIScript;
    private GameObject tooltipUIInstance;
    private Canvas canvas;
    private string uiPrefabPath;

    private TooltipUIController(string uiPrefabPath)
    {
        // 加载UI预设体
        this.uiPrefabPath = uiPrefabPath;
        InitUI();

        // 订阅场景加载事件
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitUI();
    }

    private Canvas FindRootCanvas()
    {
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            if (canvas.transform.parent == null && canvas.name == "Canvas")
            {
                return canvas;
            }
        }
        return null;
    }

    private void InitUI()
    {
        // 加载UI预设体
        GameObject tooltipUIPrefab = Resources.Load<GameObject>(uiPrefabPath);
        canvas = FindRootCanvas();

        // 实例化预设体
        tooltipUIInstance = GameObject.Instantiate(tooltipUIPrefab);
        tooltipUIInstance.transform.SetParent(canvas.transform, false);

        // 获取预设体上的脚本
        tooltipUIScript = tooltipUIInstance.GetComponent<T>();
        if (tooltipUIScript == null)
        {
            Debug.LogError($"The prefab at {uiPrefabPath} does not have a component of type {typeof(T)}.");
        }

        // 默认隐藏UI
        HideTooltipUI();
    }

    public static TooltipUIController<T> GetInstance(string uiPrefabPath)
    {
        if (instance == null)
        {
            instance = new TooltipUIController<T>(uiPrefabPath);
        }
        return instance;
    }

    public T GetTooltipUIScript()
    {
        return tooltipUIScript;
    }

    public void ShowTooltipUI()
    {
        tooltipUIInstance.SetActive(true);
    }

    /// <summary>
    /// 更新UI位置至鼠标位置
    /// </summary>
    public void UpdateUIPositionToMouse()
    {
        if (tooltipUIInstance != null)
        {
            // 获取鼠标在视口的坐标
            Vector2 mousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector2 canvasSize = canvas.GetComponent<RectTransform>().sizeDelta;

            // 将视口坐标转换为画布坐标
            Vector2 mousePositionOnCanvas = new Vector2(
                ((mousePosition.x * canvasSize.x) - (canvasSize.x * 0.5f)),
                ((mousePosition.y * canvasSize.y) - (canvasSize.y * 0.5f)));

            // UI显示在右上角
            RectTransform rectTrans = tooltipUIInstance.GetComponent<RectTransform>();
            Vector2 pivotOffset = new Vector2(rectTrans.rect.width * rectTrans.pivot.x * rectTrans.localScale.x,
                                              rectTrans.rect.height * (1f - rectTrans.pivot.y) * rectTrans.localScale.y);
            Vector2 anchoredPosition = mousePositionOnCanvas + pivotOffset;

            // 防止超出边界
            float maxX = anchoredPosition.x + rectTrans.rect.width;
            if (maxX > canvasSize.x / 2)
            {
                // UI显示在左上角 
                pivotOffset = new Vector2(-rectTrans.rect.width * rectTrans.pivot.x * rectTrans.localScale.x,
                                          rectTrans.rect.height * (1f - rectTrans.pivot.y) * rectTrans.localScale.y);
                anchoredPosition = mousePositionOnCanvas + pivotOffset;
            }

            // 设置 UI 的位置
            rectTrans.anchoredPosition = anchoredPosition;
        }
    }

    public void HideTooltipUI()
    {
        if (tooltipUIInstance != null)
        {
            // 隐藏UI
            tooltipUIInstance.SetActive(false);
        }
    }
}
