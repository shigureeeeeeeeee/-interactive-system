using UnityEngine;

public class StartPositionGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // 開始位置を球で表示
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
        
        // 進行方向を矢印で表示
        Gizmos.color = Color.blue;
        Vector3 direction = transform.forward * 2f;
        Gizmos.DrawRay(transform.position, direction);
    }
} 