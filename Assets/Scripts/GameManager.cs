using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int requiredSolves = 3;  // クリアに必要な問題数
    [SerializeField] private GameObject goalObject;    // ゴールオブジェクト
    [SerializeField] private TextMeshProUGUI progressText;  // 進捗表示用テキスト
    [SerializeField] private AudioClip clearSound;    // クリア音
    
    private int solvedCount = 0;
    private AudioSource audioSource;

    private static GameManager instance;
    public static GameManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateProgressText();
        if (goalObject != null)
        {
            goalObject.SetActive(false);
        }
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void ProblemSolved()
    {
        solvedCount++;
        UpdateProgressText();

        if (solvedCount >= requiredSolves)
        {
            ActivateGoal();
        }
    }

    private void UpdateProgressText()
    {
        if (progressText != null)
        {
            progressText.text = $"Solved: {solvedCount} / {requiredSolves}";
        }
    }

    private void ActivateGoal()
    {
        if (goalObject != null)
        {
            goalObject.SetActive(true);
            
            // クリア音を再生
            if (clearSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(clearSound);
            }
        }
    }

    public void RetryGame()
    {
        // スタート画面のシーン名を "StartScene" と仮定
        SceneManager.LoadScene("StartScene");
    }

    // タイトルに戻るメソッドも追加（オプション）
    public void BackToTitle()
    {
        SceneManager.LoadScene("StartScene");
    }

    public void SetRequiredSolves(int count)
    {
        requiredSolves = count;
        UpdateProgressText();
    }

    public int GetRequiredSolves()
    {
        return requiredSolves;
    }

    public void SetGoal(GameObject goal)
    {
        goalObject = goal;
        if (goalObject != null)
        {
            goalObject.SetActive(false);
        }
    }
} 