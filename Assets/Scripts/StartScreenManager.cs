using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScreenManager : MonoBehaviour
{
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject difficultyPanel;

    private void Start()
    {
        startPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    public void ShowDifficultySelection()
    {
        startPanel.SetActive(false);
        difficultyPanel.SetActive(true);
    }

    public void StartGame(int difficulty)
    {
        GameDifficulty.Difficulty selectedDifficulty = (GameDifficulty.Difficulty)difficulty;
        Debug.Log($"Setting difficulty to: {selectedDifficulty}");
        GameDifficulty.SetDifficulty(selectedDifficulty);
        SceneManager.LoadScene("GameScene"); // ゲームシーンの名前
    }

    public void BackToStart()
    {
        startPanel.SetActive(true);
        difficultyPanel.SetActive(false);
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
} 