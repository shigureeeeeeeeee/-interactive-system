using UnityEngine;
using TMPro;
using System.Collections;

public class MathGate : MonoBehaviour
{
    public enum ProblemType
    {
        Basic,      // 四則演算
        Derivative, // 微分
        Integral,   // 積分
        Calculus     // 微分・積分
    }

    [SerializeField] private TextMeshPro questionText;
    [SerializeField] public ProblemType problemType;
    [SerializeField] private int number1;
    [SerializeField] private int number2;
    
    private float correctAnswer;
    private bool isAnswered = false;
    private string[] operators = { "+", "-", "×", "÷" };
    private string currentOperator;

    // 微分問題用
    private string[] derivativeFunctions = {
        "x^2",      // 答え: 2x
        "x^3",      // 答え: 3x^2
        "sin(x)",   // 答え: cos(x)
        "cos(x)",   // 答え: -sin(x)
        "e^x"       // 答え: e^x
    };

    // 積分問題用
    private string[] integralFunctions = {
        "x",        // 答え: (1/2)x^2
        "x^2",      // 答え: (1/3)x^3
        "cos(x)",   // 答え: sin(x)
        "sin(x)",   // 答え: -cos(x)
        "e^x"       // 答え: e^x
    };

    private int maxNumber;

    [Header("Gate Effects")]
    [SerializeField] private float fadeOutDuration = 1f;  // 消滅にかかる時間
    [SerializeField] private GameObject dissolveEffect;   // オプションのパーティクルエフェクト

    private bool isDerivative;
    private string function;
    private string answer;

    void Start()
    {
        GenerateProblem();
    }

    public void GenerateProblem()
    {
        problemType = ProblemType.Calculus;
        isDerivative = Random.value > 0.5f;
        
        // より多様な関数パターンを選択
        int functionType = Random.Range(0, 8);
        switch (functionType)
        {
            case 0: // x^2
                function = "x^2";
                answer = isDerivative ? "2x" : "(1/3)x^3";
                correctAnswer = isDerivative ? 2 : 1f/3f;
                break;
            
            case 1: // x^3
                function = "x^3";
                answer = isDerivative ? "3x^2" : "(1/4)x^4";
                correctAnswer = isDerivative ? 3 : 0.25f;
                break;
            
            case 2: // sin(x)
                function = "sin(x)";
                answer = isDerivative ? "cos(x)" : "-cos(x)";
                correctAnswer = isDerivative ? 1 : -0.54f;
                break;
            
            case 3: // cos(x)
                function = "cos(x)";
                answer = isDerivative ? "-sin(x)" : "sin(x)";
                correctAnswer = isDerivative ? 0 : 0.84f;
                break;
            
            case 4: // 2x^2 (定数倍)
                function = "2x^2";
                answer = isDerivative ? "4x" : "(2/3)x^3";
                correctAnswer = isDerivative ? 4 : 2f/3f;
                break;
            
            case 5: // x^2 + x (和)
                function = "x^2 + x";
                answer = isDerivative ? "2x + 1" : "(1/3)x^3 + (1/2)x^2";
                correctAnswer = isDerivative ? 3 : 0.83f;
                break;
            
            case 6: // sin(x^2) (合成関数)
                function = "sin(x^2)";
                answer = isDerivative ? "2x·cos(x^2)" : "積分できません";
                correctAnswer = isDerivative ? 0 : 0;
                break;
            
            case 7: // e^x
                function = "e^x";
                answer = isDerivative ? "e^x" : "e^x";
                correctAnswer = isDerivative ? 2.718f : 2.718f;
                break;
        }

        // 問題文を設定
        questionText.text = $"{(isDerivative ? "d/dx" : "∫")} {function} {(isDerivative ? "" : "dx")}";
        Debug.Log($"Generated calculus problem: {questionText.text}, Answer: {answer}, Value: {correctAnswer}");
    }

    public bool CheckDerivativeAnswer(string playerAnswer)
    {
        // 文字列の正規化（空白、大文字小文字、中点を無視）
        string normalizedPlayerAnswer = playerAnswer.ToLower()
            .Replace(" ", "")
            .Replace("·", "")
            .Replace("*", "")
            .Replace("（", "(")
            .Replace("）", ")")
            .Replace("／", "/");
        
        string normalizedCorrectAnswer = answer.ToLower()
            .Replace(" ", "")
            .Replace("·", "")
            .Replace("*", "");
        
        // 積分不可能な場合の特別処理
        if (normalizedCorrectAnswer == "積分できません")
        {
            return normalizedPlayerAnswer == "積分できません" || 
                   normalizedPlayerAnswer == "integralimpossible" ||
                   normalizedPlayerAnswer == "impossible";
        }
        
        return normalizedPlayerAnswer == normalizedCorrectAnswer;
    }

    public float GetCorrectAnswer()
    {
        return correctAnswer;
    }

    public bool IsAnswered()
    {
        return isAnswered;
    }

    public void SetAnswered(bool value)
    {
        isAnswered = value;
        if (isAnswered)
        {
            StartCoroutine(DissolveGate());
        }
    }

    private IEnumerator DissolveGate()
    {
        // エフェクトがあれば再生
        if (dissolveEffect != null)
        {
            Instantiate(dissolveEffect, transform.position, Quaternion.identity);
        }

        // ゲートのマテリアルを取得
        MeshRenderer renderer = GetComponent<MeshRenderer>();
        Material material = renderer.material;
        Color startColor = material.color;
        Color endColor = new Color(startColor.r, startColor.g, startColor.b, 0f);

        // フェードアウト
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float alpha = 1f - (elapsedTime / fadeOutDuration);
            material.color = Color.Lerp(startColor, endColor, elapsedTime / fadeOutDuration);
            yield return null;
        }

        // ゲートを削除
        Destroy(gameObject);
    }

    public void SetMaxNumber(int max)
    {
        // 四則演算で使用する最大数を設定
        maxNumber = max;
    }
} 