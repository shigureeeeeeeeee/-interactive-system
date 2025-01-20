using UnityEngine;

public class GameDifficulty : MonoBehaviour
{
    public enum Difficulty
    {
        Easy,       // 四則演算のみ
        Normal,     // 四則演算 + 簡単な微分
        Hard        // すべての問題タイプ
    }

    public static Difficulty CurrentDifficulty { get; private set; }

    public static void SetDifficulty(Difficulty difficulty)
    {
        CurrentDifficulty = difficulty;
    }
} 