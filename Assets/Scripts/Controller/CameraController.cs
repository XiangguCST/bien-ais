using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform player1;
    public Transform player2;
    public float cameraPadding = 2f;

    private Camera cam;
    private float initialSizeRate;
    private float initialSize;
    private Vector3 initialPosition;
    private Vector3 initialOffset;

    private void Awake()
    {
        // 计算两个玩家之间的中心点
        Vector3 centerPoint = (player1.position + player2.position) / 2f;
        // 计算两个玩家之间的距离
        float distance = Vector3.Distance(player1.position, player2.position);
        float targetSize = distance / 2f + cameraPadding;
        cam = GetComponent<Camera>();
        initialSize = cam.orthographicSize;
        initialSizeRate = cam.orthographicSize / targetSize;
        initialPosition = transform.position;
        initialOffset = transform.position - centerPoint;
    }

    private void Update()
    {
        if (InputController.Instance._isGameOver) return;
        // 计算两个玩家之间的中心点
        Vector3 centerPoint = (player1.position + player2.position) / 2f;

        // 计算两个玩家之间的距离
        float distance = Vector3.Distance(player1.position, player2.position);

        // 根据距离调整相机的尺寸
        float targetSize = (distance / 2f + cameraPadding) * initialSizeRate;
        targetSize = Mathf.Max(initialSize, targetSize);

        // 根据中心点和目标尺寸计算相机的位置
        
        Vector3 targetPosition = centerPoint + initialOffset;
        targetPosition.z = initialPosition.z;

        // 平滑地调整相机的尺寸和位置
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetSize, Time.deltaTime * 5f);
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * 5f);
    }
}
