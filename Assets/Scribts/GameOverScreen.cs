using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public Text finalScoreText;
    public Button restartButton;

    void OnEnable()
    {
        if (finalScoreText != null && GameManager.Instance != null)
        {
            finalScoreText.text = "Final Score: " + GameManager.Instance.Points.GetTotalPoints();
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
    }

    void RestartGame()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RestartGame();
        }
    }
}