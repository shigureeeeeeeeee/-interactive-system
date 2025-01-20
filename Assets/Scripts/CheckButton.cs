using UnityEngine;
using UnityEngine.UI;

public class CheckButton : MonoBehaviour
{
    private Button button;
    private PlayerController playerController;

    void Start()
    {
        button = GetComponent<Button>();
        playerController = FindObjectOfType<PlayerController>();

        if (button != null && playerController != null)
        {
            button.onClick.AddListener(() => {
                Debug.Log("Check button clicked");
                playerController.CheckAnswer();
            });
        }
        else
        {
            Debug.LogError("Button or PlayerController not found!");
        }
    }
} 