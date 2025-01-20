using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject clearEffectPrefab;  // パーティクル用プレハブ
    [SerializeField] private GameObject clearUI;           // クリアUI
    [SerializeField] private TextMeshProUGUI clearTimeText; // クリアタイム表示用
    [SerializeField] private Button retryButton;      // リトライボタン
    [SerializeField] private Button titleButton;      // タイトルに戻るボタン

    [Header("Clear Settings")]
    [SerializeField] private AudioClip clearSound;
    private float startTime;
    private bool hasTriggered = false;  // 複数回トリガーを防ぐ

    private void Start()
    {
        startTime = Time.time;
        if (clearUI != null)
        {
            clearUI.SetActive(false);
        }

        // ボタンのイベント設定
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(() => {
                GameManager.Instance.RetryGame();
            });
        }

        if (titleButton != null)
        {
            titleButton.onClick.AddListener(() => {
                GameManager.Instance.BackToTitle();
            });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !hasTriggered)
        {
            hasTriggered = true;
            ShowClearScreen();
        }
    }

    private void ShowClearScreen()
    {
        // プレイヤーの移動を停止
        PlayerController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.StopMovement();
        }

        // エフェクト再生
        if (clearEffectPrefab != null)
        {
            Instantiate(clearEffectPrefab, transform.position, Quaternion.identity);
        }

        // サウンド再生
        if (clearSound != null)
        {
            AudioSource.PlayClipAtPoint(clearSound, transform.position);
        }

        // クリアUI表示
        if (clearUI != null)
        {
            clearUI.SetActive(true);
            if (clearTimeText != null)
            {
                float clearTime = Time.time - startTime;
                clearTimeText.text = $"Clear Time: {clearTime:F2} sec";
            }
        }
        else
        {
            Debug.LogWarning("Clear UI is not assigned in the Goal script!");
        }
    }
} 