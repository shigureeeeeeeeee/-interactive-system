using UnityEngine;

public class StageManager : MonoBehaviour
{
    [System.Serializable]
    public class StageSettings
    {
        public int requiredSolves;      // クリアに必要な問題数
        public float playerSpeed;        // プレイヤーの移動速度
        public float gateSpacing;        // ゲート間の距離
        public int maxNumber;            // 四則演算で使用する最大数
    }

    [Header("Prefab References")]
    [SerializeField] private StageSettings easySettings;
    [SerializeField] private StageSettings normalSettings;
    [SerializeField] private StageSettings hardSettings;
    [SerializeField] private GameObject gatePrefab;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject goalPrefab;  // ゴールのプレハブ
    [SerializeField] private Transform startPosition;

    private void Awake()
    {
        if (easySettings == null)
        {
            easySettings = new StageSettings
            {
                requiredSolves = 3,
                playerSpeed = 4f,
                gateSpacing = 10f,
                maxNumber = 10
            };
        }

        if (normalSettings == null)
        {
            normalSettings = new StageSettings
            {
                requiredSolves = 5,
                playerSpeed = 5f,
                gateSpacing = 12f,
                maxNumber = 20
            };
        }

        if (hardSettings == null)
        {
            hardSettings = new StageSettings
            {
                requiredSolves = 7,
                playerSpeed = 6f,
                gateSpacing = 15f,
                maxNumber = 30
            };
        }
    }

    private void Start()
    {
        SetupStage();
    }

    private void SetupStage()
    {
        StageSettings currentSettings = GetCurrentSettings();
        
        // プレハブの存在確認
        if (gatePrefab == null)
        {
            Debug.LogError("Gate Prefab is not assigned in StageManager!");
            return;
        }

        // より詳細なデバッグ情報
        Debug.Log("=== Stage Setup Debug Info ===");
        Debug.Log($"Current Difficulty: {GameDifficulty.CurrentDifficulty}");
        Debug.Log($"Easy Settings - Required Solves: {easySettings.requiredSolves}");
        Debug.Log($"Normal Settings - Required Solves: {normalSettings.requiredSolves}");
        Debug.Log($"Hard Settings - Required Solves: {hardSettings.requiredSolves}");
        Debug.Log($"Current Settings - Required Solves: {currentSettings.requiredSolves}");

        // 既存のプレイヤーを探して設定を更新
        GameObject existingPlayer = GameObject.FindGameObjectWithTag("Player");
        if (existingPlayer != null)
        {
            PlayerController playerController = existingPlayer.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.SetMoveSpeed(currentSettings.playerSpeed);
                existingPlayer.transform.position = startPosition.position;
            }
        }
        else
        {
            // プレイヤーが存在しない場合のみ新規生成
            GameObject player = Instantiate(playerPrefab, startPosition.position, Quaternion.identity);
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.SetMoveSpeed(currentSettings.playerSpeed);
        }

        // ゲートの生成
        float lastGatePosition = 0;
        Debug.Log($"Starting gate generation. Required solves: {currentSettings.requiredSolves}");
        
        for (int i = 0; i < currentSettings.requiredSolves; i++)
        {
            Vector3 gatePosition = startPosition.position + Vector3.forward * (i + 1) * currentSettings.gateSpacing;
            Debug.Log($"Attempting to create gate {i} at position: {gatePosition}");
            
            GameObject gate = Instantiate(gatePrefab, gatePosition, Quaternion.identity);
            if (gate == null)
            {
                Debug.LogError($"Failed to instantiate gate {i}");
                continue;
            }
            
            // ゲートの設定を確認
            MathGate mathGate = gate.GetComponent<MathGate>();
            if (mathGate == null)
            {
                Debug.LogError($"MathGate component missing on instantiated gate {i}");
                continue;
            }
            
            mathGate.SetMaxNumber(currentSettings.maxNumber);
            Debug.Log($"Gate {i} successfully created with MathGate component");
            
            lastGatePosition = gatePosition.z;
        }

        // ゴールの生成
        if (goalPrefab != null)
        {
            // 最後のゲートの少し先にゴールを配置
            Vector3 goalPosition = startPosition.position + Vector3.forward * (lastGatePosition + currentSettings.gateSpacing);
            GameObject goal = Instantiate(goalPrefab, goalPosition, Quaternion.identity);
            
            // GameManagerにゴールを設定
            if (GameManager.Instance != null)
            {
                GameManager.Instance.SetGoal(goal);
            }
            
            Debug.Log($"Goal created at position: {goalPosition}");
        }
        else
        {
            Debug.LogError("Goal Prefab is not assigned in StageManager!");
        }

        // GameManagerの問題数を設定
        GameManager.Instance.SetRequiredSolves(currentSettings.requiredSolves);
        Debug.Log($"Final GameManager Required Solves: {GameManager.Instance.GetRequiredSolves()}");
    }

    private StageSettings GetCurrentSettings()
    {
        Debug.Log($"Getting settings for difficulty: {GameDifficulty.CurrentDifficulty}");
        
        StageSettings settings = GameDifficulty.CurrentDifficulty switch
        {
            GameDifficulty.Difficulty.Easy => easySettings,
            GameDifficulty.Difficulty.Normal => normalSettings,
            GameDifficulty.Difficulty.Hard => hardSettings,
            _ => easySettings
        };
        
        Debug.Log($"Selected settings - Required Solves: {settings.requiredSolves}");
        return settings;
    }
} 