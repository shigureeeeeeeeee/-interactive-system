using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Camera Position")]
    [SerializeField] private Vector3 offset = new Vector3(0, 5, -7);
    [SerializeField] private float height = 5f;
    [SerializeField] private float distance = 7f;
    [SerializeField] private float angle = 45f;

    [Header("Camera Settings")]
    [SerializeField] private float fieldOfView = 60f;
    [SerializeField] private bool smoothFollow = true;
    [SerializeField] private float smoothSpeed = 5f;

    private Transform target;
    private Camera cam;

    void Start()
    {
        // プレイヤーを検索
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            target = player.transform;
        }

        // カメラコンポーネントの取得と初期設定
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
        }

        // 初期位置を設定
        UpdateCameraPosition();
    }

    void LateUpdate()
    {
        if (target != null)
        {
            if (smoothFollow)
            {
                // 目標位置を計算
                Vector3 targetPosition = CalculateCameraPosition();
                // 滑らかに移動
                transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            }
            else
            {
                // 直接位置を更新
                UpdateCameraPosition();
            }

            // ターゲットを見る
            transform.LookAt(target);
        }
    }

    private void UpdateCameraPosition()
    {
        transform.position = CalculateCameraPosition();
    }

    private Vector3 CalculateCameraPosition()
    {
        // プレイヤーの位置を基準に、角度と距離から位置を計算
        float angleInRadians = angle * Mathf.Deg2Rad;
        float x = target.position.x + offset.x;
        float y = target.position.y + height;
        float z = target.position.z - distance;

        // Y軸回転の適用
        Vector3 rotatedOffset = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0) * new Vector3(offset.x, 0, -distance);
        
        return new Vector3(
            target.position.x + rotatedOffset.x,
            target.position.y + height,
            target.position.z + rotatedOffset.z
        );
    }

    // インスペクターでの値変更時に即座に反映
    private void OnValidate()
    {
        if (Application.isPlaying && target != null)
        {
            UpdateCameraPosition();
        }
    }

    // 外部からカメラ設定を変更するためのメソッド
    public void SetCameraSettings(float newHeight, float newDistance, float newAngle)
    {
        height = newHeight;
        distance = newDistance;
        angle = newAngle;
        UpdateCameraPosition();
    }

    public void SetFieldOfView(float newFOV)
    {
        fieldOfView = newFOV;
        if (cam != null)
        {
            cam.fieldOfView = fieldOfView;
        }
    }
} 