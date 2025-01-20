using UnityEngine;
using TMPro;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;        // 前進速度
    [SerializeField] private float horizontalSpeed = 5f;  // 左右移動速度
    [SerializeField] private float stopDistance = 2f;
    [SerializeField] private GameObject answerUI;
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private LayerMask wallLayer; // 壁のレイヤーを指定
    [SerializeField] private float raycastDistance = 2f;  // 統一したRaycast距離
    
    private bool isMoving = true;
    private MathGate currentGate;
    private Rigidbody rb;
    private float initialY; // 初期Y座標を保存
    private Vector3 currentVelocity;

    [Header("Audio")]
    [SerializeField] private AudioClip wrongAnswerSound;  // 不正解時の効果音

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        
        // Rigidbodyの設定
        rb.useGravity = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        
        initialY = transform.position.y;
    }

    private void Start()
    {
        // UI要素の存在確認
        if (answerUI == null)
        {
            Debug.LogError("Answer UI is not assigned in PlayerController!");
        }
        if (answerInput == null)
        {
            Debug.LogError("Answer Input is not assigned in PlayerController!");
        }

        // 初期状態で非表示
        if (answerUI != null)
        {
            answerUI.SetActive(false);
        }
    }

    private void Update()
    {
        // ゲートの検出
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * raycastDistance, Color.yellow);

        if (!isMoving)
        {
            // 停止中の場合、移動再開の条件をチェック
            if (currentGate == null || currentGate.IsAnswered())
            {
                ResumeMovement();
                return;
            }
        }

        if (Physics.Raycast(transform.position, transform.forward, out hit, raycastDistance))
        {
            MathGate gate = hit.collider.GetComponent<MathGate>();
            if (gate != null)
            {
                Debug.Log($"Gate check - Distance: {hit.distance}, Name: {gate.name}, IsAnswered: {gate.IsAnswered()}, IsMoving: {isMoving}, Current Gate: {(currentGate != null ? currentGate.name : "null")}");

                if (!gate.IsAnswered() && hit.distance < 1.5f && isMoving)
                {
                    StopAtGate(gate);
                }
            }
        }

        // Enterキーでの回答確認
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (!isMoving && currentGate != null)
            {
                CheckAnswer();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!isMoving)
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        // 入力の取得と移動方向の計算
        float horizontalInput = Input.GetAxis("Horizontal");
        Vector3 targetVelocity = transform.forward * moveSpeed;
        
        // 横方向の移動を追加
        if (horizontalInput != 0)
        {
            Vector3 rightMovement = transform.right * horizontalInput * horizontalSpeed;
            if (!Physics.Raycast(transform.position, rightMovement.normalized, rightMovement.magnitude * Time.fixedDeltaTime, wallLayer))
            {
                targetVelocity += rightMovement;
            }
        }

        // 滑らかに速度を変更
        currentVelocity = Vector3.Lerp(currentVelocity, targetVelocity, Time.fixedDeltaTime * 10f);
        
        // 速度を適用
        rb.linearVelocity = new Vector3(currentVelocity.x, 0, currentVelocity.z);
        
        // Y座標を強制的に維持
        rb.position = new Vector3(rb.position.x, initialY, rb.position.z);
    }

    public void CheckAnswer()
    {
        Debug.Log("=== CheckAnswer Debug Info ===");
        Debug.Log($"Is Moving: {isMoving}");
        Debug.Log($"Current Gate: {(currentGate != null ? currentGate.name : "null")}");
        Debug.Log($"Answer Input Text: {answerInput.text}");

        if (currentGate == null)
        {
            Debug.LogWarning("currentGate is null!");
            return;
        }

        if (string.IsNullOrEmpty(answerInput.text))
        {
            Debug.LogWarning("Answer input is empty!");
            return;
        }

        Debug.Log($"Current Gate Type: {currentGate.problemType}");

        if (currentGate.problemType == MathGate.ProblemType.Basic)
        {
            if (float.TryParse(answerInput.text, out float playerAnswer))
            {
                float correctAnswer = currentGate.GetCorrectAnswer();
                Debug.Log($"Player Answer: {playerAnswer}, Correct Answer: {correctAnswer}");
                Debug.Log($"Difference: {Mathf.Abs(playerAnswer - correctAnswer)}");

                if (Mathf.Approximately(playerAnswer, correctAnswer))
                {
                    Debug.Log("Answer is correct!");
                    HandleCorrectAnswer();
                }
                else
                {
                    Debug.Log("Answer is wrong!");
                    HandleWrongAnswer();
                }
            }
            else
            {
                Debug.LogWarning($"Could not parse input: {answerInput.text}");
            }
        }
        else
        {
            // 微分・積分の文字列回答チェック
            string answer = answerInput.text.ToLower().Replace(" ", "");
            if (currentGate.CheckDerivativeAnswer(answer))
            {
                HandleCorrectAnswer();
            }
            else
            {
                HandleWrongAnswer();
            }
        }
    }

    private void HandleCorrectAnswer()
    {
        if (currentGate != null)
        {
            Debug.Log($"Handling correct answer for gate: {currentGate.name}");
            currentGate.SetAnswered(true);
            
            GameManager.Instance.ProblemSolved();
            StartCoroutine(DelayedResumeMovement());
        }
    }

    private void HandleWrongAnswer()
    {
        Debug.Log("Handling wrong answer");
        
        // 入力フィールドを赤く点滅させる
        StartCoroutine(FlashInputField());
        
        // 不正解の効果音を再生（オプション）
        if (wrongAnswerSound != null)
        {
            AudioSource.PlayClipAtPoint(wrongAnswerSound, transform.position);
        }
        
        // 入力をクリア
        answerInput.text = "";
    }

    // 入力フィールドを点滅させるコルーチン
    private IEnumerator FlashInputField()
    {
        Color originalColor = answerInput.textComponent.color;
        Color wrongColor = Color.red;
        
        for (int i = 0; i < 2; i++) // 2回点滅
        {
            answerInput.textComponent.color = wrongColor;
            yield return new WaitForSeconds(0.2f);
            answerInput.textComponent.color = originalColor;
            yield return new WaitForSeconds(0.2f);
        }
    }

    public void StopMovement()
    {
        isMoving = false;
    }

    public void SetMoveSpeed(float speed)
    {
        moveSpeed = speed;
    }

    // ゲートでの停止処理を分離
    private void StopAtGate(MathGate gate)
    {
        Debug.Log($"Stopping at gate: {gate.name}");
        isMoving = false;
        currentGate = gate;
        
        // UIの表示を確実に
        if (answerUI != null)
        {
            answerUI.SetActive(true);
            // 入力フィールドをクリアして選択状態に
            if (answerInput != null)
            {
                answerInput.text = "";
                answerInput.Select();
                answerInput.ActivateInputField();
            }
        }
        else
        {
            Debug.LogError("AnswerUI is null!");
        }
        
        rb.linearVelocity = Vector3.zero;
    }

    // 移動再開処理を分離
    private void ResumeMovement()
    {
        Debug.Log($"Resuming movement - Previous Gate: {(currentGate != null ? currentGate.name : "null")}");
        isMoving = true;
        currentGate = null;
        
        if (answerUI != null)
        {
            answerUI.SetActive(false);
            if (answerInput != null)
            {
                answerInput.text = "";
            }
        }
        
        // 速度を完全にリセット
        currentVelocity = Vector3.zero;
        rb.linearVelocity = Vector3.zero;
    }

    private IEnumerator DelayedResumeMovement()
    {
        yield return new WaitForSeconds(0.2f);
        ResumeMovement();
    }
} 