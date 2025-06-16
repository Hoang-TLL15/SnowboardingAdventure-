using UnityEngine;
using TMPro; // Use UnityEngine.UI if using legacy Text

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    void Update()
    {
        if (GameManager.Instance != null)
        {
            scoreText.text = "Score: " + GameManager.Instance.Score;
        }
    }
}