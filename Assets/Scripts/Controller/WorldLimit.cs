using UnityEngine;

public class WorldLimit : MonoBehaviour
{
    public Transform minPos;
    public Transform maxPos;

    private void OnDrawGizmosSelected()
    {
        DrawWorldLimitRect();
    }

    public static Vector3 CheckLimitPos(Vector3 rawPos)
    {
        Vector3 limitedPos = rawPos;
        limitedPos.x = Mathf.Clamp(limitedPos.x, Instance.minPos.position.x, Instance.maxPos.position.x);
        limitedPos.y = Mathf.Clamp(limitedPos.y, Instance.minPos.position.y, Instance.maxPos.position.y);
        limitedPos.z = Mathf.Clamp(limitedPos.z, Instance.minPos.position.z, Instance.maxPos.position.z);
        return limitedPos;
    }

    private void DrawWorldLimitRect()
    {
        Vector3 center = (minPos.position + maxPos.position) / 2f;
        Vector3 size = maxPos.position - minPos.position;
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(center, size);
    }

    private static WorldLimit instance;
    public static WorldLimit Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<WorldLimit>();
                if (instance == null)
                {
                    Debug.LogError("WorldLimit instance not found in the scene.");
                }
            }
            return instance;
        }
    }
}
