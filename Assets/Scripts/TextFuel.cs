using UnityEngine;
using TMPro;

public class TextFuel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText;
     private int totalScore = 0; // Accumulated score

    public void AddScore(int score)
    {
        totalScore += score; // Add the new points to the total
        scoreText.text = totalScore.ToString("0"); // Update the score text
    }
}
