using UnityEngine;
using UnityEngine.UI;

public class PointSystem : MonoBehaviour
{
    [Header("UI References")]
    public Text scoreText;

    [Header("Point Settings")]
    public int pointsNearSpawn = 10;
    public int pointsMidway = 8;
    public int pointsNearEnd = 5;

    private int totalPoints = 0;

    private void Start()
    {
        UpdateScoreDisplay();
    }

    public void AddPointsForCustomer(float completionPercentage)
    {
        int pointsToAdd;

        if (completionPercentage < 0.33f) // Near spawn (0-33%)
        {
            pointsToAdd = pointsNearSpawn;
        }
        else if (completionPercentage < 0.66f) // Midway (33-66%)
        {
            pointsToAdd = pointsMidway;
        }
        else // Near end (66-100%)
        {
            pointsToAdd = pointsNearEnd;
        }

        totalPoints += pointsToAdd;
        UpdateScoreDisplay();

        Debug.Log($"Added {pointsToAdd} points. New total: {totalPoints}");
    }

    public int GetTotalPoints()
    {
        return totalPoints;
    }

    private void UpdateScoreDisplay()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + totalPoints;
        }
    }
}